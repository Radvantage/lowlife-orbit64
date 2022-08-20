using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBase : MonoBehaviour
{
    private GameObject target;
    public GameObject head;
    public GameObject smoke;

    public int hitPoints;
    private bool inside;
    private bool running;

    // Start is called before the first frame update
    void Start()
    {
        running = false;
        target = GameObject.Find("Player");
        inside = false;
        smoke.SetActive(false);
        hitPoints = 16;
    }

    public void ResetHead()
    {
        running = false;
        target = GameObject.Find("Player");
        inside = false;
        smoke.SetActive(false);
        hitPoints = 16;
        head.SetActive(true);
        head.GetComponent<Turret>().canFire = true;
    }

    public void Damage()
    {
        hitPoints--;

        if (hitPoints == 4)
        {
            smoke.SetActive(true);
            smoke.GetComponent<ParticleSystem>().Play();
        }
        if (hitPoints <= 0)
        {
            head.SetActive(false);
            smoke.SetActive(false);
        }
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
            StartCoroutine(Spike());
        }
        inside = false;
    }

    IEnumerator Spike()
    {
        running = true;
        target.SendMessage("Damage", (int)(1), SendMessageOptions.RequireReceiver);
        yield return new WaitForSeconds(0.25f);
        running = false;
    }
}
