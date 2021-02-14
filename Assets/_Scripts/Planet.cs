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

    private int upgradeOwned;
    private float multiplier;

    public float nextCost
    {
        get { return baseCost * (Mathf.Pow(rateGrowth, upgradeOwned)); }
    }
    public float production
    {
        get { return (baseProduction * upgradeOwned) * multiplier; }
    }

    [HideInInspector] public Hapiness hapiness;
    private int hapinessPoint;
    private float hapinessDecreaseTimer;

    private void LateUpdate()
    {
        //Decrease hapiness level
        hapinessDecreaseTimer -= Time.deltaTime;
        if(hapinessDecreaseTimer == 0)
        {
            hapinessDecreaseTimer = 1f;
            hapinessPoint--;
        }

        hapinessPoint = Mathf.Clamp(hapinessPoint, (int)Hapiness.asleep, (int)Hapiness.maximumJoy);

        GetNewHapinessState();
    }

    public void BuyUpgrade()
    {
        //If currency is enough
        //Consume currency
        upgradeOwned++;
        //Update UI
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
                }
                break;

            case Hapiness.bored:
                if (hapinessPoint >= (int)Hapiness.happy)
                {
                    hapiness = Hapiness.happy;
                }
                else if(hapinessPoint <= (int)Hapiness.asleep)
                {
                    hapiness = Hapiness.asleep;
                }
                break;

            case Hapiness.happy:
                if (hapinessPoint >= (int)Hapiness.maximumJoy)
                {
                    hapiness = Hapiness.maximumJoy;
                }
                else if (hapinessPoint <= (int)Hapiness.bored)
                {
                    hapiness = Hapiness.bored;
                }
                break;

            case Hapiness.maximumJoy:
                if (hapinessPoint <= (int)Hapiness.happy)
                {
                    hapiness = Hapiness.happy;
                }
                break;
            default:
                break;
        }

        //set the multiplier
        multiplier = (int)hapiness / 100f;
    }
}

public enum Hapiness
{
    asleep = 0,
    bored = 35,
    happy = 100,
    maximumJoy = 200
}
