using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode
{
    public enum Result { Running, Failure, Success};

    public BehaviourTree Tree { get; private set; }

    public BTNode(BehaviourTree t)
    {
        Tree = t;
    }

    public virtual Result Execute()
    {
        return Result.Success;
    }
}
