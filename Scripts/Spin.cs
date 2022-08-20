using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float rSpeed = 90.0f;
    public bool bob;
    private float i;

    // Start is called before the first frame update
    void Start()
    {
        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, rSpeed * Time.deltaTime, Space.Self);
        if (bob)
        {
            transform.position += new Vector3(0f, Mathf.Sin(i) * (2f*Time.deltaTime), 0f);
            i += Mathf.PI * Time.deltaTime;
        }
    }
}
