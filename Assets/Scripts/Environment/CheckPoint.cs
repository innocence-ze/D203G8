using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public SimpleEvent OnIdle;
    public SimpleEvent OnActive;


    // Start is called before the first frame update
    void Start()
    {
        OnActive.AddListener(OnDestory);
        OnIdle?.Invoke();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject == GameManager.Singleton.pc.gameObject)
        {
            GameManager.Singleton.pc.bornPos = transform.position;
            OnActive?.Invoke();
        }
    }


    void OnDestory()
    {
        Destroy(gameObject);
    }
}
