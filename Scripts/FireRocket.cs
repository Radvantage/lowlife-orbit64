using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRocket : MonoBehaviour
{
    public bool isGrenadeRocket;
    private Transform target;
    private PlayerController pc;

    private float scramblePercent;
    private bool scrambling;

    private bool isFiring;

    public GameObject fizz;
    public GameObject bullet;
    private float velocity;
    private Vector3 approx;

    void Start()
    {
        isFiring = false;
        if (!isGrenadeRocket)
        {
            velocity = 18f;
        }
        else
        {
            velocity = 22f;
        }

        target = GameObject.Find("Player").transform;
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        scramblePercent = pc.ScrambleDegree();

        approx = new Vector3(0, 0, 0);
        float rGen = Random.Range(1, 199);
        if (scramblePercent < rGen)
        {
            scrambling = false;
        }
        else
        {
            StartCoroutine(Scramble());
            scrambling = true;
        }

        StartCoroutine(AutoRoutine());
    }

    
    // Update is called once per frame
    void Update()
    {   
        velocity = 15 * (5 / Mathf.Sqrt(Vector3.Distance(transform.position, target.position)));
        velocity = Mathf.Clamp(velocity, 10, 45);
        if (!scrambling)
        {
            if (!isGrenadeRocket)
            {
                approx = target.position - transform.position;
            }
            else
            {
                approx = (target.position - transform.position) + (-0.2f * Vector3.up);
                if (!isFiring)
                {
                    StartCoroutine(BulletRing());
                }
            }
            
        }
        else
        {            
            approx = (transform.position - target.position) + ((scramblePercent / 5) * Vector3.up);
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(approx, Vector3.up), 2f*Time.deltaTime);
        if (isGrenadeRocket)
        {
            transform.Rotate(0f, 0f, 90f, Space.Self);
        }
        
        transform.Translate(Vector3.forward * velocity * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) > 128f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator BulletRing()
    {
        isFiring = true;
        for (int i = 0; i < 32; i++)
        {
            Instantiate(bullet, transform.position, (transform.rotation * transform.rotation));
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f);
        isFiring = false;
    }

    IEnumerator Scramble()
    {
        pc.MessageToConsole("HACKED ROCKET: SCRAMBLING!");
        yield return new WaitForSeconds(3f);
        scrambling = false;
    }

    IEnumerator AutoRoutine()
    {
        yield return new WaitForSeconds(8);
        DestroySequence();
    }

    public void DestroySequence()
    {
        Instantiate(fizz, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
