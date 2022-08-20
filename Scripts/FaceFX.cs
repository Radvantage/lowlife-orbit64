using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceFX : MonoBehaviour
{
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = target.transform.rotation;
    }
}
