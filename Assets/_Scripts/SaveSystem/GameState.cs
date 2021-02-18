using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public GameState()
    {
        this.UpdateTimeStampUTC();
    }

    #region Data Saved

    public Options options;

    public double currency;

    public List<PlanetState> planetStates = new List<PlanetState>();

    [SerializeField]
    private string timeStampUTC = "";
    public DateTime TimeStampUTC
    {
        get
        {
            if (string.IsNullOrEmpty(this.timeStampUTC))
            {
                this.timeStampUTC = DateTime.UtcNow.ToString();
                Debug.LogFormat("Default timestamp has been set: {0}", this.timeStampUTC);
            }
            return DateTime.Parse(this.timeStampUTC);
        }
        set
        {
            this.timeStampUTC = value.ToString(); // yyyyMMddHHmmssfff
        }
    }
    #endregion

    [System.Serializable]
    public class Options
    {
        public string language;
        public float mVolume = 0f;
        public float sVolume = 0f;
    }

    #region methods
    public GameState UpdateTimeStampUTC()
    {
        this.TimeStampUTC = DateTime.UtcNow;
        return this;
    }

    //Returns the most recent save
    public static GameState CurrentState
    {
        get
        {
            return SaveFile.FileContent.GameStates[0];
        }
    }

    public static GameState CreateNewGameState()
    {
        Debug.Log("Will create new save.");
        GameState newSave = new GameState();
        return newSave;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public GameState GetClone()
    {
        return (this.MemberwiseClone() as GameState);
    }
    #endregion
}
