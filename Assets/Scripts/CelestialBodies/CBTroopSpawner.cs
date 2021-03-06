﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
public class CBTroopSpawner : MonoBehaviour
{
    [Min(0)]
    public int TroopWeight; /// Amount of troops this planet provides
    public float SpawnInterval; /// Every how often a troop is spawned
    public Unit UnitToSpawn;
    public GameObject parent;

    private CelestialBody planet;
    private Coroutine spawningCoroutine;
    private Team teamOwner;

    void Start()
    {
        this.planet = GetComponent<CelestialBody>();

        planet.OnPlanetConquest += (team) => {
            teamOwner = team;
            teamOwner.MaxTroopCount += TroopWeight;
            StartSpawn();
        };
        planet.OnPlanetUnconquest += () => {
            if (teamOwner != null) teamOwner.MaxTroopCount -= TroopWeight;
            teamOwner = null;
            EndSpawn();
        };
    }

    void StartSpawn()
    {
        spawningCoroutine = StartCoroutine(SpawningCoroutine());
    }

    void EndSpawn()
    {
        if(spawningCoroutine != null) StopCoroutine(spawningCoroutine);
    }


    IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(1f);

        while(true)
        {
            SpawnTroop();
            yield return new WaitForSeconds(SpawnInterval);
        }
    }

    private void SpawnTroop()
    {
        if (teamOwner.CurrentTroopCount < teamOwner.MaxTroopCount)
        {
            Unit unit = Instantiate(UnitToSpawn, planet.transform.position, Quaternion.identity, parent.transform);
            unit.SetOwner(teamOwner);
            unit.MoveToCelestialBody(planet);
        }
    }
}
