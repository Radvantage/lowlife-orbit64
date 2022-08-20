using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidImpact : MonoBehaviour
{
    public GameObject trail;
    public GameObject[] items;

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(trail, transform.position, transform.rotation);

        //Asteroid Item Container, ~4% chance of item drop
        float randomChance = Random.Range(0, 100);
        if (randomChance > 95)
        {
            Instantiate(items[0], transform.position, items[0].transform.rotation);
        }

        Destroy(transform.parent.gameObject);
    }
}