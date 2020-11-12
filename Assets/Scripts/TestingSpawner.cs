using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSpawner : MonoBehaviour
{
    public Unit troop;

    void Start()
    {
        //StartCoroutine(aa());
        
        CelestialBody p = FindObjectOfType<CelestialBody>();
        for (int i = 0; i < 1000; i++)
        {
            Unit unit = Instantiate(troop, new Vector3(3, 3, 3), Quaternion.identity);
            unit.SetOwner(GameManager.HumanPlayer);
            unit.MoveToCelestialBody(p);
        }
        Team t2 = new Team("pepa", Color.green);
        for (int i = 0; i < 1000; i++)
        {
            Unit unit = Instantiate(troop, new Vector3(10, 10, 10), Quaternion.identity);
            unit.SetOwner(t2);
            unit.MoveToCelestialBody(p);
        }
        /*
        Team t3 = new Team("manolo", Color.magenta);
        for (int i = 0; i < 1000; i++)
        {
            Unit unit = Instantiate(troop, new Vector3(10, 10, 10), Quaternion.identity);
            unit.SetOwner(t3);
            unit.MoveToCelestialBody(p);
        }

        Team t4 = new Team("aaaa", Color.yellow);
        for (int i = 0; i < 1000; i++)
        {
            Unit unit = Instantiate(troop, new Vector3(10, 10, 10), Quaternion.identity);
            unit.SetOwner(t4);
            unit.MoveToCelestialBody(p);
        }*/
    }

    /*private IEnumerator aa()
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
            unit.SetCurrentCelestialBody(p);
            yield return new WaitForSeconds(2);
        }
    }*/
}
