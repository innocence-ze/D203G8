using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public SimpleEvent OnInit;
    public ObjEvent OnShow;
    public SimpleEvent OnClose;

    public void Init()
    {
        transform.parent = GameManager.Singleton.uiMgr.canvas;
    }

    public void Close()
    {
        GameManager.Singleton.uiMgr.Close(GetType().ToString());
    }

}
