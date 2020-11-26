using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteChecker : MonoBehaviour
{
    public Action OnLevelEnd;
    public Team Winner;
    
    void Update()
    {
        Team winner = null;
        foreach(Team t in GameManager.Instance.AllTeamsPlaying)
        {
            if (t.CurrentTroopCount > 0)
            {
                if (winner != null)
                    return;
                winner = t;
            }
        }

        // If we are here, only one team had more than 0 ppl
        Winner = winner;
        LevelFinished();
    }

    private void LevelFinished()
    {
        OnLevelEnd?.Invoke();
        FindObjectOfType<LevelLoader>().LoadLevel("MainMenu");
    }
}
