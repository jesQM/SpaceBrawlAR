using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSendTroopsToCelestialBody : BTNode
{
    private List<CelestialBody> celestialBodies;
    private CelestialBody target;
    private Team team;

    public BTSendTroopsToCelestialBody(BehaviourTree t) : base(t)
    {
    }

    public override Result Execute()
    {
        celestialBodies = Tree.Blackboard["CelestialBodies"] as List<CelestialBody>;
        target = Tree.Blackboard["CelestialBodyTarget"] as CelestialBody;
        team = Tree.Blackboard["Team"] as Team;

        // Send half people on all pacific planets
        celestialBodies.Where(b => team.Equals(b.Owner) && b.IsAtPeace).ToList().ForEach(b => b.SendTroopsToTarget(team, target, 0.5f));

        return Result.Success;
    }
}
