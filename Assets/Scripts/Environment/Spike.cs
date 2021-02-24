using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float hurtAmount;
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
            object[] o = new object[2];
            o[0] = (Vector2)transform.position;
            o[1] = hurtAmount;

            h.SetHurtInfo(o);
        }
    }
}
