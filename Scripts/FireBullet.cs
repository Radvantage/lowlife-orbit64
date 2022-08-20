using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public float velocity;
    public GameObject impactSmoke;
    private GameObject player;
    void Awake()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * velocity * Time.deltaTime);
        if (Vector3.Distance(player.transform.position, transform.position) > 96f)
        {
            Impact();
        }
    }

    public void Impact()
    {
        Instantiate(impactSmoke, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject hit = other.transform.gameObject;
            hit.SendMessage("Damage", (int)8, SendMessageOptions.RequireReceiver);
        }
        Impact();
    }
}