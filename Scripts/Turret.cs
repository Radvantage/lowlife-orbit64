using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject ammoSecondary;
    public AudioSource shootSoundSecondary;

    private Transform target;
    public bool canFire;
    // Start is called before the first frame update
    void Awake()
    {
        target = GameObject.Find("Player").transform;
        canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target, Vector3.up);
    }

    void FixedUpdate()
    {
        if (canFire && Vector3.Distance(transform.position, target.position) < 40f)
        {
            shootSoundSecondary.Play();
            StartCoroutine(FireBullets((int)Random.Range(4, 12)));
        }
    }

    IEnumerator FireBullets(int amount)
    {
        canFire = false;
        for (int i = 0; i < amount; i++)
        {
            Instantiate(ammoSecondary, transform.position + (2.25f*transform.forward), transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
        float seconds = Random.Range(2, 6);
        StartCoroutine(Reload(seconds));
    }

    IEnumerator Reload(float time)
    {
        yield return new WaitForSeconds(time);
        canFire = true;
    }
}
