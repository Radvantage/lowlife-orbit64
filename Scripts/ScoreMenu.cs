using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreMenu : MonoBehaviour
{
    public Singleton gameData;
    public GameObject entryBox;
    public GameObject returnButton;
    public Text scoreText;
    private bool newScore;

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameEye(Clone)").GetComponent<Singleton>();
        Init();
        ResetText();
    }

    // Update is called once per frame
    void Update()
    {
        if (newScore && Input.GetKeyDown(KeyCode.Return))
        {
            SetScore();
        }
    }

    public void Init()
    {
        if (gameData.Score > 0)
        {
            newScore = true;
            entryBox.SetActive(true);
            returnButton.SetActive(false);
        }
        else
        {
            newScore = false;
            returnButton.SetActive(true);
            entryBox.SetActive(false);
        }
    }

    public void Return()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void SetScore()
    {
        string playerInitials = entryBox.GetComponent<InputField>().text;
        int total = gameData.Score;

        int[] scoreList = new int[3];
        scoreList[0] = PlayerPrefs.GetInt("TopScore", 0);
        scoreList[1] = PlayerPrefs.GetInt("MidScore", 0);
        scoreList[2] = PlayerPrefs.GetInt("BotScore", 0);

        string[] nameList = new string[3];
        nameList[0] = PlayerPrefs.GetString("TopName", "ABC");
        nameList[1] = PlayerPrefs.GetString("MidName", "DEF");
        nameList[2] = PlayerPrefs.GetString("BotName", "GHI");

        int recordIndex = -1;
        for (int i = 2; i >= 0; i--)
        {
            if (total > scoreList[i])
            {
                recordIndex = i;
            }
        }

        if (recordIndex >= 0)
        {
            switch (recordIndex)
            {
                case 0:
                    PlayerPrefs.SetInt("TopScore", total);
                    PlayerPrefs.SetString("TopName", playerInitials);
                    PlayerPrefs.SetInt("MidScore", scoreList[0]);
                    PlayerPrefs.SetString("MidName", nameList[0]);
                    PlayerPrefs.SetInt("BotScore", scoreList[1]);
                    PlayerPrefs.SetString("BotName", nameList[1]);
                    break;
                case 1:
                    PlayerPrefs.SetInt("MidScore", total);
                    PlayerPrefs.SetString("MidName", playerInitials);
                    PlayerPrefs.SetInt("BotScore", scoreList[1]);
                    PlayerPrefs.SetString("BotName", nameList[1]);
                    break;
                case 2:
                    PlayerPrefs.SetInt("BotScore", total);
                    PlayerPrefs.SetString("BotName", playerInitials);
                    break;
            }
        }
        gameData.Score = 0;
        newScore = false;
        returnButton.SetActive(true);
        entryBox.SetActive(false);
        ResetText();
    }

    public void ResetText()
    {
        string topScore = PlayerPrefs.GetInt("TopScore", 0).ToString("D5");
        string midScore = PlayerPrefs.GetInt("MidScore", 0).ToString("D5");
        string botScore = PlayerPrefs.GetInt("BotScore", 0).ToString("D5");

        string topName = PlayerPrefs.GetString("TopName", "ABC");
        string midName = PlayerPrefs.GetString("MidName", "DEF");
        string botName = PlayerPrefs.GetString("BotName", "GHI");

        scoreText.text = topName + " == " + topScore + "\n" + midName + " == " + midScore + "\n" + botName + " == " + botScore;
    }
}
