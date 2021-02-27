using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UpgradeEntry : MonoBehaviour
{
    public upgradeName entryName;
    public TextMeshProUGUI price;
    public TextMeshProUGUI upgradesOwned;
    public Button btn;
    public TextMeshProUGUI lvlup;

    public float[] costs = new float[10];

    public Color blue;

    private int upg;
    public void UpdateEntry()
    {
        switch (entryName)
        {
            case upgradeName.hapinessLevelUpgrade:
                upg = GameState.CurrentState.upgrades.hapinessLevelUpgrade;
                break;
            case upgradeName.hapinessDecreaseSpeedUpgrade:
                upg = GameState.CurrentState.upgrades.hapinessDecreaseSpeedUpgrade;
                break;
            case upgradeName.multiplicatorUpgrade:
                upg = GameState.CurrentState.upgrades.multiplicatorUpgrade;
                break;
            default:
                break;
        }

        if (upg < costs.Length)
        {
            upgradesOwned.text = string.Format("Lv. <color=#{1}>{0}</color>", upg.ToString(), ColorUtility.ToHtmlStringRGB(blue));
            price.text = string.Format("Joy {0}", costs[upg]);
            lvlup.text = ("level up");
        }
        else
        {
            price.text = string.Empty;
            lvlup.text = ("MAX");

        }
        btn.interactable = GameState.CurrentState.currency >= costs[upg];
    }

    public void BuyUpgrade()
    {
        if (GameState.CurrentState.currency >= costs[upg])
        {
            GameState.CurrentState.currency -= costs[upg];

            switch (entryName)
            {
                case upgradeName.hapinessLevelUpgrade:
                    GameState.CurrentState.upgrades.hapinessLevelUpgrade++;
                    break;
                case upgradeName.hapinessDecreaseSpeedUpgrade:
                    GameState.CurrentState.upgrades.hapinessDecreaseSpeedUpgrade++;
                    break;
                case upgradeName.multiplicatorUpgrade:
                    GameState.CurrentState.upgrades.multiplicatorUpgrade++;
                    break;
                default:
                    break;
            }

            UpdateEntry();
        }
    }

    public enum upgradeName
    {
        hapinessLevelUpgrade,
        hapinessDecreaseSpeedUpgrade,
        multiplicatorUpgrade
    }
}
