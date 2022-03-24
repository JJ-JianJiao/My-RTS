using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;


    public GameObject PauseMenu;
    public GameObject tutorialOrNotPanel;
    public bool isGamePause;

    public GameObject PauseTitle;
    public GameObject WinTitle;
    public GameObject LostTitle;
    public GameObject ContinueBtn;
    public GameObject PlayAgainBtn;

    public GameObject redDragon;


    public int killNumMin = 2;

    private bool AutoActivePanel;

    private int gameState; //0: not finish, 1:win, -1:lost
    public AudioSource victoryAS;
    public List<AudioClip> loseWinClips;

    private void Awake()
    {
        instance = this;
        isGamePause = false;
        AutoActivePanel = false;
        gameState = 0;
        victoryAS.ignoreListenerPause = true;
    }



    private void Update()
    {
        if (redDragon.GetComponent<Health>().IsUnitDie() && !AutoActivePanel)
        {
            AutoActivePanel = true;
            gameState = 1;
            Invoke("DisPlayWinPanel", 6f);
        }
        else if (redDragon.GetComponent<RedDragon>().GetKilledNum() >= 12)
        {
            //else if (redDragon.GetComponent<RedDragon>().GetKilledNum() >= 2 && !AutoActivePanel) {
            AutoActivePanel = true;
            gameState = -1;
            Invoke("DisPlayLostPanel", 3f);
        }

        if (Input.GetKeyUp(KeyCode.Escape) && !tutorialOrNotPanel.activeInHierarchy && gameState != -1) {
            if (!PauseMenu.activeInHierarchy)
                ActivePauseMenu();
            else
                //Invoke("InactivePauseMenu", 0.2f);
                InactivePauseMenu();
        }

        if (isGamePause && PauseMenu.activeInHierarchy) {
            if (Input.GetKeyDown(KeyCode.C)){
                ResumeBtnOnClick();
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                ReturnToMainBtnOnClick();
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                QuitGameBtnOnClick();
            }
        }
        
    }


    private void ActivePauseMenu(int activeType = 0)
    {
        if(activeType == 0)
            SoundManager.instance.PlayGamePauseAS();
        else if(activeType == 1){ 
        
        }
        else{ 
        
        }
        PauseMenu.SetActive(true);
        PauseGameState();
        isGamePause = true;
    }


    private void InactivePauseMenu() {
        if(gameState != -1)
            SoundManager.instance.PlayGameResumeAS();
        PauseMenu.SetActive(false);
        ResumeGameState();
        isGamePause = false;

        if (gameState == 1) {
            gameState = 0;
            WinTitle.gameObject.SetActive(false);
            PauseTitle.gameObject.SetActive(true);
        }
    }

    public void ResumeBtnOnClick() {
        Time.timeScale = 1;
        Invoke("InactivePauseMenu", 0.03f);
        //InactivePauseMenu();
    }

    public void ReturnToMainBtnOnClick() {
        //ResumeGameState();
        SceneManager.LoadScene(0);
    }

    public void QuitGameBtnOnClick()
    {
        Application.Quit();
    }

    private void PauseGameState() {
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    private void ResumeGameState() {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    private void DisPlayWinPanel() {
        victoryAS.clip = loseWinClips[0];
        victoryAS.Play();
        ActivePauseMenu(1);
        PauseTitle.gameObject.SetActive(false);
        WinTitle.gameObject.SetActive(true);
    }

    private void DisPlayLostPanel()
    {
        victoryAS.clip = loseWinClips[1];
        victoryAS.Play();
        ActivePauseMenu(-1);
        PauseTitle.gameObject.SetActive(false);
        WinTitle.gameObject.SetActive(false);
        LostTitle.gameObject.SetActive(true);
        PlayAgainBtn.gameObject.SetActive(true);
        ContinueBtn.gameObject.SetActive(false);
    }

    public void PlayAgainBtnOnClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGameState();
    }

    public void WeakBossTroggle(bool value) {
        if (redDragon != null && !redDragon.GetComponent<Health>().IsUnitDie())
        {
            if (value)
            {
                redDragon.GetComponent<Health>().currentHealth = 200;
                Debug.Log("on");
            }
            else
            {
                redDragon.GetComponent<Health>().currentHealth = redDragon.GetComponent<Health>().fullHealth;
                Debug.Log("off");
            }
        }
    }

}
