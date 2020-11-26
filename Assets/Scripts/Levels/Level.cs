using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : LevelStarter
{
    public CelestialBody[] CelestialBodies;
    public string[] TeamInBody;
    public int[] TroopsOfTeam;

    void Awake() {
        this.planets.AddRange(CreatePlanetStartState(CelestialBodies, TeamInBody, TroopsOfTeam));
    }
}
