using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //int de 0 à 18 446 744 073 709 551 615
    public ulong currency;
    public List<Planet> planetsOwned = new List<Planet>();

    public Planet focusedPlanet;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            focusedPlanet.IncreaseHapiness();
        }

        float gainedCurrency = 0;
        for(int i =0; i< planetsOwned.Count; i++)
        {
            gainedCurrency += planetsOwned[i].production;
        }
        currency += (ulong)Mathf.Ceil(gainedCurrency);
    }

}