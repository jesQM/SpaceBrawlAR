using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAttackCelestialBody : BTNode
{
    private List<CelestialBody> celestialBodies;
    private Team team;

    public BTAttackCelestialBody(BehaviourTree t) : base(t)
    {
        object temp = null;
        try {
            temp = t.Blackboard["CelestialBodies"];
        } catch (KeyNotFoundException e) {
            temp = UnityEngine.GameObject.FindObjectsOfType<CelestialBody>().ToList();
            t.Blackboard.Add("CelestialBodies", temp);
        }

        team = t.Blackboard["Team"] as Team;

        celestialBodies = temp as List<CelestialBody>;
    }

    public override Result Execute()
    {
        var s = celestialBodies.Where(b => !team.Equals(b.Owner));
        if (s.Count() == 0) return Result.Failure; // Nowhere to attack

        int startIdx = Random.Range(0, s.Count()-1);
        int currentIdx = startIdx;
        bool stop = false;

        CelestialBody target = null;
        while (!stop) // Select a viable target
        {
            target = s.ElementAt(currentIdx);
            int enemyCount = target.Troops.ToList()
                .Where(pair => !pair.Key.Equals(team))
                .Aggregate(0, (int add, KeyValuePair<Team, List<ITroop>> pair) => pair.Value.Count + add);
            
            if (enemyCount < team.CurrentTroopCount/2f) break;

            currentIdx++;
            if (currentIdx >= s.Count()) currentIdx = 0;
            if (currentIdx == startIdx) return Result.Failure;
        }

        Tree.Blackboard["CelestialBodyTarget"] = target;
        return Result.Success;
    }
}

public class BTDefendCelestialBody : BTNode
{
    private List<CelestialBody> celestialBodies;
    private Team team;

    public BTDefendCelestialBody(BehaviourTree t) : base(t)
    {
        object temp = null;
        try
        {
            temp = t.Blackboard["CelestialBodies"];
        }
        catch (KeyNotFoundException e)
        {
            temp = UnityEngine.GameObject.FindObjectsOfType<CelestialBody>().ToList();
            t.Blackboard.Add("CelestialBodies", temp);
        }

        team = t.Blackboard["Team"] as Team;

        celestialBodies = temp as List<CelestialBody>;
    }

    public override Result Execute()
    {
        var target = celestialBodies.Where(b => team.Equals(b.Owner) && !b.IsAtPeace).FirstOrDefault();
        if (target == null) return Result.Failure; // Nowhere to defend

        Tree.Blackboard["CelestialBodyTarget"] = target;
        return Result.Success;
    }
}
