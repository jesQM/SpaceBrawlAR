using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTCompositeNode
{
    private int currentNode = 0;

    public BTSelector(BehaviourTree t, List<BTNode> children) : base(t, children)
    {
    }
    public BTSelector(BehaviourTree t, BTNode[] children) : base(t, children)
    {
    }

    public override Result Execute()
    {
        if (currentNode < Children.Count)
        {
            Result res = Children[currentNode].Execute();

            switch (res)
            {
                case Result.Running:
                    return res;
                case Result.Failure:
                    currentNode++;
                    if (currentNode < Children.Count)
                        return Result.Running;
                    currentNode = 0;
                    return Result.Failure;
                default:
                    currentNode = 0;
                    return Result.Success;
            }
        }

        return Result.Success;
    }
}
