﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    public Unit UnitPrefab;
    
    protected List<PlanetStartState> planets = new List<PlanetStartState>();

    protected void Start()
    {
        List<Team> participatingTeams = new List<Team>();

        planets.ForEach( p => {
            for (int i = 0; i < p.TroopCount; i++) {
                SpawnTroop(p.Team, p.Planet);
                participatingTeams.Add(p.Team);
            }
        });

        GameManager.Instance.AllTeamsPlaying.AddRange(participatingTeams.Distinct());
    }

    private void SpawnTroop(Team t, CelestialBody planet)
    {
        Unit unit = Instantiate(UnitPrefab, planet.transform.position, Quaternion.identity);
        unit.SetOwner(t);
        unit.MoveToCelestialBody(planet);
    }

    protected Team GetTeamFromAllPossibleTeams(string name)
    {
        return GameManager.Instance.AllPossibleTeams.Where(t => t.Name.Equals(name)).FirstOrDefault();
    }

    protected List<PlanetStartState> CreatePlanetStartState(CelestialBody[] celestialBodies, string[] teamInBody, int[] troopsOfTeam)
    {
        List<PlanetStartState> res = new List<PlanetStartState>();
        List<Team> Teams = teamInBody.Select(s => GetTeamFromAllPossibleTeams(s)).ToList();

        for (int i = 0; i < celestialBodies.Length; i++)
        {
            PlanetStartState temp = new PlanetStartState();
            temp.Planet = celestialBodies[i];
            temp.Team = Teams[i];
            temp.TroopCount = troopsOfTeam[i];

            res.Add(temp);
        }

        return res;
    }
}


public struct PlanetStartState {
    public CelestialBody Planet;
    public Team Team;
    public int TroopCount;
}