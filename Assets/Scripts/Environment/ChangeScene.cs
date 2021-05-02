using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public int sceneIndex;

    public SimpleEvent OnChangeScene;

    private void Awake()
    {
        //因为没有考虑到mainmenu
        sceneIndex += 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject == GameManager.Singleton.pc.gameObject)
        {
            GameManager.Singleton.pc.SaveData(sceneIndex);
            OnChangeScene?.Invoke();
            StartCoroutine(LoadAsyncScreen());
        }
    }

    IEnumerator LoadAsyncScreen()
    {
        AsyncOperation asyncLoad;
        asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);


        while (!asyncLoad.isDone)
        {
            yield return 0;
        }
    }
}
