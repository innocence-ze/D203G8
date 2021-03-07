using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public SimpleEvent OnIdle;
    public SimpleEvent OnActive;

    public Vector3 respawnPos;

    // Start is called before the first frame update
    void Start()
    {
        OnIdle?.Invoke();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject == GameManager.Singleton.pc.gameObject)
        {
            OnActive?.Invoke();
            GameManager.Singleton.pc.bornPos = respawnPos;
        }
    }

}
