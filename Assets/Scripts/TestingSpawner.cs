using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSpawner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(aa());
    }

    private IEnumerator aa()
    {
        Team t1 = new Team("paco", Color.red);
        Team t2 = new Team("pepa", Color.green);

        CelestialBody p = FindObjectOfType<CelestialBody>();
        Random r = new Random();
        while (true)
        {
            Unit unit = new Unit(GameManager.HumanPlayer);

            if (Random.value < 0.66)
            {
                if (Random.value < 0.5)
                {
                    unit.SetOwner(GameManager.HumanPlayer);
                }
                else
                {
                    unit.SetOwner(GameManager.HumanPlayer);
                }
            }
            else
            {
                unit.SetOwner(t1);
            }

            p.TroopArrival(unit);
            yield return new WaitForSeconds(2);
        }
    }
}
