using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FunctionUI : MonoBehaviour
{
    public int mainMenuIndex = 26;

    public void ReturnMainMenu()
    {
        var p = GameManager.Singleton.pc.transform.position;

        GameManager.Singleton.pc.SaveData(SceneManager.GetActiveScene().buildIndex, p.x, p.y, p.z);
        StartCoroutine(LoadAsyncScreen());
    }

    IEnumerator LoadAsyncScreen()
    {
        AsyncOperation asyncLoad;
        asyncLoad = SceneManager.LoadSceneAsync(mainMenuIndex);

  
        while (!asyncLoad.isDone)
        {
            yield return 0;
        }
    }

    public void AutoDeath()
    {
        GameManager.Singleton.pc.SetHurtInfo(new object[2] { new Vector2(1,1), GameManager.Singleton.pc.maxHp });
    }
}
