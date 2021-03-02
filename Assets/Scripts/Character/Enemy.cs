using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    protected Transform target;
    public float face = 1;//right 1; left -1 
    [Header("Patrol--巡逻")]
    public float moveSpeed;
    public List<Vector3> partolPos = new List<Vector3>();
    protected int targetIndex = 0;
    [Header("Detect--侦探")]
    public float detectiveRange;
    [Header("Chase--追击")]
    public List<Vector3> maxChasePos = new List<Vector3>();
    public float chaseSpeed;
    [Header("Attack--攻击")]
    public bool canAttack;
    public bool attackable;
    public float attackRange;
    public float attackInterval;

    [Header("Event")]
    public SimpleEvent OnPatrol;
    public SimpleEvent OnDetect;
    public SimpleEvent OnChase;
    public SimpleEvent OnAttack;
    public FloatEvent onChangeDir;

    protected override void Start()
    {
        base.Start();
        target = GetComponent<PlayerCharacter>().transform;
    }

    public bool ChaseCondition=> Vector3.SqrMagnitude(target.position - transform.position) <= detectiveRange * detectiveRange;
    protected IEnumerator AfterAttack()
    {
        attackable = false;
        yield return new WaitForSeconds(attackInterval);
        attackable = true;
    }

}
