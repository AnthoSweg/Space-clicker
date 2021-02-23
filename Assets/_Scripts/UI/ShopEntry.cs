using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopEntry : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI price;
    public TextMeshProUGUI upgradesOwned;
    private Planet planet;

    public void SetupEntry(Planet p)
    {
        planet = p;
        icon.sprite = planet.data.faces[3];
        UpdateEntry();
    }

    private void UpdateEntry()
    {

        price.text = string.Format("Buy upgrade for {0}", planet.upgradeCost.ToString("F2"));
        upgradesOwned.text = string.Format("owned : {0}", planet.state.upgradeOwned.ToString());
    }

    public void BuyUpgrade()
    {
        planet.BuyUpgrade();
        UpdateEntry();
    }
}
