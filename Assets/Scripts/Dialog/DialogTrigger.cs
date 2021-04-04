using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    bool firstTime = true;

    public DialogData data;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Singleton.pc.gameObject && firstTime && GameManager.Singleton.dialogMgr.state == DialogManager.State.off)
        {
            firstTime = false;
            GameManager.Singleton.dialogMgr.StartDialog(data);
        }
    }
}
