public interface ITroop
{
    Team GetOwner();
    void SetOwner(Team owner);
    float GetHealth();
    float GetMaxHealth();

    void Kill();
    bool IsOnPlanet();
    void SetCurrentCelestialBody(CelestialBody planet);
    CelestialBody GetCurrentCelestialBody();

    void MoveToCelestialBody(CelestialBody target);

}

public class Unit : ITroop
{
    private Team owner;
    private float maxHealth;
    private float health;

    private bool isOnPlanet;
    private CelestialBody currentPlanet;

    public Unit(Team owner)
    {
        SetOwner(owner);
        maxHealth = 100;
        health = this.maxHealth;
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

    public bool IsOnPlanet()
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
        //Destroy(this.gameObject);
    }

    public void MoveToCelestialBody(CelestialBody target)
    {
        target.TroopArrival(this);
        SetCurrentCelestialBody(target);
    }

    public void SetOwner(Team owner)
    {
        this.owner = owner;
    }
    #endregion
}