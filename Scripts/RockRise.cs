using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRise : MonoBehaviour
{
    private Vector3 originP;
    private Rigidbody rBody;
    private bool risen;
    // Start is called before the first frame update
    void Start()
    {
        risen = false;
        originP = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y - 100, transform.position.z);
        rBody = GetComponent<Rigidbody>();
        //rBody.velocity = new Vector3(0, 20, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!risen)
        {
            rBody.MovePosition(transform.position + new Vector3(0, 1, 0) * Time.deltaTime * 35);
            
            if (Mathf.Round(Vector3.Distance(originP, transform.position)) == 0)
            {
                risen = true;
            }
        }
    }
}
