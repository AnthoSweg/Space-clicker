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
    private Planet planet;

    public void SetupEntry(Planet p)
    {
        planet = p;
        nameTextMesh.text = planet.data.planetName;
        icon.sprite = planet.data.faces[3];
        UpdateEntry();
    }

    private void UpdateEntry()
    {
        if (planet.state.unlocked)
        {
            upgradesOwned.text = string.Format("Lv. {0}", planet.state.upgradeOwned.ToString());
            dps.text = string.Format("Joy : {0}/s", planet.normalProduction.ToString("F1"));

            price.text = string.Format("Buy upgrade for {0}", planet.upgradeCost.ToString("F1"));
            dpsGain.text = string.Format("+{0} Joy/s", planet.nextUpgradeProduction.ToString("F1"));
        }
        else
        {
            price.text = string.Format("Unlock {1} for {0}", planet.upgradeCost.ToString("F1"), planet.data.planetName);
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

