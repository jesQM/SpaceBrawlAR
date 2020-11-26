using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // Sigleton
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
        private set {
            throw new NotSupportedException();
        }
    }


    public List<Team> AllPossibleTeams = new List<Team>();

    public Team HumanPlayer = new Team("Player", Color.blue);
    public List<Team> AllTeamsPlaying = new List<Team>();
    public List<CelestialBody> CelestialBodiesSelectedByHumanPlayer = new List<CelestialBody>();


    private GameManager()
    {
        AllPossibleTeams.Add(HumanPlayer);
        AllPossibleTeams.Add(new Team("Team1", new Color(1, 1, 0) ));
        AllPossibleTeams.Add(new Team("Team2", new Color(1, 0, 1) ));
        AllPossibleTeams.Add(new Team("Team3", new Color(0, 1, 1) ));
    }

    public void SendTroopsToTarget(CelestialBody target)
    {
        foreach (CelestialBody sender in CelestialBodiesSelectedByHumanPlayer)
        {
            sender.SendTroopsToTarget(HumanPlayer, target, 0.5f);
        }
        DeselectAll();
    }

    public void DeselectAll()
    {
        // Create a new list to loop through it without messing up with indexes
        List<CelestialBody> copy = new List<CelestialBody>(CelestialBodiesSelectedByHumanPlayer);
        copy.ForEach(item => item.SetSelected(false));
    }
}