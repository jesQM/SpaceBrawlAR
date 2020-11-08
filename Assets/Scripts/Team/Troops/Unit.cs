using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ITroop
{
    private Team owner;
    [Min(0)]
    public float maxHealth;
    private float health;

    private bool isOnPlanet;
    private float rotationOffset = 0;
    private CelestialBody currentPlanet;
    private CelestialBody targetPlanet;

    private float rotationScale = 1f;
    private float rotationSpeed = 0.5f;
    private float movementSpeed = 1.5f;
    
    void Start()
    {
        health = this.maxHealth;
        /*owner = GameManager.HumanPlayer;

        CelestialBody p = FindObjectOfType<CelestialBody>();
        MoveToCelestialBody(p);*/
    }

    void Update()
    {
        if (isOnPlanet)
        {
            Vector3 scale = currentPlanet.transform.localScale;
            float time = Time.time * rotationSpeed;
            Vector3 position = new Vector3(Mathf.Sin(time + rotationOffset) * scale.x, 0, Mathf.Cos(time + rotationOffset) * scale.z) * rotationScale;
            position += currentPlanet.transform.position;
            transform.position = position;
        }
        else
        {
            Vector3 targetPosition = targetPlanet.transform.position;
            Vector3 direction = (targetPosition - transform.position).normalized;

            this.transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isOnPlanet) return;

        CelestialBody cb = other.gameObject.GetComponent<CelestialBody>();
        if (cb != null && cb.Equals(targetPlanet)) {
            cb.TroopArrival(this);
            SetCurrentCelestialBody(cb);
            rotationOffset = Random.Range(0, 2*Mathf.PI);
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
        this.currentPlanet = planet;
        isOnPlanet = true;
    }

    public void Kill()
    {
        if (isOnPlanet) currentPlanet.TroopGotKilled(this);
        Destroy(this.gameObject);
    }

    public void MoveToCelestialBody(CelestialBody target)
    {
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
    #endregion
}