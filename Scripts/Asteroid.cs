using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public GameObject smoke;
    private GameObject target;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        target = GameObject.Find("Player");
        transform.LookAt(target.transform.position + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), -transform.right);
        rb.velocity = transform.forward * 20f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(target.transform.position, transform.position) > 160f)
        {
            Destroy(gameObject);
        }
    }
}
