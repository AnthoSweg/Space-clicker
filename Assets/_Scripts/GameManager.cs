using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int satelliteNbr;
    public GameObject satellitePrefab;
    [SerializeField]private TextMeshProUGUI satelliteNbrTextMesh;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < satelliteNbr; i++)
        {
            SpawnSatellite();
        }
    }

    [SerializeField] private TextMeshProUGUI fpsTextMesh;
    float fpsUpdateDelay = .5f;
    float fpsTime = 0f;
    int FPSCounter = 0;
    private void Update()
    {
        //Display FPS
        if (fpsTextMesh != null)
        {
            FPSCounter++;
            if (fpsTime + fpsUpdateDelay < Time.unscaledTime)
            {
                FPSCounter = Mathf.RoundToInt(1f * FPSCounter / (Time.unscaledTime - fpsTime));
                fpsTextMesh.text = FPSCounter.ToString();

                fpsTime = Time.unscaledTime;
                FPSCounter = 0;
            }
        }
    }

    private int satelliteCount;
    public void SpawnSatellite()
    {
        Instantiate(satellitePrefab);
        satelliteCount++;
        satelliteNbrTextMesh.text = satelliteCount.ToString();
    }
}
