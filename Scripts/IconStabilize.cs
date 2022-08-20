using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconStabilize : MonoBehaviour
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
        transform.rotation = Quaternion.Euler(90f, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
    }
}
