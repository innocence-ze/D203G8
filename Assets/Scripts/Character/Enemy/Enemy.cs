using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    protected Transform target;
    [Header("Patrol--巡逻")]
    public float patrolSpeed;
    public List<Vector3> patrolPos = new List<Vector3>();
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
    public float afterAttackFreezeTime;
    public float attackInterval;
    protected bool inAttack;

    [Header("Event")]
    public SimpleEvent onPatrol;
    public SimpleEvent onChase;
    public SimpleEvent onAttack;
    public Vec2Event OnAttackObj;
    public SimpleEvent onBack;
    public SimpleEvent onRecover;
    public FloatEvent onChangeDir;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<PlayerCharacter>().transform;
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

}
