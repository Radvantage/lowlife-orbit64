using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactRocket : MonoBehaviour
{
    public GameObject trail;
    private BoxCollider box;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy")
        {
            if (other.tag == "Player")
            {
                GameObject hit = other.transform.gameObject;
                hit.SendMessage("Damage", (int)20, SendMessageOptions.RequireReceiver);
                hit.SendMessage("Push", transform.position, SendMessageOptions.RequireReceiver);
            }
            if (other.tag == "Projectile")
            {
                Destroy(other.transform.gameObject);
            }
            if (other.tag == "Hazard")
            {
                return;
            }
            
            Impact();
        }
    }

    private void Impact()
    {
        Instantiate(trail, transform.position, transform.rotation);
        Destroy(transform.parent.gameObject);
    }
}