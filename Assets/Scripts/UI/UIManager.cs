using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public Transform canvas;

    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    public T Open<T>(params object[] args) where T : BasePanel
    {
        string name = typeof(T).ToString();
        if (panelDic.ContainsKey(name))
        {
            return null;
        }

        T curPanel = ObjectPoolManager.Singleton.Utilize(name).GetComponent<T>();
        curPanel.Init();

        curPanel.OnInit?.Invoke();

        curPanel.OnShow?.Invoke(args);

        panelDic.Add(name, curPanel);

        return panelDic[name] as T;
    }

    public void Close(string name, float closeTime = 0)
    {
        if (!panelDic.ContainsKey(name))
        {
            return;
        }

        var panel = panelDic[name];
        if(panel == null)
        {
            return;
        }


        panel.OnClose?.Invoke();
        panelDic.Remove(name);

        ObjectPoolManager.Singleton.Recycle(panel.gameObject, closeTime);
    }

    public T GetPanel<T>() where T : BasePanel
    {
        var name = typeof(T).ToString();
        if (panelDic.ContainsKey(name))
        {
            return panelDic[name] as T;
        }
        return null;
    }
}
