using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
public class CBConquestManager : MonoBehaviour
{
    private CelestialBody planet;
    private ICelestialBodyConquestStrategy strategy;

    void Start()
    {
        planet = GetComponent<CelestialBody>();

        Action<Team> eventToDo = (team) => {
            strategy = strategy.PlanetStatusChange();
        };

        planet.OnNewTeamArrival += eventToDo;
        planet.OnTeamLeave += eventToDo;
        planet.OnPlanetConquest += (team) => { strategy = strategy.PlanetConquestChange(); };
        planet.OnPlanetUnconquest += () => { strategy = strategy.PlanetConquestChange(); };

        strategy = new PlanetConquestStrategyIdle(planet);
        strategy = strategy.PlanetStatusChange();
    }

    void Update()
    {
        strategy.Update();
    }
}


internal interface ICelestialBodyConquestStrategy
{
    ICelestialBodyConquestStrategy PlanetStatusChange();
    ICelestialBodyConquestStrategy PlanetConquestChange();
    void Update();
}

internal class PlanetConquestStrategyIdle : ICelestialBodyConquestStrategy
{
    private CelestialBody Planet;

    public PlanetConquestStrategyIdle(CelestialBody planet)
    {
        this.Planet = planet;
    }

    public ICelestialBodyConquestStrategy PlanetConquestChange()
    {
        throw new NotSupportedException("Conquerin/Unconquering cannot happen when idle");
        /*
        HOW?
        - We are idle yet a conquering/unconquering took place
        - Must be a bug
        */
    }

    public ICelestialBodyConquestStrategy PlanetStatusChange()
    {
        Team currentTeamInPlanet = null;

        if (Planet.IsAtPeace)
        { // Peace  => is the planet of the team in the planet?

            if (Planet.CurrentTeamsInPlanet.Count > 0) currentTeamInPlanet = Planet.CurrentTeamsInPlanet[0];
            if (currentTeamInPlanet == null) return this; // No team in planet => nothing to do

            if (Planet.Owner != null && Planet.Owner.Equals(currentTeamInPlanet) )
            {
                // Team in planet is owner =>
                //              if % is not 100 => Conquer
                //              if % is 100 => Idle
                if (Planet.ConquestPercentage.percentage == 100f)
                    return this;
                return new PlanetConquestStrategyConquer(Planet);

            } else {
                // Team in planet is not owner =>
                //          Is Progress so far ous? =>
                //              Yes => Conquer
                //              No => reduce conquer % => Unconquer
                if (currentTeamInPlanet.Equals(Planet.ConquestPercentage.team))
                    return new PlanetConquestStrategyConquer(Planet);
                return new PlanetConquestStrategyUnconquer(Planet);

            }
        }
        // War => keep idle, cannot conquer / deconquer

        return this;
    }

    public void Update()
    {
        return;
    }
}

internal abstract class AbstractPlanetConquestStrategy : ICelestialBodyConquestStrategy
{
    public static float ConqueringSpeed = 100f;
    protected CelestialBody Planet;

    public AbstractPlanetConquestStrategy(CelestialBody planet)
    {
        this.Planet = planet;
    }

    public abstract ICelestialBodyConquestStrategy PlanetConquestChange();

    public abstract ICelestialBodyConquestStrategy PlanetStatusChange();

    public virtual void Update()
    {
        float incrementInConquest = IncrementInConquestOperation();
        Planet.IncrementConquest(Planet.ConquestPercentage.team, incrementInConquest);
    }

    protected virtual float IncrementInConquestOperation()
    {
        return Time.deltaTime * ConqueringSpeed * GetSpeedDependingOnTroopCount(GetTroopCount());
    }

    private float GetSpeedDependingOnTroopCount(int troopCount)
    {
        int maxTroop = 80;

        float percentage = troopCount / maxTroop;
        return Mathf.Lerp(0.1f, ConqueringSpeed, percentage);
    }

    private int GetTroopCount()
    {
        // There should be one team => We can ask for it with no problem
        List<ITroop> troops = Planet.Troops[Planet.CurrentTeamsInPlanet[0]];
        return troops.Count;
    }
}

internal class PlanetConquestStrategyConquer : AbstractPlanetConquestStrategy
{
    public PlanetConquestStrategyConquer(CelestialBody planet) : base(planet)
    {
    }

    public override ICelestialBodyConquestStrategy PlanetConquestChange()
    {
        // A conquest/unconquest event while conquering => 
        //      Must be the planet getting conquered =>
        //          Go idle
        return new PlanetConquestStrategyIdle(Planet);
    }
    
    public override ICelestialBodyConquestStrategy PlanetStatusChange()
    {
        if (!Planet.IsAtPeace)
        { // War broke out
            return new PlanetConquestStrategyIdle(Planet);
        }

        if (Planet.CurrentTeamsInPlanet.Count == 0) // Only team in planet left => stop
            return new PlanetConquestStrategyIdle(Planet);
        return this;
    }

    public override void Update()
    {
        float incrementInConquest = IncrementInConquestOperation();
        Planet.IncrementConquest(Planet.CurrentTeamsInPlanet[0], incrementInConquest);
    }
}

internal class PlanetConquestStrategyUnconquer : AbstractPlanetConquestStrategy
{
    public PlanetConquestStrategyUnconquer(CelestialBody planet) : base(planet)
    {
    }

    public override ICelestialBodyConquestStrategy PlanetConquestChange()
    {
        // A conquest/unconquest event while unconquering => 
        //      Must be the planet getting unconquered =>
        //          Start Conquering
        return new PlanetConquestStrategyConquer(Planet);
    }

    public override ICelestialBodyConquestStrategy PlanetStatusChange()
    {
        if (!Planet.IsAtPeace)
        { // War broke out
            return new PlanetConquestStrategyIdle(Planet);
        }
        if (Planet.CurrentTeamsInPlanet.Count == 0) // Only team in planet left => stop
            return new PlanetConquestStrategyIdle(Planet);
        return this;
    }

    protected override float IncrementInConquestOperation()
    {
        return base.IncrementInConquestOperation() * -1;
    }
}