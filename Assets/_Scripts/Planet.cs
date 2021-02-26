using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Planet : MonoBehaviour
{
    public GameObject eyes;
    
    [Header("Debug")]
    public PlanetState state;
    public PlanetData data;
    public Transform pTransform;

    private Animator animator;

    [HideInInspector] public float multiplier;

    public float upgradeCost
    {
        get { return data.baseCost * (Mathf.Pow(data.rateGrowth, state.upgradeOwned)); }
    }
    public float acutalProduction
    {
        get { return (data.baseProduction * state.upgradeOwned) * multiplier; }
    }

    public float normalProduction
    {
        get { return data.baseProduction * state.upgradeOwned; }
    }
    public float nextUpgradeProduction
    {
        get
        {
            return (data.baseProduction * (state.upgradeOwned + 1)) - normalProduction;
        }
    }

    [Header("Debug")]
    public Hapiness hapiness;

    [SerializeField] private SpriteRenderer face;

    public void Initialize(int index)
    {
        pTransform = this.transform;
        animator = GetComponentInChildren<Animator>();
        //Search for the corresponding planet in the save file, based on the name
        PlanetState ps = GameState.CurrentState.planetStates.Find(x => x.planetName.Equals(data.planetName));
        if (ps != null)
        {
            this.state = ps;
            if (state.unlocked)
            {
                GetHapinessPointsAfterBeingAway();
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogErrorFormat("Planet {0} not found in the save file", data.planetName);
        }
    }

    private Hapiness lastFrameHapiness;
    private float timer;
    private void LateUpdate()
    {
        if (!GameManager.gameStarted)
            return;

        state.hapinessPoint -= Time.deltaTime * GameParams.Main.hapinessDecreaseSpeed;
        state.hapinessPoint = Mathf.Clamp(state.hapinessPoint, 0, GameParams.Main.happinessPointsNeededPerLevel[3]);

        if(!focused)
        {
            timer -= Time.deltaTime;
            if(timer<=0)
            {
                timer = UnityEngine.Random.Range(1, 5);
                SetLookAtTarget(new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)));
            }
        }

        LookAtPoint();

        GetNewHapinessState();

        lastFrameHapiness = hapiness;
    }

    public void BuyUpgrade()
    {
        if (!state.unlocked)
            UnlockPlanet();
        state.upgradeOwned++;
    }

    void UnlockPlanet()
    {
        state.unlocked = true;
        GameManager.ownedPlanets.Add(this);
        this.gameObject.SetActive(true);
        GetNewHapinessState(true);
    }

    public void IncreaseHapiness()
    {
        state.hapinessPoint++;
    }

    public float maxOffset;
    private Vector2 lookPoint;
    public float lookSpeed;
    private void LookAtPoint()
    {
        eyes.transform.localPosition = Vector2.Lerp(eyes.transform.localPosition, lookPoint,  lookSpeed);
    }

    public void SetLookAtTarget(Vector3 touchPoint)
    {
        if (touchPoint == Vector3.zero)
            //    lookPoint = eyes.transform.localPosition.

            lookPoint = Vector2.zero;
        else
            lookPoint = (touchPoint - eyes.transform.position).normalized * maxOffset;
    }

    private bool focused = false;
    public void Focus(bool focus)
    {
        focused = focus;
    }

    public void GetNewHapinessState(bool forceUpdate = false)
    {
        if (forceUpdate == false)
        {
            switch (hapiness)
            {
                case Hapiness.asleep:
                    if (state.hapinessPoint >= GameParams.Main.happinessPointsNeededPerLevel[1])
                    {
                        hapiness = Hapiness.bored;
                    }
                    break;

                case Hapiness.bored:
                    if (state.hapinessPoint >= GameParams.Main.happinessPointsNeededPerLevel[2])
                    {
                        hapiness = Hapiness.happy;
                    }
                    else if (state.hapinessPoint <= GameParams.Main.happinessPointsNeededPerLevel[0])
                    {
                        hapiness = Hapiness.asleep;
                    }
                    break;

                case Hapiness.happy:
                    if (state.hapinessPoint >= GameParams.Main.happinessPointsNeededPerLevel[3])
                    {
                        hapiness = Hapiness.maximumJoy;
                    }
                    else if (state.hapinessPoint <= GameParams.Main.happinessPointsNeededPerLevel[1])
                    {
                        hapiness = Hapiness.bored;
                    }
                    break;

                case Hapiness.maximumJoy:
                    if (state.hapinessPoint <= GameParams.Main.happinessPointsNeededPerLevel[2])
                    {
                        hapiness = Hapiness.happy;
                    }
                    break;
                default:
                    break;
            }
            if (hapiness != lastFrameHapiness)
                UpdateBehaviour();
        }
        else
        {
            if (state.hapinessPoint <= GameParams.Main.happinessPointsNeededPerLevel[0])
            {
                hapiness = Hapiness.asleep;
            }
            else if (state.hapinessPoint <= GameParams.Main.happinessPointsNeededPerLevel[1])
            {
                hapiness = Hapiness.bored;
            }
            else if (state.hapinessPoint <= GameParams.Main.happinessPointsNeededPerLevel[2])
            {
                hapiness = Hapiness.happy;
            }
            else if (state.hapinessPoint <= GameParams.Main.happinessPointsNeededPerLevel[3])
            {
                hapiness = Hapiness.maximumJoy;
            }
            UpdateBehaviour();
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
    }

    public static float defaultMultiplicatorTextFontSize;
    void UpdateBehaviour()
    {
        switch (hapiness)
        {
            case Hapiness.asleep:
                //face.sprite = data.faces[0];
                animator.SetFloat("hapiness", 1);
                break;
            case Hapiness.bored:
                //face.sprite = data.faces[1];
                animator.SetFloat("hapiness", 2);
                break;
            case Hapiness.happy:
                //face.sprite = data.faces[2];
                animator.SetFloat("hapiness", 3);
                break;
            case Hapiness.maximumJoy:
                //face.sprite = data.faces[3];
                animator.SetFloat("hapiness", 4);
                break;
            default:
                break;
        }
    }

    private void GetHapinessPointsAfterBeingAway()
    {
        TimeSpan timeSpentAway = DateTime.UtcNow - GameState.CurrentState.TimeStampUTC;
        state.hapinessPoint -= Mathf.FloorToInt((float)timeSpentAway.TotalSeconds);
        GetNewHapinessState(true);
    }
}

//Planet Data class manage values that will be saved
[System.Serializable]
public class PlanetState
{
    //Used as an ID to find the corresponding planet in the game
    public string planetName = "";
    public bool unlocked = false;
    public int upgradeOwned = 1;
    public float hapinessPoint;
}

[System.Serializable]
public class PlanetData
{
    public string planetName;
    [Header("First cost of the upgrade")]
    public float baseCost;
    [Header("nbr generated/s")]
    public float baseProduction = 1.67f;
    [Header("Exponential growth")]
    public float rateGrowth = 1.07f;
    [Header("Faces from saddest to happiest")]
    public Sprite[] faces;
    public Sprite shopIcon;
}

public enum Hapiness
{
    asleep,
    bored,
    happy,
    maximumJoy
}
