using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitMovementManager : MonoBehaviour
{
    Unit Unit;
    internal IMovementStrategy Strategy;
    
    void Start()
    {
        Unit = GetComponent<Unit>();

        // Events that cause the strategy to change
        Unit.OnCelestialBodyArrival += cb => this.Strategy = Strategy.UnitArrivedAtCelestialBody(cb);
        Unit.OnCelestialBodyMoveTo += cb => this.Strategy = Strategy.UnitMoveToCelestialBody(cb);

        if (Unit.IsOnCelestialBody())
            Strategy = new MovementStrategyOrbitingCelestialBody(Unit, Unit.GetCurrentCelestialBody());
        else
            Strategy = new MovementStrategyMoveToCelestialBody(Unit, Unit.GetTargetCelestialBody());
    }
    
    void Update()
    {
        Strategy.Update();
    }
}


internal interface IMovementStrategy
{
    void Update();
    IMovementStrategy UnitArrivedAtCelestialBody(CelestialBody cb);
    IMovementStrategy UnitMoveToCelestialBody(CelestialBody cb);
}

internal abstract class AbstractMovementStrategy : IMovementStrategy
{
    protected Unit Unit;

    public AbstractMovementStrategy(Unit unit)
    {
        this.Unit = unit;
    }

    public abstract IMovementStrategy UnitArrivedAtCelestialBody(CelestialBody cb);
    public abstract IMovementStrategy UnitMoveToCelestialBody(CelestialBody cb);
    public abstract void Update();
}

internal class MovementStrategyMoveToCelestialBody : AbstractMovementStrategy
{
    private float movementSpeed = 1.5f;

    private CelestialBody Cb;

    public MovementStrategyMoveToCelestialBody(Unit unit, CelestialBody movetToBody) : base(unit)
    {
        this.Cb = movetToBody;
    }

    public override IMovementStrategy UnitArrivedAtCelestialBody(CelestialBody cb)
    {
        return new MovementStrategyOrbitingCelestialBody(Unit, cb);
    }

    public override IMovementStrategy UnitMoveToCelestialBody(CelestialBody cb)
    {
        throw new System.Exception("Unit is already moving");
    }

    public override void Update()
    {
        Vector3 targetPosition = Cb.transform.position;
        Vector3 direction = (targetPosition - Unit.transform.position).normalized;

        Unit.transform.position += direction * movementSpeed * Time.deltaTime;
    }
}

internal class MovementStrategyOrbitingCelestialBody : AbstractMovementStrategy
{
    public float rotationOffset { get; private set; } = 0;
    public float timeStartRotation { get; private set; } = 0;

    public float rotationScale { get; private set; } = 1f;
    public float rotationSpeed { get; private set; } = 0.5f;
    public float movementSpeed { get; private set; } = 1.5f;

    public Vector3 scale { get; private set; }

    private CelestialBody Cb;
    private IMovementStrategyOrbitingCelestialBody strategy;

    public MovementStrategyOrbitingCelestialBody(Unit unit, CelestialBody orbitingBody) : base(unit)
    {
        this.Cb = orbitingBody;
        this.scale = Cb.transform.localScale;
        this.rotationOffset = UnityEngine.Random.Range(0, 2*Mathf.PI);
        this.timeStartRotation = Time.time;

        Unit.GetComponent<Collider>().enabled = false;

        strategy = new MovementStrategyOrbitingCelestialBody_PrepareOrbit(Unit, Cb, this);
    }

    public override IMovementStrategy UnitArrivedAtCelestialBody(CelestialBody cb)
    {
        throw new System.Exception("Unit is already at a planet");
    }

    public override IMovementStrategy UnitMoveToCelestialBody(CelestialBody cb)
    {
        Unit.GetComponent<Collider>().enabled = true;
        return new MovementStrategyMoveToCelestialBody(this.Unit, cb);
    }

    public override void Update()
    {
        strategy.Update();
    }

    internal void DesiredOrbitAchieved()
    {
        strategy = new MovementStrategyOrbitingCelestialBody_Orbit(Unit, Cb, this);
    }
}

// ==============================================================
//      Strategy of MovementStrategyOrbitingCelestialBody
// ==============================================================
internal interface IMovementStrategyOrbitingCelestialBody
{
    void Update();
}

internal class MovementStrategyOrbitingCelestialBody_Orbit : IMovementStrategyOrbitingCelestialBody
{
    private CelestialBody Cb;
    private Unit Unit;
    private MovementStrategyOrbitingCelestialBody Manager;

    public MovementStrategyOrbitingCelestialBody_Orbit(Unit unit, CelestialBody orbitingBody, MovementStrategyOrbitingCelestialBody manager)
    {
        this.Cb = orbitingBody;
        this.Unit = unit;
        this.Manager = manager;
    }

    public void Update()
    {
        Vector3 scale = Manager.scale;
        float time = (Time.time - Manager.timeStartRotation) * Manager.rotationSpeed;
        Vector3 position = new Vector3(Mathf.Sin(time + Manager.rotationOffset) * scale.x, 0, Mathf.Cos(time + Manager.rotationOffset) * scale.z) * Manager.rotationScale;
        position += Cb.transform.position;
        Unit.transform.position = position;
    }
}

internal class MovementStrategyOrbitingCelestialBody_PrepareOrbit : IMovementStrategyOrbitingCelestialBody
{
    private CelestialBody Cb;
    private Unit Unit;
    private MovementStrategyOrbitingCelestialBody Manager;

    public MovementStrategyOrbitingCelestialBody_PrepareOrbit(Unit unit, CelestialBody orbitingBody, MovementStrategyOrbitingCelestialBody manager)
    {
        this.Cb = orbitingBody;
        this.Unit = unit;
        this.Manager = manager;
    }

    public void Update()
    {
        Vector3 scale = Manager.scale;
        Vector3 targetPosition = new Vector3(Mathf.Sin(Manager.rotationOffset) * scale.x, 0, Mathf.Cos(Manager.rotationOffset) * scale.z) * Manager.rotationScale;
        targetPosition += Cb.transform.position;
        Vector3 direction = (targetPosition - Unit.transform.position).normalized;

        this.Unit.transform.position += direction * Manager.movementSpeed * Time.deltaTime;
        if ((targetPosition - Unit.transform.position).sqrMagnitude < 0.01)
        {
            Manager.DesiredOrbitAchieved();
        }

    }
}