using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWaitLeaf : BTNode
{
    private float endTime;

    public BTWaitLeaf(BehaviourTree t, float seconds) : base(t)
    {
        this.endTime = Time.time + seconds;
    }

    public override Result Execute()
    {
        if (Time.time >= endTime)
            return Result.Success;
        return Result.Running;
    }
}

