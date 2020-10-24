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