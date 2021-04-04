using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkableNpc : MonoBehaviour
{
    public List<DialogData> data = new List<DialogData>();
    public GameObject Tips;


    //玩家是否进入trigger
    protected bool playerInside = false;
    //是否在一个对话中
    public bool finishTalk = true;
    //data有多个时的index
    protected int talkTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //开始对话

    }

    public void OnChildTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Singleton.pc.gameObject)
        {
            playerInside = true;
        }
    }

    public void OnChildTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Singleton.pc.gameObject)
        {
            playerInside = false;
        }
    }

    public void OnChildTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Singleton.pc.gameObject)
        {
            playerInside = true;
        }
    }
}
