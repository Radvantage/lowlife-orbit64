using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swivel : MonoBehaviour
{
    public float rSpeed;
    public float timeUntilReversal;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeUntilReversal)
        {
            rSpeed *= -1f;
            timer = 0f;
        }
        
        transform.Rotate(0.0f, 0.0f, rSpeed * Time.deltaTime, Space.Self);
    }
}
