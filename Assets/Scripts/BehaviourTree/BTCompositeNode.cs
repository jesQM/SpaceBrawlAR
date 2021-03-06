﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCompositeNode : BTNode
{
    public List<BTNode> Children { get; private set; }

    public BTCompositeNode(BehaviourTree t, List<BTNode> children) : base(t)
    {
        this.Children = children;
    }

    public BTCompositeNode(BehaviourTree t, BTNode[] children) : this(t, children.ToList())
    {
    }
}
