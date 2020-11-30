using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBrawlBehaviourTree : BehaviourTree
{
    public Team team;
    
    protected override void BlackboardConfiguration()
    {
        Blackboard.Add("Team", team);
        Blackboard.Add("CelestialBodyTarget", null);
    }

    protected override void NodesConfiguration()
    {
        BTNode root = new BTSelector(this, new BTNode[]{

                new BTSequence(this, new BTNode[] { new BTDefendCelestialBody(this), new BTSendTroopsToCelestialBody(this) }), // Defend Sequence
                new BTSequence(this, new BTNode[] { new BTAttackCelestialBody(this), new BTSendTroopsToCelestialBody(this) }) // Defend Sequence

            }); // End selector
        this.Root = root;
    }
}
