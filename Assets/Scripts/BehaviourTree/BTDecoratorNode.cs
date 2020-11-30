using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDecoratorNode : BTNode
{
    public BTNode Child { get; private set; }

    public BTDecoratorNode(BehaviourTree t, BTNode child) : base(t)
    {
        this.Child = child;
    }

}
