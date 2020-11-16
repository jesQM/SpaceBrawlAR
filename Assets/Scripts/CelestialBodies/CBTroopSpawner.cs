using System;
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


    private CelestialBody planet;
    private Coroutine spawningCoroutine;
    private Team teamOwner;

    void Start()
    {
        this.planet = GetComponent<CelestialBody>();

        planet.OnPlanetConquest += (team) => {
            teamOwner = team;
            StartSpawn();
        };
        planet.OnPlanetUnconquest += () => {
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
        Unit unit = Instantiate(UnitToSpawn, planet.transform.position, Quaternion.identity);
        unit.SetOwner(teamOwner);
        unit.MoveToCelestialBody(planet);
    }
}
