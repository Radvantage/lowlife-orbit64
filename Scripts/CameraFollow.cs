using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    public float mSensitivity = 2.0f;
    [SerializeField]
    public float mSmoothing = 10.0f;

    public GameObject body;
    public GameObject equip;
    public GameObject tex;
    public AudioMixerGroup audioM;

    private Vector2 mouseLook;
    private Vector2 resultLook;

    // - Animation Variables -
    //Strafing lean var
    private float strafeD;

    //Headbob vars
    private float count = 0;
    private float median = 0.5f;

    private float bIntensity = 0.375f;
    private PlayerController pc;
    private Camera cam;
    public float bSpeed = 2f;
    private Color defC;

    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioSource a in FindObjectsOfType<AudioSource>())
        {
            a.outputAudioMixerGroup = audioM;
        }

        cam = GetComponent<Camera>();
        cam.fieldOfView = ((PlayerPrefs.GetInt("FOV", 100)) * (Screen.width / Screen.height)) / ((float) cam.pixelWidth / cam.pixelHeight);

        defC = tex.GetComponent<RawImage>().color;
        Application.targetFrameRate = 60;
        body = this.transform.parent.gameObject;
        pc = body.GetComponent<PlayerController>();
        equip = GameObject.Find("Weapon");
        strafeD = 0f;
    }

    private float eqpAngleZ; 
    private float eqpAngleX;
    private Color render;
    // Update is called once per frame
    void Update()
    {
        if (!pc.gameOver)
        {
            float colorPercent = ((float)pc.HitPoints() / (float)pc.maxHP);
            render = Color.white;
            render.r = Mathf.Clamp(colorPercent + 0.5f, 0.5f, 1f);
            render.g = Mathf.Clamp((2f * colorPercent) + 0.5f, 0.5f, 1f);
            render.b = Mathf.Clamp((2f * colorPercent) + 0.5f, 0.5f, 1f);
            if (pc.HitPoints() < Mathf.Round(0.25f * (float)pc.maxHP))
            {
                render.a = Mathf.Clamp(6 * colorPercent, 0.45f, 0.95f);
            }
            else if (render.a < 1)
            {
                render.a = Mathf.Lerp(render.a, 1, Time.deltaTime);
            }
            
            if (tex.GetComponent<RawImage>().color != render)
            {
                tex.GetComponent<RawImage>().color = Color.Lerp(tex.GetComponent<RawImage>().color, render, 2 * Time.deltaTime);
            }
            

            var delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            delta = Vector2.Scale(delta, new Vector2(mSensitivity * mSmoothing, mSensitivity * mSmoothing));
            
            //Prevent camera movement when paused...
            if (Time.timeScale > 0)
            {
                resultLook.x = Mathf.Lerp(resultLook.x, delta.x, 1f/mSmoothing);
                resultLook.y = Mathf.Lerp(resultLook.y, delta.y, 1f/mSmoothing);
            }
            else
            {
                resultLook = Vector2.zero;
            }

            if (mouseLook.y + resultLook.y < 85f && mouseLook.y + resultLook.y > -85f) {
                mouseLook.y += resultLook.y;
            }
            mouseLook.x += resultLook.x;
            
            transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            body.transform.rotation = Quaternion.AngleAxis(mouseLook.x, body.transform.up);

            strafeD = Mathf.Lerp(strafeD, -6f*Input.GetAxis("Horizontal"), 3f*Time.deltaTime);

            transform.Rotate(new Vector3(0f, 0f, strafeD));
            
            if (Input.GetAxisRaw("Mouse X") != 0f)
            {
                eqpAngleZ = Mathf.Clamp(Mathf.Lerp(eqpAngleZ, 5*delta.x, 2f*Time.deltaTime), -10f, 10f);
            }
            else
            {
                eqpAngleZ = Mathf.Lerp(eqpAngleZ, 0, 4f*Time.deltaTime);
            }

            if (Input.GetAxisRaw("Mouse Y") != 0f)
            {
                eqpAngleX = Mathf.Clamp(Mathf.Lerp(eqpAngleX, 5*delta.y, 2f*Time.deltaTime), -10f, 10f);
            }
            else
            {
                eqpAngleX = Mathf.Lerp(eqpAngleX, 0, 4f*Time.deltaTime);
            }

            equip.transform.localRotation = Quaternion.Euler(-85f + eqpAngleX, 180f, -90f + eqpAngleZ);
            
            Headbob();
        }
        else
        {
            tex.transform.localScale = Vector3.Lerp(tex.transform.localScale, new Vector3(16f, 20f*(2f/3f), 16f), Time.deltaTime);
            render = Color.red;
            render.a = 0.5f;
            tex.GetComponent<RawImage>().color = Color.Lerp(tex.GetComponent<RawImage>().color, render, Time.deltaTime);
        }

    }

    void Hurt()
    {
        //transform.localRotation = Quaternion.Euler(5f, 0, 0);
    }
    
    void Headbob()
    {
        Vector3 bobPos = transform.localPosition;
        Vector3 eqpPos = equip.transform.localPosition;

        float lastY = bobPos.y;
        float eqpY = eqpPos.y;

        float trigArc = 0f;

        float forward = Mathf.Abs(Input.GetAxis("Vertical"));
        float side = Mathf.Abs(Input.GetAxis("Horizontal"));
        if (forward == 0f && side == 0f)
        {
            count = 0f;
        }
        else
        {
            if (body.GetComponent<PlayerController>().OnFloor())
            {
                trigArc = Mathf.Sin(count);
                count += bSpeed*Mathf.PI;
                if (count > 2*Mathf.PI)
                {
                    count = 0f;
                }
            }
            else
            {
                count = 0f;
                trigArc = 0f;
            }
        }

        if (trigArc != 0)
        {
            float displace = trigArc * bIntensity;
            float axis = Mathf.Clamp(forward + side, 0f, 1f);
            displace *= axis;
            bobPos.y = displace + median;
            eqpPos.y = (displace/12f) + (median - 0.8f);
        }
        else {
            bobPos.y = median;
            eqpPos.y = median - 0.8f;
        }

        //Dampen sudden shifts in motion
        bobPos.y = Mathf.Lerp(lastY, bobPos.y, 6f*Time.deltaTime);
        eqpPos.y = Mathf.Lerp(eqpY, eqpPos.y, 6f*Time.deltaTime);

        transform.localPosition = bobPos;
        equip.transform.localPosition = eqpPos;
    }
}