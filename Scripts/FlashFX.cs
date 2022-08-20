using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFX : MonoBehaviour
{
    public ParticleSystem sparksFX;
    public GameObject projectile;

    private GameObject flashFX;
    private GameObject equip;
    private PlayerController pc;
    private AudioSource shotSound;

    public float delay;
    private float delayMax;
    public bool ready;

    void Start()
    {
        flashFX = GameObject.Find("Flash");
        flashFX.transform.localPosition = new Vector3(0f, 0f, -0.5f);
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        
        
        
        equip = GameObject.Find("Weapon");
        shotSound = GetComponent<AudioSource>();
        ready = true;
        delay = 0f;
    }

    IEnumerator DelayShot()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i >= 1)
            {
                ProjectileSpawn();
            }
            yield return new WaitForSeconds(delayMax / 3);
        }
    }

    void ProjectileSpawn()
    {
        if (pc.firePower >= 3.19f)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(projectile, transform.parent.gameObject.transform.position - (transform.right +
                (Vector3.Scale(transform.up, new Vector3(0.125f, 0.125f, 0.125f))) +
                (Vector3.Scale(transform.forward, new Vector3(0.25f, 0.25f, 0.25f)))), 
                transform.parent.gameObject.transform.rotation * Quaternion.AngleAxis(2 * (i - 1), Vector3.up));
            }
        }
        else
        {
            Instantiate(projectile, transform.parent.gameObject.transform.position - (transform.right +
            (Vector3.Scale(transform.up, new Vector3(0.125f, 0.125f, 0.125f))) +
            (Vector3.Scale(transform.forward, new Vector3(0.25f, 0.25f, 0.25f)))), transform.parent.gameObject.transform.rotation);
        }
    }

    void Shoot()
    {
        shotSound.Play();
        sparksFX.Play();
        ready = false;
        delay = delayMax;
        //offset projectile with scaled relative directional vectors
        if (pc.powerI == 3 && !pc.selectingPower)
        {
            StartCoroutine(DelayShot());
        }
        
        ProjectileSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        //Check stats
        if (pc.firePower < 1.59f)
        {
            delayMax = 0.3f * (1.4f - (0.5f * pc.firePower));
        }
        else
        {
            delayMax = 0.15f;
        }

        Cooldown();

        //Interval check and not game over before firing
        if (ready && !pc.gameOver)
        {
            equip.transform.localPosition = new Vector3(equip.transform.localPosition.x, equip.transform.localPosition.y, Mathf.Lerp(equip.transform.localPosition.z, 1f, 16f*Time.deltaTime));
            
            //Prevent firing weapon when paused...
            if (Time.timeScale > 0)
            {
                if (pc.firePower < 1.59f && Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }
                else if (pc.firePower >= 1.59f)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        Shoot();
                    }
                }
                
            }
        }

        if (delay > (delayMax/2)) {
            flashFX.transform.localPosition = new Vector3(-0.0025f, 0f, 0.00045f);
        }
        else
        {
            flashFX.transform.localPosition = new Vector3(0f, 0f, -0.05f);
        }
        flashFX.transform.Rotate(0f, 2.5f, 0f, Space.Self);
    }

    void Cooldown()
    {
        if (!ready)
        {
            delay -= Time.deltaTime;
            equip.transform.localPosition = new Vector3(equip.transform.localPosition.x, equip.transform.localPosition.y, Mathf.Lerp(equip.transform.localPosition.z, 0.8f, (delayMax * 100)*Time.deltaTime));
        }
        if (delay < 0)
        {
            ready = true;
            delay = 0;
        }
    }
}