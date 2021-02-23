using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //int de 0 à 18 446 744 073 709 551 615
    //public ulong currency;
    public List<Planet> allPlanets = new List<Planet>();
    private static List<PlanetState> allPlanetsState = new List<PlanetState>();
    public static List<Planet> ownedPlanets = new List<Planet>();
    //public int unlockedPlanetsCount
    //{
    //    get
    //    {
    //        int count = 0;
    //        for (int i = 0; i < allPlanets.Count; i++)
    //        {
    //            if (allPlanets[i].state.unlocked)
    //                count++;
    //        }
    //        return count;
    //    }
    //}

    public Planet focusedPlanet;

    [HideInInspector] public bool gameStarted = false;

    public PlayerInput inputs;
    public Inputs controls;

    private void Awake()
    {
        controls = new Inputs();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.General.Click.performed += _ => OnClick();
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.General.Click.performed -= _ => OnClick();
    }

    //I don't put anything in Start or Awake, this way I have control on the order of execution of everything and make sure there are no errors
    public void Initialize()
    {
        //Setup all planets
        for (int i = 0; i < allPlanets.Count; i++)
        {
            int index = i;
            allPlanets[index].data = GameParams.Main.planetDatas[index];
            allPlanets[index].Initialize(index);
        }
        allPlanetsState.Clear();
        for (int i = 0; i < allPlanets.Count; i++)
        {
            allPlanetsState.Add(allPlanets[i].state);
        }
        for (int i = 0; i < allPlanets.Count; i++)
        {
            if (allPlanets[i].state.unlocked)
                ownedPlanets.Add(allPlanets[i]);
        }

        //Get currency based on time spent not on app
        TimeSpan timeWhileAway = DateTime.UtcNow - GameState.CurrentState.TimeStampUTC;
        double timeAwayInSeconds = timeWhileAway.TotalSeconds;
        timeAwayInSeconds = Mathf.Clamp((float)timeAwayInSeconds, 0, GameParams.Main.baseMaxTimeAwayInHour * 3600); //max time away is in hour
        GainCurrency((float)timeWhileAway.TotalSeconds);

        Planet.defaultMultiplicatorTextFontSize = GameAssets.Main.multiplicatorTextMesh.fontSize;

        GameAssets.Main.PlanetPanel.SetActive(false);

        GameAssets.Main.camController.ZoomOut();

        Save();
    }

    void Update()
    {
        //IF the game is not yet started (play game menu), don't start creating currency
        if (!gameStarted)
            return;

        Orbit();

        GainCurrency();

        UpdatePlanetPanel();

        if (Input.GetKeyDown(KeyCode.Escape))
            if (Application.platform == RuntimePlatform.Android)
            {
                Save();
            }
    }

    public void OnClick()
    {
        //Block actions if the click hits a UI element
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (focusedPlanet != null)
        {
            focusedPlanet.IncreaseHapiness();
            GainCurrency();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(GameAssets.Main.camController.cam.ScreenToWorldPoint(Input.mousePosition), GameAssets.Main.camController.cam.transform.forward);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent(out Planet planet))
                {
                    FocusPlanet(planet);
                }
            }
        }
    }

    Vector2 newPos;
    private float timeInGame;
    public float testValue;
    void Orbit()
    {
        if (focusedPlanet == null)
        {
            timeInGame += Time.deltaTime;
            for (int i = 0; i < ownedPlanets.Count; i++)
            {
                //First planet (index 0) is at the center and must not orbit
                if (i > 0)
                {
                    newPos = GameParams.Main.orbit.Evaluate(((1f / (ownedPlanets.Count - 1)) * i) + timeInGame * GameParams.Main.orbitSpeed);
                    //newPos = GameParams.Main.orbit.Evaluate(testValue * i);
                    ownedPlanets[i].pTransform.position = newPos;
                }
            }
        }
    }

    private void GainCurrency(float second = 1f)
    {
        float gainedCurrency = 0;
        for (int i = 0; i < ownedPlanets.Count; i++)
        {
            //if (allPlanets[i].state.unlocked)
                gainedCurrency += ownedPlanets[i].production;
        }
        GameState.CurrentState.currency += gainedCurrency * Time.deltaTime * second;

        UpdateCurrencyUI(gainedCurrency);
    }

    private void FocusPlanet(Planet planet)
    {
        focusedPlanet = planet;
        focusedPlanet.GetNewHapinessState();
        GameAssets.Main.camController.ZoomIn(planet.pTransform);
        GameAssets.Main.PlanetPanel.SetActive(true);
    }

    public void UnfocusPlanet()
    {
        focusedPlanet = null;
        GameAssets.Main.camController.ZoomOut();
        GameAssets.Main.PlanetPanel.SetActive(false);
    }

    public void UpgradeSelectedPlanet()
    {
        if (focusedPlanet != null)
        {
            if (GameState.CurrentState.currency >= focusedPlanet.upgradeCost)
            {
                GameState.CurrentState.currency -= focusedPlanet.upgradeCost;
                focusedPlanet.BuyUpgrade();
                UpdateCurrencyUI();

                Save();
            }
            else
            {
                Debug.LogFormat("<color=red>Not enough money : {0} out of {1}</color>", GameState.CurrentState.currency, focusedPlanet.upgradeCost);
            }
        }
    }

    private void UpdateCurrencyUI(float gainedCurrency = -1)
    {
        GameAssets.Main.currencyTextMesh.text = GameState.CurrentState.currency.ToString("F2");

        if (gainedCurrency != -1)
            GameAssets.Main.currencyProdTextMesh.text = gainedCurrency.ToString("F2");
    }

    void UpdatePlanetPanel()
    {
        if (focusedPlanet != null)
        {
            GameAssets.Main.hapinessSlider.value = focusedPlanet.state.hapinessPoint / (float)Hapiness.maximumJoy;
            GameAssets.Main.planetProdTextMesh.text = string.Format("{0} Joy/s", focusedPlanet.production.ToString("F2"));
        }
    }

    public static void Save()
    {
        SaveFile.Write();
    }
}