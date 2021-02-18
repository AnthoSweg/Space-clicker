using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameAssets : MonoBehaviour
{
    private static GameAssets main;
    public static GameAssets Main
    {
        get { if (main == null)
                main = FindObjectOfType<GameAssets>();
            return main;
        }
    }
    public Sprite[] faces;
    public Sprite lockedPlanet;
    public Camera cam;
    public TextMeshProUGUI currencyTextMesh;
    public TextMeshProUGUI currencyProdTextMesh;
    public TextMeshProUGUI upgradeTextMesh;
    public TextMeshProUGUI multiplicatorTextMesh;
    public TextMeshProUGUI planetProdTextMesh;
    public GameObject ShopPanel;
    public GameObject PlanetPanel;
    public Slider hapinessSlider;
}
