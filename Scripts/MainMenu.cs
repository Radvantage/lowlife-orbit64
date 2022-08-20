using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public GameObject gameEye;
    public AudioMixer audioM;
    public Slider volumeSlider;
    public Slider fovSlider;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;

    Resolution[] resOptions;

    public Image omLogo;
    public Image flash;
    public Image shadow;

    public GameObject mainButtons;
    public GameObject settingsButtons;
    public GameObject sideButtons;
    public GameObject creditsBox;

    private Color logoColor;
    private Color bgColor;
    private Color bgbColor;

    private float timer;
    private bool booting;

    void Start()
    {
        if (FindObjectsOfType<Singleton>().Length < 1)
        {
            Instantiate(gameEye, new Vector3(), transform.rotation);
        }

        PopulateResolutions();
        LoadPref();

        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        booting = true;
        timer = 0;
        logoColor = omLogo.color;
        bgColor = flash.color;

        logoColor.a = 0;
        omLogo.color = logoColor;

        bgbColor = shadow.color;
    }

    void Update()
    {
        //Bootup Foreground
        if (timer < 22.1f)
        {
            timer += Time.deltaTime;
            
            //Logo Appear
            if (timer > 1.5f && timer < 8f)
            {
                logoColor.a = Mathf.Lerp(logoColor.a, 1, 0.5f*Time.deltaTime);
                omLogo.color = logoColor;
                omLogo.gameObject.transform.localScale *= (1.005f);
            }
            //Logo Disappear
            if (timer > 8f && timer < 12f)
            {
                logoColor.a = Mathf.Lerp(logoColor.a, 0, 2f*Time.deltaTime);
                omLogo.gameObject.transform.Rotate(0f, 90f * Time.deltaTime, 0f, Space.Self);
                omLogo.color = logoColor;
            }

            //Flash Out
            if (timer > 13f && timer < 18f)
            {
                bgColor.a = Mathf.Lerp(bgColor.a, 0, Time.deltaTime);
                flash.color = bgColor;
            }
            
            //Dissolve
            if (timer > 20.25f & timer < 22f)
            {
                bgbColor.a = Mathf.Lerp(bgbColor.a, 0, 4f*Time.deltaTime);
                shadow.color = bgbColor;
            }
        }
        if (booting)
        {
            if (bgbColor.a < 0.01f || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                booting = false;
                timer = 30f;
                omLogo.gameObject.SetActive(false);
                flash.gameObject.SetActive(false);
                shadow.gameObject.SetActive(false);
            }
        }

        //Key Codes - Activate Methods
        if (Input.GetKeyDown(KeyCode.E))
        {
            Enter();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            HighScores();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Quit();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Settings();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Credits();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnFromSettings();
        }
    }

    public void Enter()
    {
        SceneManager.LoadScene("Arena", LoadSceneMode.Single);
    }

    public void Settings()
    {
        mainButtons.SetActive(false);
        settingsButtons.SetActive(true);
    }

    public void ReturnFromSettings()
    {
        settingsButtons.SetActive(false);
        mainButtons.SetActive(true);
    }

    private bool runningCredits;
    public void Credits()
    {
        if (!runningCredits)
        {
            Color tempC = Color.black;
            tempC.a = 1;
            shadow.color = tempC;
            StartCoroutine(CreditsTimed());
        }
    }

    private IEnumerator CreditsTimed()
    {
        runningCredits = true;
        shadow.gameObject.SetActive(true);
        creditsBox.SetActive(true);
        yield return new WaitForSeconds(5f);
        shadow.gameObject.SetActive(false);
        creditsBox.SetActive(false);
        runningCredits = false;
    }

    public void HighScores()
    {
        SceneManager.LoadScene("ScorePage", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetVolume(float value)
    {
        value *= 10f;
        audioM.SetFloat("level", value);
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }

    public void SetResolution(int index)
    {
        Resolution r = resOptions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        int full = 0;
        switch (Screen.fullScreen)
        {
            case true:
                full = 1;
                break;
            case false:
                full = 0;
                break;
        }
        
        PlayerPrefs.SetInt("Fullscreen", full);
        PlayerPrefs.Save();
    }

    private int view = 100;
    public void SetFOV(float value)
    {
        view = (int) Mathf.Round(value);
        PlayerPrefs.SetInt("FOV", view);
        PlayerPrefs.Save();
    }

    private void PopulateResolutions()
    {
        resOptions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resLabels = new List<string>();

        for (int i = 0; i < resOptions.Length; i++)
        {
            string opt = resOptions[i].width + "x" + resOptions[i].height + "; " + Mathf.Round(resOptions[i].refreshRate) + "HZ";
            resLabels.Add(opt);
        }

        resolutionDropdown.AddOptions(resLabels);
    }

    void LoadPref()
    {
        //Fullscreen / Windowed
        bool isFull = true;
        switch (PlayerPrefs.GetInt("Fullscreen", 1))
        {
            case 1:
                isFull = true;
                break;
            case 0:
                isFull = false;
                break;
        }
        SetFullscreen(isFull);
        fullscreenToggle.isOn = Screen.fullScreen;

        //Volume Levels
        float savedVolume = (PlayerPrefs.GetFloat("Volume", 1)) / 10f;
        SetVolume(savedVolume);
        volumeSlider.value = savedVolume;
        
        //Resolution
        int currentSizeI = PlayerPrefs.GetInt("ResolutionIndex", 0);
        SetResolution(currentSizeI);
        resolutionDropdown.value = currentSizeI;
        resolutionDropdown.RefreshShownValue();
        
        //Field-of-View
        float view = PlayerPrefs.GetInt("FOV", 100);
        SetFOV(view);
        fovSlider.value = view;
    }
}
