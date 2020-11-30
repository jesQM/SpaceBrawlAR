using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWaitDecorator : BTDecoratorNode
{
    private float endTime;

    public BTWaitDecorator(BehaviourTree t, BTNode child, float seconds) : base(t, child)
    {
        this.endTime = Time.time + seconds;
    }

    public override Result Execute()
    {
        if (Time.time >= endTime)
            return Child.Execute();
        return Result.Running;
    }
}
