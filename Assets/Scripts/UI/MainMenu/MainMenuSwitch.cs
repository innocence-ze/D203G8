using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuSwitch : MonoBehaviour
{
    public PCData init, cur;

    public List<GameObject> panels;
    List<bool> panelState = new List<bool>();

    public float bgMove, bgScale, bgTime;

    public int startSceneIndex;

    public void Close(int panelIndex)
    {
        Graphic[] graphic = panels[panelIndex].GetComponentsInChildren<Graphic>(true);
        for(int i = 0; i < graphic.Length; i++)
        {
            var g = graphic[i];
            g.transform.DORotate(new Vector3(90, 0, 0), 0.2f).OnComplete(()=>g.gameObject.SetActive(false));
        }
    }

    public void Open(int panelIndex)
    {
        Graphic[] graphic = panels[panelIndex].GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphic.Length; i++)
        {
            graphic[i].gameObject.SetActive(true);
            graphic[i].transform.DORotate(new Vector3(0, 0, 0), 0.2f);
        }
    }

    public void OpenPanel(int panelIndex)
    {
        for(int i = 0; i < panelState.Count; i++)
        {
            if (panelState[i])
            {
                panelState[i] = false;
                Close(i);
                break;
            }
        }

        panelState[panelIndex] = true;
        Open(panelIndex);
    }

    public void BeginGame(bool bstart)
    {
        if(bstart)
        {
            cur.Copy(init);
        }    
        transform.GetChild(0).DOMoveY(bgMove, bgTime);
        transform.GetChild(0).DOScale(bgScale, bgTime).OnComplete(()=>UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(cur.continueSceneIndex));
    }

    public void Quit()
    {
        transform.GetChild(0).DOScale(0.001f, 0.5f).OnComplete(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }


    void Start()
    {
        panelState.Add(true);
        for(int i = 1; i < panels.Count; i++)
        {
            Close(i);
            panelState.Add(false);
        }
    }

}
