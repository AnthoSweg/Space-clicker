﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int satelliteNbr;
    public GameObject satellitePrefab;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < satelliteNbr; i++)
        {
            Instantiate(satellitePrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Instantiate(satellitePrefab);
    }
}
