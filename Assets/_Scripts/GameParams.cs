﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameParams : MonoBehaviour
{
    private static GameParams main;
    public static GameParams Main
    {
        get { if (main == null)
                main = FindObjectOfType<GameParams>();
            return main;
        }
    }

    public List<PlanetData> planetDatas = new List<PlanetData>();
    public float[] happinessPointsNeededPerLevel = new float[4];
    public float[] baseMultiplierPerHapniessLevel = new float[4]; //4 hapiness level exist
    public float baseMaxTimeAwayInHour = 2;

    public Ellipse orbit;
    public float orbitSpeed;
    public float amplitude;

    public float zoomOutLevel = 10;
    public float zoomInLevel = 5;
    public float camYOffset = -1f;

    public float[] hapinessLevelUpgrade = new float[10];
    public float[] hapinessDecreaseSpeedUpgrade = new float[10];
    public float[] multiplicatorUpgrade = new float[10];
}