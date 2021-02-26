using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopEntry : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI price;
    public TextMeshProUGUI nameTextMesh;
    public TextMeshProUGUI dps;
    public TextMeshProUGUI dpsGain;
    public TextMeshProUGUI upgradesOwned;
    public TextMeshProUGUI lvlup;
    public Button btn;
    private Planet planet;

    public Color pink;
    public Color blue;

    private float currenCost;
    private int buyNbr = 1;

    public void SetupEntry(Planet p)
    {
        planet = p;
        nameTextMesh.text = planet.data.planetName;
        icon.sprite = planet.data.shopIcon;
        GetCurrentCost(1);
        UpdateEntry();
    }

    public void UpdateEntry()
    {
        if (planet.state.unlocked)
        {
            upgradesOwned.text = string.Format("Lv. <color=#{1}>{0}</color>", planet.state.upgradeOwned.ToString(), ColorUtility.ToHtmlStringRGB(blue));
            dps.text = string.Format("Joy per second: <color=#{1}>{0} Joy</color>", planet.normalProduction.ToString("F1"), ColorUtility.ToHtmlStringRGB(pink));

            price.text = string.Format("Joy {0}", currenCost.ToString("F1"));
            lvlup.text = ("level up");
            dpsGain.text = string.Format("+{0} Joy/s", (planet.nextUpgradeProduction * buyNbr).ToString("F1"));

        }
        else
        {
            price.text = string.Format("Joy {0}", currenCost.ToString("F1"), planet.data.planetName);
            lvlup.text = ("UNLOCK");
            upgradesOwned.text = string.Empty;
            dps.text = string.Empty;
            dpsGain.text = string.Empty;
        }
        btn.interactable = GameState.CurrentState.currency >= currenCost;
    }

    public void GetCurrentCost(int buyNbr)
    {
        this.buyNbr = buyNbr;
        currenCost = planet.data.baseCost * (((Mathf.Pow(planet.data.rateGrowth, buyNbr) - 1) * Mathf.Pow(planet.data.rateGrowth, planet.state.upgradeOwned)) / (planet.data.rateGrowth - 1));
    }

    public void BuyUpgrade()
    {
        if (GameState.CurrentState.currency >= currenCost)
        {
            GameState.CurrentState.currency -= currenCost;
            for (int i = 0; i < buyNbr; i++)
            {
                planet.BuyUpgrade();
            }
            UpdateEntry();
        }
    }
}

