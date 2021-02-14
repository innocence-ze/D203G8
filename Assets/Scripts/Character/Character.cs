using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] Vec2Event OnHurt;
    [SerializeField] Vec2Event OnAttack;

    [Header("HP--血量")]
    public float maxHp;
    public float curHp;
    public float recoverHp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
