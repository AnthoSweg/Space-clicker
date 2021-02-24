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
    [Header("Sprites")]
    public Sprite lockedPlanet;
    [Header("GameObject links")]
    public CameraController camController;
    public GameObject ShopPanel;
    public GameObject PlanetPanel;
    public Slider hapinessSlider;
    public ParticleSystem tapEffect;
    [Header("Texts")]
    public TextMeshProUGUI currencyTextMesh;
    public TextMeshProUGUI currencyProdTextMesh;
    public TextMeshProUGUI multiplicatorTextMesh;
    public TextMeshProUGUI planetProdTextMesh;
}
