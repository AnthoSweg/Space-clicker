using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Planet : MonoBehaviour
{
    public PlanetState state;
    public PlanetData data;
    public Transform pTransform;

    private float multiplier;

    public float upgradeCost
    {
        get { return data.baseCost * (Mathf.Pow(data.rateGrowth, state.upgradeOwned)); }
    }
    public float production
    {
        get { return (data.baseProduction * state.upgradeOwned) * multiplier; }
    }

    [Header("Debug")]
    public Hapiness hapiness;
    private float hapinessDecreaseTimer;

    [SerializeField] private SpriteRenderer face;

    public void Initialize(int index)
    {
        pTransform = this.transform;
        //Search for the corresponding planet in the save file, based on the name
        PlanetState ps = GameState.CurrentState.planetStates.Find(x => x.planetName.Equals(data.planetName));
        if (ps != null)
        {
            this.state = ps;
            if (state.unlocked)
                GetHapinessPointsAfterBeingAway();
            else
            {
                face.sprite = GameAssets.Main.lockedPlanet;
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogErrorFormat("Planet {0} not found in the save file", data.planetName);
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.gameStarted)
            return;

        state.hapinessPoint -= Time.deltaTime;
        state.hapinessPoint = Mathf.Clamp(state.hapinessPoint, (int)Hapiness.asleep, (int)Hapiness.maximumJoy);

        GetNewHapinessState();        
    }

    public void BuyUpgrade()
    {
        state.upgradeOwned++;
    }

    public void IncreaseHapiness()
    {
        state.hapinessPoint++;
    }

    public void GetNewHapinessState(float points = -1)
    {
        //By default points = -1 which means we go the normal way
        if (points == -1)
        {
            //Get new hapiness state
            switch (hapiness)
            {
                case Hapiness.asleep:
                    if (state.hapinessPoint >= (int)Hapiness.bored)
                    {
                        hapiness = Hapiness.bored;
                        UpdateBehaviour();
                    }
                    break;

                case Hapiness.bored:
                    if (state.hapinessPoint >= (int)Hapiness.happy)
                    {
                        hapiness = Hapiness.happy;
                        UpdateBehaviour();
                    }
                    else if (state.hapinessPoint <= (int)Hapiness.asleep)
                    {
                        hapiness = Hapiness.asleep;
                        UpdateBehaviour();
                    }
                    break;

                case Hapiness.happy:
                    if (state.hapinessPoint >= (int)Hapiness.maximumJoy)
                    {
                        hapiness = Hapiness.maximumJoy;
                        UpdateBehaviour();
                    }
                    else if (state.hapinessPoint <= (int)Hapiness.bored)
                    {
                        hapiness = Hapiness.bored;
                        UpdateBehaviour();
                    }
                    break;

                case Hapiness.maximumJoy:
                    if (state.hapinessPoint <= (int)Hapiness.happy)
                    {
                        hapiness = Hapiness.happy;
                        UpdateBehaviour();
                    }
                    break;
                default:
                    break;
            }
        }
        //If we come back from after some times, we wanna restart at some hapiness state, based on the time spent
        else
        {
            if (state.hapinessPoint <= (int)Hapiness.asleep)
                hapiness = Hapiness.asleep;
            if (state.hapinessPoint <= (int)Hapiness.bored)
                hapiness = Hapiness.bored;
            if (state.hapinessPoint <= (int)Hapiness.happy)
                hapiness = Hapiness.happy;
            if (state.hapinessPoint <= (int)Hapiness.maximumJoy)
                hapiness = Hapiness.maximumJoy;
        }

        //set the multiplier
        switch (hapiness)
        {
            case Hapiness.asleep:
                multiplier = GameParams.Main.baseMultiplierPerHapniessLevel[0];
                break;
            case Hapiness.bored:
                multiplier = GameParams.Main.baseMultiplierPerHapniessLevel[1];
                break;
            case Hapiness.happy:
                multiplier = GameParams.Main.baseMultiplierPerHapniessLevel[2];
                break;
            case Hapiness.maximumJoy:
                multiplier = GameParams.Main.baseMultiplierPerHapniessLevel[3];
                break;
            default:
                break;
        }

        //Display the multiplicator value under the hapiness bar
        GameAssets.Main.multiplicatorTextMesh.text = string.Format("x{0}", multiplier);
        GameAssets.Main.multiplicatorTextMesh.fontSize = defaultMultiplicatorTextFontSize * multiplier;
    }

    public static float defaultMultiplicatorTextFontSize;
    void UpdateBehaviour()
    {
        //For now it's just sprite swap but we can play anim here, play audio ect
        switch (hapiness)
        {
            case Hapiness.asleep:
                face.sprite = data.faces[0];
                break;
            case Hapiness.bored:
                face.sprite = data.faces[1];
                break;
            case Hapiness.happy:
                face.sprite = data.faces[2];
                break;
            case Hapiness.maximumJoy:
                face.sprite = data.faces[3];
                break;
            default:
                break;
        }

        //GameManager.Save();
    }

    private void GetHapinessPointsAfterBeingAway()
    {
        TimeSpan timeSpentAway = DateTime.UtcNow - GameState.CurrentState.TimeStampUTC;
        state.hapinessPoint -= Mathf.FloorToInt((float)timeSpentAway.TotalSeconds);
        GetNewHapinessState(state.hapinessPoint);
    }
}

//Planet Data class manage values that will be saved
[System.Serializable]
public class PlanetState
{
    //Used as an ID to find the corresponding planet in the game
    public string planetName ="";
    public bool unlocked = false;
    public int upgradeOwned = 1;
    public float hapinessPoint;
}

[System.Serializable]
public class PlanetData
{
    public string planetName;
    [Header("Planet cost")]
    public float planetCost;
    [Header("First cost of the upgrade")]
    public float baseCost;
    [Header("nbr generated/s")]
    public float baseProduction = 1.67f;
    [Header("Exponential growth")]
    public float rateGrowth = 1.07f;
    [Header("Faces from saddest to happiest")]
    public Sprite[] faces;
}

public enum Hapiness
{
    asleep = 1,
    bored = 10,
    happy = 20,
    maximumJoy = 30
}
