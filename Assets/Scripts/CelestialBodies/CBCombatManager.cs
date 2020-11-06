using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
public class CBCombatManager : MonoBehaviour
{
    public static float AttackInterval = 2f;
    // Time penalization for just arrived team before being able to attack
    public static float DelayBeforeStartAttackingMs = 1000f;

    private Dictionary<Team, float> DamageAccumulatedPerPlayer = new Dictionary<Team, float>();

    private CelestialBody Planet;
    
    void Start()
    {
        Planet = GetComponent<CelestialBody>();

        Planet.OnNewTeamArrival += (team) => {
            DamageAccumulatedPerPlayer.Add(team, 0);
        };
        Planet.OnTeamLeave += (team) => {
            DamageAccumulatedPerPlayer.Remove(team);
            if (Planet.IsAtPeace && Planet.CurrentTeamsInPlanet.Count > 0) {
                DamageAccumulatedPerPlayer[Planet.CurrentTeamsInPlanet[0]] = 0; // Reset accumulation of damage
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Planet.IsAtPeace) return;

        foreach (KeyValuePair<Team, float> pair in DamageAccumulatedPerPlayer.ToArray()) {
            if (!DamageAccumulatedPerPlayer.ContainsKey(pair.Key)) continue; // Team has been killed

            float totalDamage = pair.Value + GetDamageDealt(pair.Key);
            //Debug.Log("Total Damage of " + pair.Key + " is " + totalDamage);

            DamageAccumulatedPerPlayer[pair.Key] = ApplyDamageFromTeam(pair.Key, totalDamage);
        }
    }

    private float ApplyDamageFromTeam(Team team, float damage)
    {
        List<Team> possibleTargets = new List<Team>(Planet.CurrentTeamsInPlanet);
        possibleTargets.Remove(team);
        Team targetTeam = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];

        List<ITroop> units = Planet.Troops[targetTeam];
        ITroop unitReceiving = units[0];
        float health = unitReceiving.GetHealth();

        while ( damage >= health) {
            Debug.Log("Damage from " + team + " to " + targetTeam);
            damage -= health;
            unitReceiving.Kill();

            // Update vars
            if (units.Count <= 1) break; // If that was the last unit => skip

            units = Planet.Troops[targetTeam];
            unitReceiving = units[0];
            health = unitReceiving.GetHealth();
        }

        return damage;
    }

    // TODO, upgrade implementation
    private float GetDamageDealt(Team team)
    {
        List<ITroop> units = Planet.Troops[team];
        return units.Count * units[0].GetDamage() * Time.deltaTime;
    }
}