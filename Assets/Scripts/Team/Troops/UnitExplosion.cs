using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Unit))]
public class UnitExplosion : MonoBehaviour
{
    //public VisualEffect explosionEffect;
    public GameObject explosionEffect;

    Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        unit.OnKilled += () => CreateExplosion();
    }

    void CreateExplosion()
    {
        GameObject go = Instantiate(explosionEffect, unit.transform.position, unit.transform.rotation);
        go.transform.localScale = unit.transform.localScale * 2;
        go.GetComponent<VisualEffect>().SendEvent("OnExplode");
        Destroy(go, 2f);
    }
}
