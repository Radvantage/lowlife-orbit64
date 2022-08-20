using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject shardObj;
    private ParticleSystem shardsFX;

    // Start is called before the first frame update
    void Start()
    {
        shardsFX = GameObject.Find("BoxShards").GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject hit = other.transform.gameObject;
            if (hit.GetComponent<PlayerController>().powerI == 0 && !hit.GetComponent<PlayerController>().selectingPower)
            {
                Instantiate(shardObj, transform.position, transform.rotation);
                hit.SendMessage("ActivatePower");
                Destroy(gameObject);
            }
        }
    }
}
