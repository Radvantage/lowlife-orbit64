using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemyB;
    public GameObject enemyS;
    public GameObject asteroid;
    public GameObject powerUp;
    public GameObject safeZone;
    public Text enemyText;
    public GameObject[] rockGroups;
    public GameObject[] pickupGroups;
    public GameObject[] itemGroups;

    private PlayerController playerController;

    private int enemyCount;
    private int wave;
    private float hpMulti;
    private float range;
    private AudioSource[] sounds;

    private AudioSource announcerSZ;
    private AudioSource announcercomboA;
    private AudioSource announcercomboB;
    private AudioSource announcercomboC;

    private AudioSource announcerPMult;
    private AudioSource announcerPRock;
    private AudioSource announcerPSSpd;
    private AudioSource announcerPSJmp;
    private AudioSource announcerPImmt;

    private bool comboTimer;
    private int comboCount;

    private float roundTimer;

    // Start is called before the first frame update
    void Start()
    {
        //Shuffle Pickups & Items for Semi-Random Set Spawns
        for (int i = 4; i < 8; i++)
        {
            GameObject temp = pickupGroups[i];
            int rIndex = (int)Random.Range(i, 7);
            pickupGroups[i] = pickupGroups[rIndex];
            pickupGroups[rIndex] = temp;
        }
        for (int i = 0; i < 5; i++)
        {
            GameObject temp = itemGroups[i];
            int rIndexB = (int)Random.Range(i, 4);
            itemGroups[i] = itemGroups[rIndexB];
            itemGroups[rIndexB] = temp;
        }

        roundTimer = 0;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        comboCount = 0;
        comboTimer = false;

        sounds = GetComponents<AudioSource>();
        announcerSZ = sounds[0];
        announcercomboA = sounds[1];
        announcercomboB = sounds[2];
        announcercomboC = sounds[3];
        announcerPMult = sounds[4];
        announcerPRock = sounds[5];
        announcerPImmt = sounds[6];
        announcerPSJmp = sounds[7];
        announcerPSSpd = sounds[8];

        wave = 1;
        hpMulti = 1;
        range = 12f;
        SpawnEnemies(1);
    }

    float asteroidTimer;
    // Update is called once per frame
    void Update()
    {
        roundTimer += Time.deltaTime;
        enemyCount = FindObjectsOfType<EnemyAI>().Length;
        enemyText.text = "<" + enemyCount.ToString("D2") + ">";
        if (enemyCount == 0)
        {
            playerController.Score(wave);
            if (roundTimer <= 30)
            {
                playerController.maxHP += 5;
                playerController.MessageToConsole("FAST ROUND: +5 MAX HP");
            }
            roundTimer = 0;
            safeZone.transform.Rotate(0f, 180f, 0f, Space.Self);
            wave++;
            wave = (int)Mathf.Clamp(wave, 1f, 32f);
            range = 16 + (wave * 1.25f);
            hpMulti += 1.5f;
            
            if (wave > 1 && wave < 9)
            {
                if (wave % 2 == 0)
                {
                    rockGroups[(wave / 2) - 1].SetActive(true);
                    pickupGroups[(wave / 2) - 1].SetActive(true);
                }

            }

            if (wave > 9)
            {
                if (wave < 17 && wave % 2 == 0)
                {
                    pickupGroups[(wave / 2) - 1].SetActive(true);
                }
                if (wave % 4 == 0)
                {
                    if (wave > 11 && wave < 29)
                    {
                        itemGroups[(wave / 4) - 3].SetActive(true);
                    }
                }
            }

            SpawnEnemies(wave);
            if (FindObjectsOfType<PowerUp>().Length == 0)
            {
                Instantiate(powerUp, transform.position, powerUp.transform.rotation);
            }
        }

        if (wave > 9)
        {
            
            asteroidTimer += Time.deltaTime;

            float aTimeMax = 16 - (Mathf.Clamp((wave * 2) / 8, 0, 16));

            if (asteroidTimer > aTimeMax)
            {
                Instantiate(asteroid, GeneratePosition(), asteroid.transform.rotation);
                asteroidTimer = 0;
            }
        }

    }

    void SpawnEnemies(int number)
    {
        int max = number;

        if (wave > 1)
        {
            AnnounceInterrupt();
            if (wave % 8 == 0)
            {
                playerController.MessageToConsole("HARDER BOTS. NEW SAFEZONE!");
            }
            else
            {
                playerController.MessageToConsole("ROUND " + wave + " CLEARED. NEW SAFEZONE!");
            }
            
            announcerSZ.Play();

            if (wave > 9)
            {
                if (wave > 11 && wave % 4 == 0)
                {
                    foreach (TurretBase t in FindObjectsOfType<TurretBase>())
                    {
                        t.ResetHead();
                    }
                }

                if (wave % 2 == 0)
                {
                    max += (int) (wave / 8);
                }
            }
        }
        if (number != 6)
        {
            for (int i = 0; i < max; i++)
            {
                if (number < 6)
                {
                    Instantiate(enemy, GeneratePosition(), enemy.transform.rotation);
                }
                else
                {
                    float enemyRandom = Random.Range(-1f, 6f);
                    if (enemyRandom > 3f)
                    {
                        if (wave > 31)
                        {
                            if (Random.Range(0, 4) > 1f)
                            {
                                Instantiate(enemyS, GeneratePosition(), enemy.transform.rotation);
                            }
                            else
                            {
                                Instantiate(enemyB, GeneratePosition(), enemy.transform.rotation);
                            }
                        }
                        else
                        {
                            Instantiate(enemyB, GeneratePosition(), enemy.transform.rotation);
                        } 
                    }
                    else if (enemyRandom < 0)
                    {
                        Instantiate(asteroid, GeneratePosition(), asteroid.transform.rotation);
                    }
                    else
                    {
                        Instantiate(enemy, GeneratePosition(), enemy.transform.rotation);
                    }
                }
            }
        }
        else
        {
            Instantiate(enemyB, GeneratePosition(), enemy.transform.rotation);
            Instantiate(enemyB, GeneratePosition(), enemy.transform.rotation);
        }

        if (wave % 8 == 0)
        {
            foreach (EnemyAI e in FindObjectsOfType<EnemyAI>())
            {
                e.hitPoints += 2*((int)(hpMulti / 6));
            }
        }
        
    }

    void AnnounceInterrupt()
    {
        foreach (AudioSource a in sounds)
        {
            if (a.isPlaying)
            {
                a.Stop();
            }
        }
    }

    void PowerupImmort()
    {
        AnnounceInterrupt();
        announcerPImmt.Play();
    }
    void PowerupJump()
    {
        AnnounceInterrupt();
        announcerPSJmp.Play();
    }
    void PowerupSpeed()
    {
        AnnounceInterrupt();
        announcerPSSpd.Play();
    }
    void PowerupMulti()
    {
        AnnounceInterrupt();
        announcerPMult.Play();
    }
    void PowerupRocket()
    {
        AnnounceInterrupt();
        announcerPRock.Play();
    }

    void ComboScore(int x)
    {
        AnnounceInterrupt();

        switch (x)
        {
            case 3:
                announcercomboA.Play();
                playerController.MessageToConsole("COOL COMBO! +" + (x*5) + " POINTS");
                break;
            case 5:
                announcercomboB.Play();
                playerController.MessageToConsole("CRAZY COMBO! +" + (x*5) + " POINTS");
                break;
            case 7:
                announcercomboC.Play();
                playerController.MessageToConsole("INCONCEIVABLE! +" + (x*5) + " POINTS");
                break;
        }

        playerController.Score(x*5);
    }

    void ComboCounter()
    {
        comboCount++;
        
        if (comboCount == 3)
        {
            ComboScore(comboCount);
        }
        if (comboCount == 5)
        {
            ComboScore(comboCount);
        }
        if (comboCount == 7)
        {
            ComboScore(comboCount);
        }
        
        if (!comboTimer)
        {
            comboTimer = true;
            StartCoroutine(ComboTimer());
        }
        
        
    }

    IEnumerator ComboTimer()
    {
        yield return new WaitForSeconds(5.0f);
        comboCount = 0;
        comboTimer = false;
    }

    private Vector3 GeneratePosition()
    {
        float spawnPosX = Random.Range(-range + transform.position.x, range + transform.position.x);
        float spawnPosZ = Random.Range(-range + transform.position.z, range + transform.position.z);
        Vector3 start = new Vector3(spawnPosX, 30f, spawnPosZ);
        return start;
    }
}
