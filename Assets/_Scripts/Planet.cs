using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Header("First cost of the upgrade")]
    public float baseCost;
    [Header("nbr generated/s")]
    public float baseProduction = 1.67f;
    [Header("Exponential growth")]
    public float rateGrowth = 1.07f;

    public string planetName;

    private int upgradeOwned;
    private float multiplier;

    public float upgradeCost
    {
        get { return baseCost * (Mathf.Pow(rateGrowth, upgradeOwned)); }
    }
    public float production
    {
        get { return (baseProduction * upgradeOwned) * multiplier; }
    }

    public Hapiness hapiness;
    private int hapinessPoint;
    private float hapinessDecreaseTimer;

    [SerializeField] private SpriteRenderer face;

    private void Start()
    {
        if (upgradeOwned == 0)
            upgradeOwned++;
        hapiness = Hapiness.asleep;
    }

    private void LateUpdate()
    {
        //Decrease hapiness level
        hapinessDecreaseTimer -= Time.deltaTime;
        if(hapinessDecreaseTimer <= 0)
        {
            hapinessDecreaseTimer = 1f;
            hapinessPoint--;
        }

        hapinessPoint = Mathf.Clamp(hapinessPoint, (int)Hapiness.asleep, (int)Hapiness.maximumJoy);

        GetNewHapinessState();
    }

    public void BuyUpgrade()
    {
        upgradeOwned++;
    }

    public void IncreaseHapiness()
    {
        hapinessPoint++;
    }

    private void GetNewHapinessState()
    {
        //Get new hapiness state
        switch (hapiness)
        {
            case Hapiness.asleep:
                if(hapinessPoint>=(int)Hapiness.bored)
                {
                    hapiness = Hapiness.bored;
                    UpdateBehaviour();
                }
                break;

            case Hapiness.bored:
                if (hapinessPoint >= (int)Hapiness.happy)
                {
                    hapiness = Hapiness.happy;
                    UpdateBehaviour();
                }
                else if(hapinessPoint <= (int)Hapiness.asleep)
                {
                    hapiness = Hapiness.asleep;
                    UpdateBehaviour();
                }
                break;

            case Hapiness.happy:
                if (hapinessPoint >= (int)Hapiness.maximumJoy)
                {
                    hapiness = Hapiness.maximumJoy;
                    UpdateBehaviour();
                }
                else if (hapinessPoint <= (int)Hapiness.bored)
                {
                    hapiness = Hapiness.bored;
                    UpdateBehaviour();
                }
                break;

            case Hapiness.maximumJoy:
                if (hapinessPoint <= (int)Hapiness.happy)
                {
                    hapiness = Hapiness.happy;
                    UpdateBehaviour();
                }
                break;
            default:
                break;
        }

        //set the multiplier
        switch (hapiness)
        {
            case Hapiness.asleep:
                multiplier = .1f;
                break;
            case Hapiness.bored:
                multiplier = .85f;
                break;
            case Hapiness.happy:
                multiplier = 1;
                break;
            case Hapiness.maximumJoy:
                multiplier = 1.25f;
                break;
            default:
                break;
        }
    }

    void UpdateBehaviour()
    {
        //For now it's just sprite swap but we can play anim here, play audio ect
        switch (hapiness)
        {
            case Hapiness.asleep:
                face.sprite = GameAssets.Main.faces[0];
                break;
            case Hapiness.bored:
                face.sprite = GameAssets.Main.faces[1];
                break;
            case Hapiness.happy:
                face.sprite = GameAssets.Main.faces[2];
                break;
            case Hapiness.maximumJoy:
                face.sprite = GameAssets.Main.faces[3];
                break;
            default:
                break;
        }
    }
}

public enum Hapiness
{
    asleep = 1,
    bored = 10,
    happy = 20,
    maximumJoy = 30
}
