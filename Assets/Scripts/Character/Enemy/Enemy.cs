using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    protected Transform target;
    [Header("Patrol--巡逻")]
    public float patrolSpeed;
    public List<float> patrolPos = new List<float>();
    protected int targetIndex = 0;
    [Header("Chase--追击")]
    public float detectiveRange;
    public float chaseSpeed;
    public float chaseLeftBoundary;
    public float chaseRightBoundary;

    [Header("Attack--攻击")]
    public bool canAttack = true;
    public bool attackable = true;
    public float attackRange;
    public float afterAttackFreezeTime;//攻击后的僵硬时间
    public float attackInterval;//两次攻击的间隔时间
    protected bool inAttack;
    public Transform attackPos;

    [Header("Event")]
    public SimpleEvent onPatrol;
    public SimpleEvent onChase;
    public SimpleEvent onAttack;
    public Vec2Event OnAttackObj;
    public SimpleEvent onBack;
    public SimpleEvent onRecover;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<PlayerCharacter>().transform;
        onChangeDir.AddListener(ChangeAttackPos);
    }

    protected override void Update()
    {
        base.Update();
        if (rb.velocity.x * face < 0)
        {
            face = -face;
            onChangeDir?.Invoke(new Vector2(face,0));
        }
    }

    public bool ChaseCondition => face * (target.position.x - transform.position.x) <= detectiveRange && face * (target.position.x - transform.position.x) > 0 && Mathf.Abs(target.position.y - transform.position.y) < detectiveRange;
    public bool AttackCondition => canAttack && attackable && Mathf.Abs(target.position.x - transform.position.x) <= attackRange && Mathf.Abs(target.position.y - transform.position.y) <= attackRange;
    public bool BackCondition => transform.position.x < chaseLeftBoundary || transform.position.x > chaseRightBoundary || Mathf.Abs(target.position.x - transform.position.x) > detectiveRange;
    public bool HurtCondition => getHurt && !invincible;
    protected IEnumerator AttackIntervalTimer()
    {
        attackable = false;
        inAttack = true;
        yield return new WaitForSeconds(afterAttackFreezeTime);
        inAttack = false;
        yield return new WaitForSeconds(attackInterval - afterAttackFreezeTime);
        attackable = true;
    }

    //根据朝向变更攻击位置
    public void ChangeAttackPos(Vector2 dir)
    {
        if(dir.x * attackPos.localPosition.x < 0)
        {
            attackPos.localPosition = new Vector3(-attackPos.localPosition.x, attackPos.localPosition.y, attackPos.localPosition.z);
        }
    }

}
