using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public SimpleEvent OnInit;
    public ObjEvent OnShow;
    public SimpleEvent OnClose;

    float timer = 0;

    public virtual void Init() { OnInit?.Invoke(); }

    public void Show(params object[] args)
    {
        gameObject.SetActive(true);
        OnShow?.Invoke(args);
    }

    public void Close(float closeTime = 0)
    {
        OnClose?.Invoke();
        if (closeTime != 0)
        {
            StartCoroutine(ClosePanel(closeTime));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator ClosePanel(float t)
    {
        while (timer < t)
        {
            timer += Time.deltaTime;
            yield return 0;
        }
        timer = 0;
        gameObject.SetActive(false);
    }

}
