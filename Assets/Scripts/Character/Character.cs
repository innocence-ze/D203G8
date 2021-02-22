using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] Vec2Event OnHurt;
    [SerializeField] SimpleEvent OnDie;

    [Header("HP--血量")]
    public float maxHp;
    public float curHp;
    public float recoverHp;
    public bool IsAlive { get { return curHp > 0; } set { IsAlive = value; } }


    // Start is called before the first frame update
    void Start()
    {
        curHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHurt(float hurt)
    {
        if(curHp > hurt)
        {
            curHp -= hurt;
        }
        else
        {
            curHp = 0;
        }
    }

}
