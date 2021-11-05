using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public static bool isPaused, isFirst;
    public static bool inDialogue, inBasement;
    public GameObject pauseMenuUI, dialogUI1, playUI, deathUI, victoryUI;
    public GameObject mainCamera, kenPlayMode;
    public Camera dialogueCam1;

    public TMP_Text timeStampText;

    public void PauseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pauseMenuUI.SetActive(true);
        playUI.SetActive(false);

        Time.timeScale = 0f;
        

        isPaused = true;
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log(Cursor.lockState);

        pauseMenuUI.SetActive(false);
        playUI.SetActive(true);

        Time.timeScale = 1f;
        
        isPaused = false;
    }

    public void DeathGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pauseMenuUI.SetActive(false);
        playUI.SetActive(false);
        deathUI.SetActive(true);

        Time.timeScale = 0f;


        isPaused = true;
    }

    public void WinGame()
    {
        pauseMenuUI.SetActive(false);
        playUI.SetActive(false);
        victoryUI.SetActive(true);

        Time.timeScale = 0f;

        timeStampText.text = "Finished in " + Stopwatch.minute.ToString("00") + ":" + Stopwatch.seconds.ToString("00");
    }

    public void RedirectMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
    }

    private bool activateCam = false;

    private void Update()
    {
        if (inDialogue)
        {
            dialogUI1.SetActive(true);
            playUI.SetActive(false);
            dialogueCam1.enabled = true;
            mainCamera.SetActive(false);
            kenPlayMode.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            dialogUI1.SetActive(false);
            playUI.SetActive(true);
            dialogueCam1.enabled = false;
            mainCamera.SetActive(true);
            if (!activateCam)
            {
                mainCamera.SetActive(true);
                activateCam = true;
            }
            kenPlayMode.SetActive(true);

            if(isFirst)
            {
                isFirst = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            } 
            else if (Player.isDead)
            {
                DeathGame();
            }
        }
    }

    private void Start()
    {
        inDialogue = true;
        isFirst = true;
        isPaused = false;
        inBasement = false;
    }
}
