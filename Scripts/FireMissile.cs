using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMissile : MonoBehaviour
{
    private float velocity;
    public GameObject smoke;

    private Rigidbody rb;
    private GameObject player;
    private PlayerController playerController;
    private float power;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        power = playerController.firePower;
        transform.localScale *= (0.5f * power);
        
        velocity = 36 * power;
        
        //Projectile Spread
        float maxR = 3 - Mathf.Clamp(power, 0, 3);
        float minR = 1.5f - Mathf.Clamp((power / 2), 0, 1.5f);
        transform.Rotate(Random.Range(-maxR, minR), Random.Range(-maxR, minR), Random.Range(-maxR, minR), Space.Self);

        rb.velocity = transform.forward * velocity;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.forward * velocity * Time.deltaTime);
        if (Vector3.Distance(player.transform.position, transform.position) > 256f)
        {
            Impact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            if (other.tag == "Safe" || other.tag == "Minion")
            {
                return;
            }
            if (playerController.powerI == 4 && !playerController.selectingPower)
            {
                if (other.tag != "Projectile")
                {
                    Instantiate(playerController.explosive, transform.position, transform.rotation);
                }
            }
            if (other.tag == "Enemy")
            {
                if (other.transform.parent.gameObject != null)
                {
                    GameObject target = other.transform.parent.gameObject;
                    if (target != null && target.GetComponent<EnemyAI>() != null)
                    {
                        target.GetComponent<EnemyAI>().hitPoints--;
                    }
                    
                }
            }
            
            if (other.tag == "Turret")
            {
                GameObject target = other.transform.gameObject;
                target.GetComponent<TurretBase>().Damage();
            }

            if (other.tag == "Clone" || other.tag == "Battery" || other.tag == "Pill" || other.tag == "Pickup" || other.tag == "Projectile" || other.tag == "Hazard")
            {
                return;
            }
            Impact();
        }
        else
        {
            return;
        }
    }

    private void Impact()
    {
        Instantiate(smoke, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}