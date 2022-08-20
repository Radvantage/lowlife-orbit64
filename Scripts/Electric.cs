using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electric : MonoBehaviour
{
    private GameObject target;
    private bool inside;
    private bool running;

    void Start()
    {
        running = false;
        target = GameObject.Find("Player");
        inside = false;
    }

    void OnTriggerStay(Collider other)
    {
        GameObject hit = other.transform.gameObject;
        if (other.tag == "Player")
        {
            inside = true;
        }
    }

    void Update()
    {
        if (inside && !running)
        {
            StartCoroutine(Shock());
        }
        inside = false;
    }

    IEnumerator Shock()
    {
        running = true;
        target.SendMessage("Damage", (int)(3), SendMessageOptions.RequireReceiver);
        yield return new WaitForSeconds(0.25f);
        running = false;
    }
}
