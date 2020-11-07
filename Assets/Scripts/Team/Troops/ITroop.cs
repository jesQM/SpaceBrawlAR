public interface ITroop
{
    Team GetOwner();
    void SetOwner(Team owner);
    float GetHealth();
    float GetMaxHealth();
    float GetDamage();

    void Kill();
    bool IsOnCelestialBody();
    void SetCurrentCelestialBody(CelestialBody planet);
    CelestialBody GetCurrentCelestialBody();

    void MoveToCelestialBody(CelestialBody target);

}