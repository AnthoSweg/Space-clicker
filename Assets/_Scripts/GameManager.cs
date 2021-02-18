using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //int de 0 à 18 446 744 073 709 551 615
    //public ulong currency;
    public List<Planet> allPlanets = new List<Planet>();
    private static List<PlanetState> allPlanetsState = new List<PlanetState>();

    private static double currency;
    //public List<Planet> planetsOwned = new List<Planet>();

    public Planet focusedPlanet;

    [HideInInspector] public bool gameStarted = false;

    //I don't put anything in Start or Awake, this way I have control on the order of execution of everything and make sure there are no errors
    public void Initialize()
    {
        for (int i = 0; i < allPlanets.Count; i++)
        {
            allPlanets[i].Initialize();
        }
        allPlanetsState.Clear();
        for (int i = 0; i < allPlanets.Count; i++)
        {
            allPlanetsState.Add(allPlanets[i].state);
        }

        currency = SaveFile.CurrentState.currency;
        TimeSpan timeWhileAway = DateTime.UtcNow - SaveFile.CurrentState.TimeStampUTC;
        Debug.Log(timeWhileAway.TotalSeconds);
        GainCurrency((float)timeWhileAway.TotalSeconds);

        Planet.defaultMultiplicatorTextFontSize = GameAssets.Main.multiplicatorTextMesh.fontSize;

        Save();
    }

    void Update()
    {
        //IF the game is not yet started (play game menu), don't start creating currency
        if (!gameStarted)
            return;


        if (Input.GetMouseButtonDown(0))
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
                RaycastHit2D hit = Physics2D.Raycast(GameAssets.Main.cam.ScreenToWorldPoint(Input.mousePosition), GameAssets.Main.cam.transform.forward);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent(out Planet planet))
                    {
                        FocusPlanet(planet);
                    }
                }
            }
        }

        GainCurrency();

        UpdatePlanetPanel();

        //Check mobile input
        //if(Input.GetKeyDown(KeyCode.bac))
    }

    private void GainCurrency(float second = 1f)
    {
        float gainedCurrency = 0;
        for (int i = 0; i < allPlanets.Count; i++)
        {
            if (allPlanets[i].state.unlocked)
                gainedCurrency += allPlanets[i].production;
        }
        currency += gainedCurrency * Time.deltaTime * second;
        Debug.Log(gainedCurrency * Time.deltaTime * second);

        UpdateCurrencyUI(gainedCurrency);
    }

    private void FocusPlanet(Planet planet)
    {
        focusedPlanet = planet;
        GameAssets.Main.PlanetPanel.SetActive(true);
        focusedPlanet.GetNewHapinessState();
        GameAssets.Main.ShopPanel.SetActive(true);
        UpdateShopUI();
    }

    public void UnfocusPlanet()
    {
        focusedPlanet = null;
        GameAssets.Main.ShopPanel.SetActive(false);
        GameAssets.Main.PlanetPanel.SetActive(false);
    }

    public void UpgradeSelectedPlanet()
    {
        if (focusedPlanet != null)
        {
            if (currency >= focusedPlanet.upgradeCost)
            {
                currency -= focusedPlanet.upgradeCost;
                focusedPlanet.BuyUpgrade();
                UpdateCurrencyUI();
                UpdateShopUI();

                Save();
            }
            else
            {
                Debug.LogFormat("<color=red>Not enough money : {0} out of {1}</color>", currency, focusedPlanet.upgradeCost);
            }
        }
    }

    public void UpdateShopUI()
    {
        GameAssets.Main.upgradeTextMesh.text = string.Format("Upgrade {0} for {1}", focusedPlanet.planetName, focusedPlanet.upgradeCost.ToString("F2"));
    }

    private void UpdateCurrencyUI(float gainedCurrency = -1)
    {
        GameAssets.Main.currencyTextMesh.text = currency.ToString("F2");

        if (gainedCurrency != -1)
            GameAssets.Main.currencyProdTextMesh.text = gainedCurrency.ToString("F2");
    }

    //private float sliderSmoothness = .95f;
    void UpdatePlanetPanel()
    {
        if (focusedPlanet != null)
        {
            GameAssets.Main.hapinessSlider.value = focusedPlanet.state.hapinessPoint / (float)Hapiness.maximumJoy;
            GameAssets.Main.planetProdTextMesh.text = focusedPlanet.production.ToString("F2");
        }
    }

    public static void Save()
    {
        SaveFile.CurrentState.currency = currency;
        SaveFile.CurrentState.planetStates = allPlanetsState;
        SaveFile.Write();
    }
}