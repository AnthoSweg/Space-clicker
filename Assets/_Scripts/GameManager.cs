using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //int de 0 à 18 446 744 073 709 551 615
    //public ulong currency;
    public double currency;
    public List<Planet> planetsOwned = new List<Planet>();

    public Planet focusedPlanet;

    private float oneSecondTimer;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Block actions if the click hits a UI element
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (focusedPlanet != null)
                focusedPlanet.IncreaseHapiness();
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

        float gainedCurrency = 0;
        for (int i = 0; i < planetsOwned.Count; i++)
        {
            gainedCurrency += planetsOwned[i].production;
        }
        currency += gainedCurrency * Time.deltaTime;

        UpdateCurrencyUI(gainedCurrency);
        UpdateHapinessSlider();
    }

    private void FocusPlanet(Planet planet)
    {
        focusedPlanet = planet;
        GameAssets.Main.PlanetPanel.SetActive(true);
        focusedPlanet.GetNewHapinessState();
        //GameAssets.Main.ShopPanel.SetActive(true);
        //UpdateShopUI();
    }

    public void UnfocusPlanet()
    {
        focusedPlanet = null;
        //GameAssets.Main.ShopPanel.SetActive(false);
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

    private float sliderSmoothness = .95f;
    void UpdateHapinessSlider()
    {
        if(focusedPlanet != null)
        {
            GameAssets.Main.hapinessSlider.value = Mathf.Lerp(GameAssets.Main.hapinessSlider.value, focusedPlanet.hapinessPoint / (float)Hapiness.maximumJoy, sliderSmoothness*Time.deltaTime);
        }
    }
}