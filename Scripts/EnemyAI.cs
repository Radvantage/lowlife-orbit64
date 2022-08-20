using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject ammo;
    public GameObject ammoSecondary;
    public bool secondary;
    public bool canShock;
    public GameObject smoke;
    public float responseTime;
    
    private GameObject lastHit;
    private bool canHit;
    private Transform target;
    private float speed = 16f;
    private float strafe;
    private Rigidbody rb;

    public int hitPoints;
    private ParticleSystem shootFX;
    private ParticleSystem smokeFX;

    private AudioSource[] sounds;
    private AudioSource shootSound;
    private AudioSource shootSoundSecondary;
    private PlayerController targetPC;

    private SpawnManager game;

    private bool seekingTarget;
    private float seekTime;
    private float tRange;

    // Start is called before the first frame update
    void Start()
    {
        tRange = 20f;
        if (canShock)
        {
            tRange /= 4f;
        }

        strafe = 0;
        speed *= (responseTime / 8f);
        seekTime = 0f;
        target = GameObject.Find("Player").transform;
        targetPC = target.GetComponent<PlayerController>();
        game = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        rb = GetComponent<Rigidbody>();
        sounds = GetComponents<AudioSource>();
        shootSound = sounds[0];
        if (secondary)
        {
            shootSoundSecondary = sounds[1];
        }
        lastHit = gameObject;
        canHit = false;
        StartCoroutine(RefreshRoutine());

        shootFX = gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        smokeFX = gameObject.transform.GetChild(2).GetComponent<ParticleSystem>();
    }

    
    // Update is called once per frame
    void Update()
    {
        //Destroy if enemy slips underneath the map - fall into wormhole!
        if (transform.position.y < -36)
        {
            Instantiate(smoke, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        Combat();
        if (!targetPC.gameOver)
        {
            bool canMove = true;

            Vector3 approx = target.position - transform.position;
            float distance = Vector3.Distance(transform.position, target.position);

            if (canMove)
            {
                rb.AddForce(transform.right * strafe * (0.5f*speed));
                if (transform.position.y < 0f)
                {
                    rb.AddForce(Vector3.up * 2.25f * speed);
                }
                
                if (distance > tRange)
                {
                    rb.AddForce(transform.forward * ((distance/speed)*speed));
                }
                else if (distance < (tRange - 5f))
                {
                    rb.AddForce(transform.forward * ((-speed/distance)*speed));
                }
                else
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, 0), Time.deltaTime);
                    if (Random.Range(-1f, 1f) > 0)
                    {
                        rb.AddForce(transform.up * (2.5f * speed));
                    }
                    else
                    {
                        rb.AddForce(transform.up * (2.5f * speed));
                    }
                }
            }
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed/2f);
            if (!secondary)
            {
                if (!seekingTarget)
                {
                    //transform.LookAt(target, Vector3.left);
                    seekTime = Random.Range(1f, responseTime);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(approx, Vector3.left), 2.5f*Time.deltaTime);
                }
                
                if (seekTime > 4)
                {
                    seekingTarget = true;
                    
                    StartCoroutine(Seeking(seekTime));
                    seekTime = -1f;
                }
            }
            else
            {
                transform.LookAt(target, Vector3.left);
            }

            if (hitPoints < 2)
            {
                if (!smokeFX.isPlaying)
                {
                    smokeFX.Play();
                }
                if (hitPoints <= 0)
                {
                    game.SendMessage("ComboCounter");
                    Instantiate(smoke, transform.position, transform.rotation);
                    Destroy(gameObject);
                }
            }
        }
    }

    //Seek target cycle
    IEnumerator Seeking(float r)
    {
        strafe = Random.Range(-3f, 3f);
        yield return new WaitForSeconds(r);
        seekingTarget = false;
        strafe = 0f;
        transform.LookAt(target, Vector3.left);
    }

    //Aiming & Shooting == Previously FixedUpdate() as a note - !BAD PERFORMANCE!
    void Combat()
    {
        int eCount = FindObjectsOfType<EnemyAI>().Length;
        //Raycast to check if player in vision
        if (!canShock)
        {
            if (!seekingTarget && eCount < 16)
            {
                var ray = new Ray(transform.position, transform.forward);
            
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 128))
                {
                    lastHit = hit.transform.gameObject;
                }
            }
            else if (eCount > 15 && Vector3.Distance(transform.position, target.position) < 80f)
            {
                lastHit = GameObject.Find("Player");
            }
        }
        else
        {
            lastHit = GameObject.Find("Player");
        }

        if (lastHit != null)
        {
            if (lastHit.tag == "Player" || lastHit.tag == "Safe")
            {
                if (Random.Range(-10f, 30f) > 0f)
                {
                    if (canHit && FindObjectsOfType<FireRocket>().Length < 11)
                    {
                        
                        if (!secondary)
                        {
                            shootFX.Play();
                            shootSound.Play();
                            Instantiate(ammo, transform.position + (3f*transform.forward), transform.rotation);
                        }
                        else
                        {
                            if (Random.Range(0f, 2f) >= 1f)
                            {
                                shootFX.Play();
                                shootSound.Play();
                                Instantiate(ammo, transform.position + (3f*transform.forward), transform.rotation);
                            }
                            shootSoundSecondary.Play();
                            StartCoroutine(FireBullets((int)Random.Range(5, 10)));
                        }
                        
                        canHit = false;
                        StartCoroutine(RefreshRoutine());
                    }
                }
            }
        }
    }

    IEnumerator FireBullets(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(ammoSecondary, transform.position + (2.25f*transform.forward) + (-2.5f*transform.up), transform.rotation);
            Instantiate(ammoSecondary, transform.position + (2.25f*transform.forward) + (2.5f*transform.up), transform.rotation);
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator RefreshRoutine()
    {
        yield return new WaitForSeconds(5);
        canHit = true;
    }
}