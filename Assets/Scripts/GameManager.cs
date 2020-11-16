using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
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

    public Team HumanPlayer = new Team("player", Color.blue);
    public List<Team> AllTeams = new List<Team>();

    public List<CelestialBody> CelestialBodiesSelectedByHumanPlayer = new List<CelestialBody>();


    private GameManager()
    {
        AllTeams.Add(HumanPlayer);
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