using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public abstract class BehaviourTree : MonoBehaviour
{
    public BTNode Root;
    public Dictionary< string, object > Blackboard;

    Coroutine behaviour;

    void Start()
    {
        Blackboard = new Dictionary<string, object>();
        BlackboardConfiguration();
        NodesConfiguration();
        behaviour = StartCoroutine(RunBehaviour());
    }

    protected abstract void BlackboardConfiguration();
    protected abstract void NodesConfiguration();

    IEnumerator RunBehaviour()
    {
        BTNode.Result res = Root.Execute();
        while (true) // res == BTNode.Result.Running
        {
            res = Root.Execute();
            yield return null;
        }
    }
}