using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public GameObject smoke;
    private GameObject lightFX;
    private Renderer visual;
    private AudioSource boomSound;

    private float r;
    private float g;
    private float b;
    private float a;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountdownRoutine());
        lightFX = this.gameObject.transform.GetChild(0).gameObject;
        visual = GetComponent<Renderer>();
        boomSound = GetComponent<AudioSource>();
        r = visual.material.color.r;
        g = visual.material.color.g;
        b = visual.material.color.b;
        a = visual.material.color.a;
    }

    private Vector3 vMultiply = new Vector3(10, 10, 10);
    private float vFactor = 2f;
    // Update is called once per frame
    void Update()
    {
        vMultiply *= vFactor;
        vFactor -= (vFactor/20);
        transform.localScale = transform.localScale + vMultiply * Time.deltaTime;
        lightFX.GetComponent<Light>().range += 5f*Time.deltaTime;
        
        visual.material.SetColor("_Color", new Color(r, g, b, a));
        r -= Time.deltaTime;
        g -= Time.deltaTime;
        b -= Time.deltaTime;
        a -= 6f*Time.deltaTime;
    }

    IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(1);
        Instantiate(smoke, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.transform.gameObject;
        if (hit != null)
        {
            if (other.tag == "Player")
            {
                hit.SendMessage("Damage", (int)(Mathf.Clamp(6 * vFactor, 6, 12)), SendMessageOptions.RequireReceiver);
                Destroy(gameObject);
                Instantiate(smoke, transform.position, transform.rotation);
                hit.SendMessage("Push", transform.position, SendMessageOptions.RequireReceiver);
            }

            if (other.tag == "Turret")
            {
                hit.GetComponent<TurretBase>().hitPoints -= 4;
            }

            if (other.tag == "Enemy")
            {
                if (other.transform.parent.gameObject != null && other.transform.parent.gameObject.GetComponent<EnemyAI>() != null)
                {
                    hit = other.transform.parent.gameObject;
                    hit.GetComponent<EnemyAI>().hitPoints--;
                    Instantiate(smoke, transform.position, transform.rotation);
                }
            }
        }
    }
}