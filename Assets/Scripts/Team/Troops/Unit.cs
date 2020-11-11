﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Unit : MonoBehaviour, ITroop
{
    public Action<CelestialBody> OnCelestialBodyArrival;
    public Action<CelestialBody> OnCelestialBodyMoveTo;
    public Action OnKilled;


    private Team owner;
    [Min(0)]
    public float maxHealth;
    private float health;

    private bool isOnPlanet;
    
    private CelestialBody currentPlanet;
    private CelestialBody targetPlanet;
    
    void Start()
    {
        health = this.maxHealth;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isOnPlanet) return;

        CelestialBody cb = other.gameObject.GetComponent<CelestialBody>();
        if (cb != null && cb.Equals(targetPlanet)) {
            cb.TroopArrival(this);
            SetCurrentCelestialBody(cb);
        }
    }

    #region ITroop  

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public Team GetOwner()
    {
        return owner;
    }

    public bool IsOnCelestialBody()
    {
        return isOnPlanet;
    }

    public CelestialBody GetCurrentCelestialBody()
    {
        return currentPlanet;
    }

    public void SetCurrentCelestialBody(CelestialBody planet)
    {
        OnCelestialBodyArrival?.Invoke(planet);
        this.currentPlanet = planet;
        isOnPlanet = true;
    }

    public void Kill()
    {
        OnKilled?.Invoke();
        if (isOnPlanet) currentPlanet.TroopGotKilled(this);
        Destroy(this.gameObject);
    }

    public void MoveToCelestialBody(CelestialBody target)
    {
        OnCelestialBodyMoveTo?.Invoke(target);
        targetPlanet = target;
        isOnPlanet = false;
    }

    public void SetOwner(Team owner)
    {
        this.owner = owner;
    }

    public float GetDamage()
    {
        return 10f;
    }

    public CelestialBody GetTargetCelestialBody()
    {
        if (isOnPlanet)
            return null;
        return targetPlanet;
    }
    #endregion
}