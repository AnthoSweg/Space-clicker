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
    private Planet planet;

    public Color pink;
    public Color blue;

    public void SetupEntry(Planet p)
    {
        planet = p;
        nameTextMesh.text = planet.data.planetName;
        icon.sprite = planet.data.shopIcon;
        UpdateEntry();
    }

    private void UpdateEntry()
    {
        if (planet.state.unlocked)
        {
            upgradesOwned.text = string.Format("Lv. <color=#{1}>{0}</color>", planet.state.upgradeOwned.ToString(), ColorUtility.ToHtmlStringRGB(blue));
            dps.text = string.Format("Joy per second: <color=#{1}>{0} Joy</color>", planet.normalProduction.ToString("F1"), ColorUtility.ToHtmlStringRGB(pink));

            price.text = string.Format("Joy {0}", planet.upgradeCost.ToString("F1"));
            lvlup.text = ("level up");
            dpsGain.text = string.Format("+{0} Joy/s", planet.nextUpgradeProduction.ToString("F1"));
        }
        else
        {
            price.text = string.Format("Joy {0}", planet.upgradeCost.ToString("F1"), planet.data.planetName);
            lvlup.text = ("UNLOCK");
            upgradesOwned.text = string.Empty;
            dps.text = string.Empty;
            dpsGain.text = string.Empty;
        }
    }

    public void BuyUpgrade()
    {
        if (GameState.CurrentState.currency >= planet.upgradeCost)
        {
            GameState.CurrentState.currency -= planet.upgradeCost;
            planet.BuyUpgrade();
            UpdateEntry();
        }
    }
}

