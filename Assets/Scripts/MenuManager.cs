using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene("Level_1_blockout_Sam");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
