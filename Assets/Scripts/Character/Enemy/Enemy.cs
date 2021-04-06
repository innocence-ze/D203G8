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
    public float attackInterval;//两次攻击的间隔时间
    public Transform attackPos;
    public LayerMask attackableLayer;

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
        if (velocityX * face < 0)
        {
            face = -face;
            onChangeDir?.Invoke(new Vector2(face,0));
        }
    }

    Vector3 deltaPos { get { return target.position - transform.position; } }

    public bool ChaseCondition => face * deltaPos.x <= detectiveRange && face * deltaPos.x > 0 && Mathf.Abs(deltaPos.y) < detectiveRange;
    public bool AttackCondition => canAttack && attackable && Physics2D.OverlapCircle(attackPos.position, attackRange, attackableLayer);
    public bool BackCondition => transform.position.x < chaseLeftBoundary || transform.position.x > chaseRightBoundary || (Mathf.Abs(deltaPos.x) > detectiveRange || Mathf.Abs(deltaPos.y) > detectiveRange);
    public bool HurtCondition => getHurt && !invincible;
    protected IEnumerator AttackIntervalTimer()
    {
        attackable = false;
        yield return new WaitForSeconds(attackInterval);
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
