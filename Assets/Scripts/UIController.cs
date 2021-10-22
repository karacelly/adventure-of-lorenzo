using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static bool isPaused;
    public static bool inDialogue;
    public GameObject pauseMenuUI, dialogUI1, playUI, deathUI;
    public GameObject mainCamera, kenPlayMode, kenDialogueMode;
    public Camera dialogueCam1;

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

    public void RedirectMenu()
    {
        SceneManager.LoadScene(0);
    }

    private bool activateCam = false;

    private void Update()
    {
        if (inDialogue)
        {
            playUI.SetActive(false);
            dialogueCam1.enabled = true;
            mainCamera.SetActive(false);
            kenDialogueMode.SetActive(true);
            kenPlayMode.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            dialogUI1.SetActive(false);
            playUI.SetActive(true);
            dialogueCam1.enabled = false;
            if (!activateCam)
            {
                mainCamera.SetActive(true);
                activateCam = true;
            }
            kenDialogueMode.SetActive(false);
            kenPlayMode.SetActive(true);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

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
        isPaused = false;
    }
}
