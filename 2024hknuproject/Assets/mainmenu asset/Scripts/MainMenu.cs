using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void OpenOptions()
    {
        // 옵션 메뉴를 여는 로직
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}