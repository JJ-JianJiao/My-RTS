using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D selectCursor;

    private void Awake()
    {
        
        Cursor.SetCursor(selectCursor, new Vector2(16, 16), CursorMode.Auto);
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public void StartBtnOnClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitBtnOnClick() {
        Application.Quit();
    }
}
