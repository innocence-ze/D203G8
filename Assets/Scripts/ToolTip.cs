using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class ToolTip : MonoBehaviour
{
    public string tooltipText;
    private bool entered;

    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (!entered)
            {
                GameManager.Singleton.uiMgr.ShowPanel<ToolTipPanel>((object)tooltipText);
                entered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            GameManager.Singleton.uiMgr.ClosePanel(typeof(ToolTipPanel).ToString());
        }
    }
}
