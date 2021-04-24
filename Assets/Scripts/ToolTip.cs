using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class ToolTip : MonoBehaviour
{
    public GameObject myPanel;
    public Text myText;
    public string tooltipText;
    private bool entered;

    private void Start()
    {
        myPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (!entered)
            {
                myPanel.SetActive(true);
                myText.text = tooltipText;
                entered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            myPanel.SetActive(false);
        }
    }
}
