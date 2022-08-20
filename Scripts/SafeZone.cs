using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public GameObject target;
    private Material zone_m;
    private Light light;
    float green;
    private bool inside;
    private bool running;
    private int scale;

    private float healLimit = 0;
    void OnTriggerStay(Collider other)
    {
        GameObject hit = other.transform.gameObject;
        if (other.tag == "Player" && healLimit < 13 * (scale / 100))
        {
            inside = true;
            healLimit += Time.deltaTime;
        }
    }

    public void Start()
    {
        zone_m = GetComponent<Renderer>().material;
        light = GetComponent<Light>();
        green = zone_m.color.g;
    }

    public void Update()
    {
        scale = target.GetComponent<PlayerController>().maxHP;
        green = 1 - (healLimit / (13 * (scale / 100)));

        green = Mathf.Clamp(green, 0 , 1);
        healLimit = Mathf.Clamp(healLimit, 0, 13 * (scale / 100));
        zone_m.color = new Color(zone_m.color.r, green, zone_m.color.b, zone_m.color.a);
        light.color = new Color(zone_m.color.r, green, zone_m.color.b, zone_m.color.a);
    }
        
        

    void FixedUpdate()
    {
        if (inside && !running)
        {
            StartCoroutine(HealPlayer());
        }
        if (!inside && healLimit > 0)
        {
            healLimit -= 0.5f * Time.deltaTime;
        }
        inside = false;
    }

    IEnumerator HealPlayer()
    {
        running = true;
        
        yield return new WaitForSeconds(0.25f);
        if (healLimit < (10.75 * (scale / 100)))
        {
            target.SendMessage("Heal");
        }
        else
        {
            inside = false;
        }
        running = false;
    }
}


