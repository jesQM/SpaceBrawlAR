using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTCompositeNode
{
    private int currentNode = 0;

    public BTSequence(BehaviourTree t, List<BTNode> children) : base(t, children)
    {
    }

    public override Result Execute()
    {
        if (currentNode < Children.Count)
        {
            Result res = Children[currentNode].Execute();

            switch(res)
            {
                case Result.Running:
                    return res;
                case Result.Success:
                    currentNode++;
                    if (currentNode < Children.Count)
                        return Result.Running;
                    currentNode = 0;
                    return Result.Success;
                default:
                    currentNode = 0;
                    return Result.Failure;
            }
        }

        return Result.Success;
    }
}
