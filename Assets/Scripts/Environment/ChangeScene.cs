using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public int sceneIndex;

    public SimpleEvent OnChangeScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == GameManager.Singleton.pc.gameObject)
        {
            GameManager.Singleton.pc.SetData();
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
