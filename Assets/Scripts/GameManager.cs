using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public static Team HumanPlayer = new Team("player", Color.blue);
    public static List<Team> AllTeams = new List<Team>();

    public static List<CelestialBody> CelestialBodiesSelectedByHumanPlayer = new List<CelestialBody>();


    public static void SendTroopsToTarget(CelestialBody target)
    {
        foreach (CelestialBody sender in CelestialBodiesSelectedByHumanPlayer)
        {
            sender.SendTroopsToTarget(HumanPlayer, target, 0.5f);
        }
        DeselectAll();
    }

    public static void DeselectAll()
    {
        // Create a new list to loop through it without messing up with indexes
        List<CelestialBody> copy = new List<CelestialBody>(CelestialBodiesSelectedByHumanPlayer);
        copy.ForEach(item => item.SetSelected(false));
    }
}