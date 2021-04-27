using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//通过mgr来和外部交互
public class UIManager : MonoBehaviour
{

    public Transform canvas;

    public List<GameObject> panels = new List<GameObject>();

    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    public void Init()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            var p = panels[i].GetComponent<BasePanel>();
            p.Init();
            panels[i].SetActive(false);
        }
    }

    public T ShowPanel<T>(params object[] args) where T : BasePanel
    {
        string name = typeof(T).ToString();
        if (panelDic.ContainsKey(name))
        {
            return null;
        }

        T curPanel = GetComponentInChildren<T>(true);

        curPanel.Show(args);

        panelDic.Add(name, curPanel);

        return panelDic[name] as T;
    }

    public void ClosePanel(string name, float closeTime = 0)
    {
        if (!panelDic.ContainsKey(name))
        {
            return;
        }

        var panel = panelDic[name];
        if (panel == null)
        {
            return;
        }

        panel.Close(closeTime);
        panelDic.Remove(name);

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
