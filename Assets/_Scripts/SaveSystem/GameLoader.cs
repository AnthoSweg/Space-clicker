using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    public GameManager gm;

    public void PlayGame()
    {
        //Load Save File
        if (SaveFile.CurrentState.currency == 0)
        {
            NewGame();
        }
        else
        {
            LoadGame();
        }
        gm.gameStarted = true;
    }

    private void NewGame()
    {
        Debug.Log("/////////NEW GAME/////////");
        for (int i = 0; i < gm.allPlanets.Count; i++)
        {
            gm.allPlanets[i].state.planetName = gm.allPlanets[i].planetName;
            SaveFile.CurrentState.planetStates.Add(gm.allPlanets[i].state);
        }
        //Unlcok first planet by default and set its state to happy
        gm.allPlanets[0].state.unlocked = true;
        gm.allPlanets[0].state.hapinessPoint = (int)Hapiness.happy;

        //GameManager.Save();

        gm.Initialize();
    }

    private void LoadGame()
    {
        Debug.Log("/////////LOAD GAME/////////");
        gm.Initialize();
    }
}
