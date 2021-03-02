using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float hurtAmount;
    public SimpleEvent OnIdle;
    public SimpleEvent OnEnter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var h = collision.GetComponent<IHurtable>();
        if (h != null)
        {
            OnEnter?.Invoke();

            h.SetHurtInfo(new object[2]
            {
                (Vector2)transform.position,
                hurtAmount
            });
        }
    }
}
