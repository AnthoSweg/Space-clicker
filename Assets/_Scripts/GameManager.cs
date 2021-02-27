using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public List<Planet> allPlanets = new List<Planet>();
    private static List<PlanetState> allPlanetsState = new List<PlanetState>();
    public static List<Planet> ownedPlanets = new List<Planet>();

    public Planet focusedPlanet;

    public static bool gameStarted = false;

    //I don't put anything in Start or Awake, this way I have control on the order of execution of everything and make sure there are no errors
    public void Initialize()
    {
        //Planet.defaultMultiplicatorTextFontSize = GameAssets.Main.multiplicatorTextMesh.fontSize;

        //Setup all planets
        for (int i = 0; i < allPlanets.Count; i++)
        {
            int index = i;
            allPlanets[index].data = GameParams.Main.planetDatas[index];
            allPlanets[index].Initialize(index);
        }
        allPlanetsState.Clear();
        ownedPlanets.Clear();
        for (int i = 0; i < allPlanets.Count; i++)
        {
            allPlanetsState.Add(allPlanets[i].state);
            if (allPlanets[i].state.unlocked)
                ownedPlanets.Add(allPlanets[i]);
        }

        //Get currency based on time spent not on app
        TimeSpan timeWhileAway = DateTime.UtcNow - GameState.CurrentState.TimeStampUTC;
        double timeAwayInSeconds = timeWhileAway.TotalSeconds;
        timeAwayInSeconds = Mathf.Clamp((float)timeAwayInSeconds, 0, GameParams.Main.baseMaxTimeAwayInHour * 3600); //max time away is in hour
        GainCurrency((float)timeWhileAway.TotalSeconds);

        GameAssets.Main.PlanetPanel.SetActive(false);

        GameAssets.Main.camController.cam.orthographicSize = 50;
        GameAssets.Main.camController.ZoomOut();

        Save();
    }

    private Vector3 mousePos;
    void Update()
    {
        //IF the game is not yet started (play game menu), don't start creating currency
        if (!gameStarted)
            return;

        if (!Application.isMobilePlatform)
        {
            mousePos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
            if (Input.GetKeyDown(KeyCode.S))
                Save();
        }
        else
        {
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                mousePos = Input.touches[0].position;
                OnClick();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu))
                Save();
        }


        Orbit();

        GainCurrency();

        UpdatePlanetPanel();
    }

    public void OnClick()
    {
        //Block actions if the click hits a UI element
        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0)|| EventSystem.current.IsPointerOverGameObject(-1))
            return;

        if (focusedPlanet != null)
        {
            focusedPlanet.IncreaseHapiness();
            //GainCurrency();
            Vector3 clickpos = GameAssets.Main.camController.cam.ScreenToWorldPoint(mousePos);
            clickpos = new Vector3(clickpos.x, clickpos.y, 0);
            GameAssets.Main.tapEffect.transform.position = clickpos;
            GameAssets.Main.tapEffect.Play();
            focusedPlanet.SetLookAtTarget(clickpos);
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
    void Orbit()
    {
        for (int i = 0; i < ownedPlanets.Count; i++)
        {
            newPos = Vector2.zero;
            if (focusedPlanet == null)
            {
                timeInGame += Time.deltaTime;
            }
            //First planet (index 0) is at the center and must not orbit
            if (i > 0)
                newPos = GameParams.Main.orbit.Evaluate(((1f / (ownedPlanets.Count - 1)) * i) + timeInGame * GameParams.Main.orbitSpeed);

            float yOffset = Mathf.Sin(Time.time * (3 * ((int)ownedPlanets[i].hapiness + 1))) * GameParams.Main.amplitude;
            newPos += new Vector2(0, yOffset);

            ownedPlanets[i].pTransform.position = newPos;
        }
    }

    private void GainCurrency(float second = 1f)
    {
        float gainedCurrency = 0;
        for (int i = 0; i < ownedPlanets.Count; i++)
        {
            //if (allPlanets[i].state.unlocked)
            gainedCurrency += ownedPlanets[i].acutalProduction;
        }
        GameState.CurrentState.currency += gainedCurrency * Time.deltaTime * second;

        UpdateCurrencyUI(gainedCurrency);
    }

    private void FocusPlanet(Planet planet)
    {
        focusedPlanet = planet;
        focusedPlanet.Focus(true);
        focusedPlanet.GetNewHapinessState();
        GameAssets.Main.camController.ZoomIn(planet.pTransform);
        GameAssets.Main.PlanetPanel.SetActive(true);
    }

    public void UnfocusPlanet()
    {
        focusedPlanet.SetLookAtTarget(Vector3.zero);
        focusedPlanet.Focus(false);
        focusedPlanet = null;
        GameAssets.Main.camController.ZoomOut();
        GameAssets.Main.PlanetPanel.SetActive(false);
    }

    private void UpdateCurrencyUI(float gainedCurrency = -1)
    {
        GameAssets.Main.currencyTextMesh.text = string.Format("{0} Joy", GameState.CurrentState.currency.ToString("F2"));

        if (gainedCurrency != -1)
            GameAssets.Main.currencyProdTextMesh.text = string.Format("{0} Joy/s", gainedCurrency.ToString("F2"));
    }

    void UpdatePlanetPanel()
    {
        if (focusedPlanet != null)
        {
            GameAssets.Main.hapinessSlider.value = focusedPlanet.state.hapinessPoint / ((GameParams.Main.happinessPointsNeededPerLevel[3] * GameParams.Main.hapinessLevelUpgrade[GameState.CurrentState.upgrades.hapinessLevelUpgrade]));
            GameAssets.Main.planetProdTextMesh.text = string.Format("{0} Joy/s", focusedPlanet.acutalProduction.ToString("F2"));

            //Display the multiplicator value under the hapiness bar
            GameAssets.Main.multiplicatorTextMesh.text = string.Format("x{0}", focusedPlanet.multiplier);
            //GameAssets.Main.multiplicatorTextMesh.fontSize = Planet.defaultMultiplicatorTextFontSize * focusedPlanet.multiplier;
        }
    }

    public void SwitchToNextPlanet()
    {
        focusedPlanet.SetLookAtTarget(Vector3.zero);
        focusedPlanet.Focus(true);
        int index = ownedPlanets.IndexOf(focusedPlanet);
        index++;
        if (index > ownedPlanets.Count - 1)
            index = 0;
        FocusPlanet(ownedPlanets[index]);
    }

    public void SwitchToPrevPlanet()
    {
        focusedPlanet.SetLookAtTarget(Vector3.zero);
        focusedPlanet.Focus(false);
        int index = ownedPlanets.IndexOf(focusedPlanet);
        index--;
        if (index < 0)
            index = ownedPlanets.Count - 1;
        FocusPlanet(ownedPlanets[index]);
    }

    private void OnApplicationPause(bool pause)
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public static void Save()
    {
        SaveFile.Write();
    }
}