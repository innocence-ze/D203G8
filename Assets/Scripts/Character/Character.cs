using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IHurtable
{
    [SerializeField] protected Vec2Event OnHurt;
    [SerializeField] protected SimpleEvent OnDie;

    [Header("HP--血量")]
    public float maxHp;
    public float curHp;
    public float recoverHp;

    [Header("Hurt--受伤")]
    public object[] hurtInfo = new object[2];//0:攻击者方向, 1:伤害
    public bool invincible;
    public float invincibleTime;
    public float hurtRecoverTime; //硬直
    public Vector2 bornPos;

    public bool IsAlive { get { return curHp > 0; } set { IsAlive = value; } }
    public bool getHurt;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        curHp = maxHp;
        bornPos = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(!invincible && !getHurt && curHp < maxHp)
        {
            curHp += recoverHp * Time.deltaTime;
        }
    }

    protected IEnumerator InvincibleCoroutine()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
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

    public void SetHurtInfo(object[] info)
    {
        if (invincible)
            return;
        getHurt = true;
        info.CopyTo(hurtInfo, 0);
    }
}


public interface IHurtable
{
    void SetHurtInfo(object[] info);
}
