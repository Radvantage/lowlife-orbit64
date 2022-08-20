using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LowlifeOrbit 
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject pauseLogo;
        public GameObject helpBox;
        public GameObject pauseButtons;
        private bool paused;

        void Start()
        {
            pauseMenu.SetActive(false);
            pauseLogo.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (paused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
            if (paused)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Resume();
                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    Help();
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    Exit();
                }
            }
            
        }

        void Pause()
        {
            AudioListener.volume = 0;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            pauseLogo.SetActive(true);
            Time.timeScale = 0;
            paused = true;
        }

        public void Resume()
        {
            AudioListener.volume = 1;
            Cursor.lockState = CursorLockMode.Locked;
            pauseButtons.SetActive(true);
            helpBox.SetActive(false);
            pauseMenu.SetActive(false);
            pauseLogo.SetActive(false);
            Time.timeScale = 1;
            paused = false;
        }

        public void Help()
        {
            helpBox.SetActive(true);
            pauseButtons.SetActive(false);
        }

        public void CloseHelp()
        {
            helpBox.SetActive(false);
            pauseButtons.SetActive(true);
        }

        public void Exit()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        
    }
}