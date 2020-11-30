using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class BehaviourTree : MonoBehaviour
{
    public BTNode Root;
    public Dictionary< string, object > Blackboard;

    Coroutine behaviour;

    void Start()
    {
        Blackboard = new Dictionary<string, object>();
        behaviour = StartCoroutine(RunBehaviour());
    }

    public T CastObject<T>(object input)
    {
        return (T)input;
    }

    public T ConvertObject<T>(object input)
    {
        return (T)Convert.ChangeType(input, typeof(T));
    }

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