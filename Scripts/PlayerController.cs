using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public CameraFollow cam;
    public float velocity = 12f;
    public float jumpHeight = 8f;

    public Text lifeText;
    public Text scoreText;
    public Text powerTimer;
    public Text console;
    public Text statView;
    public TMPro.TMP_Text livesText;
    public RawImage powerIcon;

    public Texture[] powers;
    public GameObject modA;
    public GameObject modB;
    public GameObject shield;

    private string[] consoleFeed = new string[3];
    private float[] feedTimers = new float[3];
    private int feedIndex = 0;

    //Items - Boots & Belt
    private int freeJumps;
    private int multiJump;
    private int scrambleDegree;

    private float step;
    private float strafe;
    private bool floor;
    public bool gameOver;
    private int hitPoints;
    private int score;

    //1 - Speed
    //2 - Jump
    //3 - Multi
    //4 - Rocket
    //5 - Immortality
    public int powerI;
    public GameObject explosive;
    public bool selectingPower;

    public int maxHP;
    public int lives;
    public float resistance;
    public float firePower;
    public Singleton gameData;

    private CharacterController playerBody;

    private float defV;
    private float defJ;
    private Vector3 pushVector;

    public AudioSource[] sounds;
    private AudioSource walkSound;
    private AudioSource jumpSound;
    private AudioSource hurtSoundA;
    private AudioSource hurtSoundB;
    private AudioSource hurtSoundC;
    private AudioSource deadSound;
    private AudioSource jingle;
    private AudioSource boostSound;
    private AudioSource lifeSound;
    private AudioSource firepowerSound;
    private AudioSource resistanceSound;
    private AudioSource respawnSound;
    private AudioSource landSound;
    private AudioSource burpSound;
    private AudioSource itemSound;

    private ParticleSystem healFX;
    private GameObject hudGroup;

    private SpawnManager game;

    private Vector3 originP;
    private Quaternion originR;

    public int ScrambleDegree()
    {
        return scrambleDegree;
    }

    public int HitPoints()
    {
        return hitPoints;
    }

    private string statsFormatted;

    void StatsText()
    {
        statsFormatted = FormattedStats();
        
        statView.text = statsFormatted;
    }

    string FormattedStats()
    {
        string fpTier;
        if (firePower > 1.59f && firePower < 3.19f)
        {
            fpTier = " (TIER II)";
        }
        else if (firePower > 3.19f)
        {
            fpTier = " (TIER III)";
        }
        else
        {
            fpTier = " (TIER I)";
        }

        string result = "\n\n\n" +
            "MAX HP = " + maxHP +
            "\nRESISTANCE = " + (Mathf.Round(resistance)) +
            "\nFIREPOWER = " + (Mathf.Round(firePower * 100)) + fpTier +
            "\n\nJUMP = " + (multiJump + 1) + "X" +
            "\nHACK = " + scrambleDegree + "X";
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameEye(Clone)").GetComponent<Singleton>();
        MessageToConsole("PRESS [TAB] TO VIEW STATS");
        multiJump = 0;
        freeJumps = 0;
        scrambleDegree = 0;

        lives = 0;
        resistance = 0;
        firePower = 0.8f;
        maxHP = 100;

        originP = transform.position;
        originR = transform.rotation;
        selectingPower = false;
        powerI = 0;
        
        defV = velocity;
        defJ = jumpHeight;
        pushVector = new Vector3(0f, 0f, 0f);
        
        lifeText = GameObject.Find("LifeText").GetComponent<Text>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        powerTimer = GameObject.Find("PowerTimer").GetComponent<Text>();
        powerIcon = GameObject.Find("PowerIcon").GetComponent<RawImage>();
        game = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        hudGroup = GameObject.Find("HUD");

        sounds = GetComponents<AudioSource>();

        walkSound = sounds[0];
        jumpSound = sounds[1];
        hurtSoundA = sounds[2];
        hurtSoundB = sounds[3];
        hurtSoundC = sounds[4];
        deadSound = sounds[5];
        jingle = sounds[6];
        landSound = sounds[7];
        respawnSound = sounds[8];
        itemSound = sounds[9];
        boostSound = sounds[10];
        lifeSound = sounds[11];
        firepowerSound = sounds[12];
        resistanceSound = sounds[13];
        burpSound = sounds[14];
        

        hitPoints = maxHP;
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = GetComponent<CharacterController>();
        healFX = GetComponent<ParticleSystem>();
        gameOver = false;
        walkSound.pitch = 0.65f;
        livesText.text = "x" + lives.ToString("D2");
    }

    public float gForce = -12f;
    private float fConst = 1f;
    private Vector3 vDisplace = new Vector3();
    public string scoreLabel = "SCORE";

    private void Respawn()
    {
        if (lives > 0)
        {
            StopCoroutine(PowerTimer());
            powerTimer.text = "";
            ResetPowers();

            Time.timeScale = 0;

            //Clear projectiles
            foreach (FireRocket r in FindObjectsOfType<FireRocket>())
            {
                r.DestroySequence();
            }

            foreach (FireBullet b in FindObjectsOfType<FireBullet>())
            {
                b.Impact();
            }

            playerBody.enabled = false;
            pushVector = new Vector3(0f, 0f, 0f);
            transform.position = originP;
            transform.rotation = originR;
            playerBody.enabled = true;
            hitPoints = maxHP;
            lives--;
            livesText.text = "x" + lives.ToString("D2");

            Time.timeScale = 1;

            healFX.Play();
            respawnSound.Play();
        }
    }

    void Boost()
    {
        boostSound.Play();
        vDisplace.y = Mathf.Sqrt(jumpHeight * -8f * gForce + fConst);
        floor = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        //Rotate messages from the console out and in, update text field, only if there are messages remaining
        string consoleContents = string.Join("", consoleFeed);
        if (consoleContents != "")
        {
            for (int i = 0; i < consoleFeed.Length; i++)
            {
                if (feedTimers[i] > 0)
                {
                    feedTimers[i] -= Time.deltaTime;
                }
                else
                {
                    feedTimers[i] = 0;
                    if (consoleFeed[i] != "")
                    {
                        consoleFeed[i] = "";
                        if (feedIndex > 0)
                        {
                            if (feedTimers[feedIndex - 1] <= 0)
                            {
                                feedIndex--;
                            }
                        }
                        
                        ResetConsole();
                    }
                }
            }
        }

        walkSound.pitch = 0.65f * (velocity / defV);
        lifeText.text = hitPoints.ToString("D3");
        scoreText.text = scoreLabel + " " + score.ToString("D5");

        if (transform.position.y < -80 && !gameOver)
        {
            powerI = 0;
            if (lives == 0)
            {
                Damage(1000);
            }
            else
            {
                Respawn();
            }
            
        }
        if (!gameOver)
        {
            StatsText();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (statView.gameObject.activeInHierarchy)
                {
                    statView.gameObject.SetActive(false);
                }
                else
                {
                    statView.gameObject.SetActive(true);
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (floor)
                {
                    vDisplace.y = Mathf.Sqrt(jumpHeight * -2f * gForce + fConst);
                    jumpSound.Play();
                    floor = false;
                }
                else if (freeJumps > 0)
                {
                    vDisplace.y = Mathf.Sqrt(jumpHeight * -2f * gForce + fConst);
                    jumpSound.Play();
                    freeJumps--;
                }
            }

            if (vDisplace.y < -1.5)
            {
                floor = false;
            }

            GetDirection();
            
            //Accelerate using force of gravity if controller not touching ground
            if (!playerBody.isGrounded)
            {
                vDisplace.y += gForce * Time.deltaTime;

                //Terminal velocity, clamp the min and max vertical velocity
                vDisplace.y = Mathf.Clamp(vDisplace.y, gForce, Mathf.Pow(jumpHeight, 2f));
            }

            //Strafe - pivot
            playerBody.Move((transform.right * strafe));
            
            //Forward/backward - walk
            playerBody.Move(transform.forward * step);

            //Up/down - gravity, jumping
            playerBody.Move((vDisplace + pushVector) * Time.deltaTime);

            //Check if grounded after movement for next frame
            if (playerBody.isGrounded)
            {
                if (multiJump > 0)
                {
                    freeJumps = multiJump;
                }

                floor = true;
                //Prevent accumulation of g-acceleration when falling across surfaces
                if (vDisplace.y < gForce / 2)
                {
                    landSound.Play();
                }
                vDisplace.y = gForce/4;
            }
            if (floor)
            {
                if (step != 0 || strafe != 0)
                {
                    if (!walkSound.isPlaying)
                    {
                        walkSound.Play();
                    }
                }
                else
                {
                    if (walkSound.isPlaying)
                    {
                        walkSound.Stop();
                    }
                }
            }
            else
            {
                if (walkSound.isPlaying)
                {
                    walkSound.Stop();
                }
            } 
        }
        else
        {
            if (Time.timeScale > 0 && !deadSound.isPlaying)
            {
                AudioListener.volume = Mathf.Lerp(AudioListener.volume, 0f, 5f * Time.deltaTime);
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, 0.5f*Time.deltaTime);
            }
            if (Time.timeScale < (1f/3f))
            {
                AudioListener.volume = 1;
                Time.timeScale = 1;
                gameData.Score = score;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("ScorePage", LoadSceneMode.Single);
            }
        }
        
        if (gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            AudioListener.volume = 1;
            Time.timeScale = 1;
            gameData.Score = score;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("ScorePage", LoadSceneMode.Single);
        }
     }

    //Explosive Hit
     IEnumerator Push(Vector3 fromVector)
     {
        cam.SendMessage("Hurt");
        Vector3 targetV = new Vector3(0, 0, 0);
        
        pushVector = Vector3.ClampMagnitude(16*(transform.position - fromVector), 48f);
        if (powerI == 5)
        {
            pushVector = new Vector3(0, 0, 0);
        }
        while (Vector3.Distance(pushVector, targetV) > 0.1f)
        {

            pushVector = Vector3.Lerp(pushVector, targetV, 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
        
        pushVector = new Vector3(0f, 0f, 0f);
     }

    //Helper method - access inputs and calculate velocity per frame format
     private void GetDirection()
     {
        step = Input.GetAxis("Vertical") * velocity * Time.deltaTime;

        //adjust strafe, slower than direct movement
        strafe = Input.GetAxis("Horizontal") * (velocity * 0.75f) * Time.deltaTime;
     }

     void OnControllerColliderHit(ControllerColliderHit hit)
     {
         //Prevent floating jump on upper collision, reverse vertical momentum on impact
         if (hit.moveDirection.y >= 1) {
             vDisplace.y *= -0.25f;
         }
     }
    
    //Let other objects know if player is grounded
     public bool OnFloor() {
         return floor;
     }

     IEnumerator Shield()
     {
         shield.SetActive(true);
         yield return new WaitForSeconds(0.25f);
         shield.SetActive(false);
     }

    //Damage Handler
     void Damage(int points)
     {
         if (powerI != 5)
         {
             float chance = Random.Range(0, 99);
             //Apply resistance if point hit is less than full health
             if (points < 100 && resistance > chance)
             {
                 MessageToConsole("RESISTED " + points + " DAMAGE!");
                 StartCoroutine(Shield());
                 return;
             }
             else
             {
                 hitPoints -= points;
             }
         }

         if (lives > 0 && hitPoints <= 0)
         {
             Respawn();
             return;
         }

         if (hitPoints <= 0 && lives == 0)
         {
             if (Time.timeScale > 0.5)
             {
                 deadSound.Play();
             }
             
             hitPoints = 0;
             powerI = 0;
             hudGroup.SetActive(false);
             gameOver = true;
         }
         else
         {
            int dmgAudio = (int)Random.Range(1, 4);
            if (!hurtSoundA.isPlaying && !hurtSoundB.isPlaying && !hurtSoundC.isPlaying && hitPoints > 5)
            {
                switch (dmgAudio)
                {
                    case 1:
                        hurtSoundC.Play();
                        break;
                    case 2:
                        hurtSoundB.Play();
                        break;
                    default:
                        hurtSoundA.Play();
                        break;
                }
            }
         }
     }

    //Add Score
     public void Score(int points)
     {
         score += points;
     }

    //Powerups - Start Powerup Sequence (SuperBox)
     void ActivatePower()
     {
         jingle.Play();
         StartCoroutine(PowerSelector());
     }
    //Powerups - Select (Random Scroll)
     IEnumerator PowerSelector()
    {
        selectingPower = true;
        int tempI = 0;
        for (int i = 0; i < 24; i++)
        {
            if (tempI == 6)
            {
                tempI = 1;
            }
            powerIcon.texture = powers[tempI];
            tempI++;
            yield return new WaitForSeconds(0.125f);
        }
        powerI = Random.Range(1, 6);
        
        powerIcon.texture = powers[powerI];
        if (powerI == 1)
        {
            game.SendMessage("PowerupSpeed");
            velocity = defV * 2;
        }
        if (powerI == 2)
        {
            game.SendMessage("PowerupJump");
            jumpHeight = defJ * 2;
        }
        if (powerI == 3)
        {
            game.SendMessage("PowerupMulti");
        }
        if (powerI == 4)
        {
            game.SendMessage("PowerupRocket");
        }
        if (powerI == 5)
        {
            game.SendMessage("PowerupImmort");
        }
        StartCoroutine(PowerTimer());
        selectingPower = false;
    }
    //Powerups - Timer Active
    IEnumerator PowerTimer()
    {
        for (int i = 0; i < 15; i++)
        {
            if (powerI > 0)
            {
                int powerT = 15 - i;
                powerTimer.text = powerT.ToString("D2");
            }

            yield return new WaitForSeconds(1);
        }
        powerTimer.text = "";
        ResetPowers();
    }
    //Powerups - Reset to Default Values
    private void ResetPowers()
    {
        powerI = 0;
        velocity = defV;
        jumpHeight = defJ;
        powerIcon.texture = powers[powerI];
    }

    //Healing (In SafeZone)
     void Heal()
     {
         if (hitPoints < maxHP && !gameOver)
         {
             if (!healFX.isPlaying)
             {
                 healFX.Play();
             }
             
             hitPoints += 1;
             if (hitPoints == maxHP)
             {
                 burpSound.Play();
             }
         }
     }

     public void MessageToConsole(string message)
     {
         consoleFeed[feedIndex] = message;
         ResetConsole();

        feedTimers[feedIndex] = 3f;
         
         feedIndex++;
         if (feedIndex > consoleFeed.Length - 1)
         {
             feedIndex = 0;
         }
     }

     private void ResetConsole()
     {
         console.text = "";
         foreach (string s in consoleFeed)
         {
             console.text += s + "\n";
         }
     }

    //Pickups
     void OnTriggerEnter(Collider other)
     {
         GameObject hit = other.transform.gameObject;
         switch (other.tag)
         {
            case "Clone":
                MessageToConsole("'CLONE' PICKUP: +1 LIFE");
                lives++;
                livesText.text = "x" + lives.ToString("D2");
                lifeSound.Play();
                Destroy(hit);
                return;
            case "Battery":
                firePower += 0.1f;
                
                if (firePower > 1.59f && !modA.activeInHierarchy)
                {
                    MessageToConsole("FIREPOWER TIER II: AUTOMATIC");
                    modA.SetActive(true);
                }
                else if (firePower > 3.19f && !modB.activeInHierarchy)
                {
                    MessageToConsole("FIREPOWER TIER III: BUCKSHOT");
                    modB.SetActive(true);
                }
                else
                {
                    MessageToConsole("'CIRCUIT' PICKUP: +10 FIREPOWER");
                }
                
                firepowerSound.Play();
                Destroy(hit);
                return;
            case "Pill":
                MessageToConsole("'PELLET' PICKUP: +1 RESISTANCE");
                resistance += 1f;
                resistanceSound.Play();
                Destroy(hit);
                return;
            case "Boots":
                multiJump++;
                MessageToConsole("'BOOTS' ITEM: " + (multiJump + 1) + "X JUMP");
                freeJumps = multiJump;
                itemSound.Play();
                Destroy(hit);
                break;
            case "Belt":
                scrambleDegree += 5;
                MessageToConsole("'TOOLBELT' ITEM: " + (scrambleDegree) + "X HACK");
                itemSound.Play();
                Destroy(hit);
                break;
            case "Pad":
                Boost();
                return;
         }
     }
}