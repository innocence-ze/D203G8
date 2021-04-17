using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcEnemy : Enemy
{
    public enum NpcEnemyState
    {
        Patrol,
        Chase,
        Attack,
        Back,
        Recover,
        Hurt,
        Die,
    }

    public NpcEnemyState curState;

    bool hurted = false;
    [SerializeField] bool attacking = false;

    //巡逻状态，朝着巡逻点数组的下一个地点移动
    protected IEnumerator PatrolState()
    {
        curState = NpcEnemyState.Patrol;
        onPatrol?.Invoke();
        while (true)
        {

            var dir = patrolPos[targetIndex] - transform.position.x;
            SetVelocityX(patrolSpeed, dir);
            if (dir < 0.1f && dir > -0.1f)
            {
                targetIndex = (targetIndex + 1) % patrolPos.Count;
            }
            yield return continueState;

            if (ChaseCondition)
            {
                StartCoroutine(ChaseState());
                yield break;
            }

            if (HurtCondition)
            {
                StartCoroutine(HurtState());
                yield break;
            }

        }
    }

    //追逐状态，追逐玩家，如果长时间没追到则放弃
    protected IEnumerator ChaseState()
    {
        curState = NpcEnemyState.Chase;
        onChase?.Invoke();
        while (true)
        {
            var detx = target.position.x - transform.position.x;
            if (Mathf.Abs(detx) > 0.2f)
            {
                SetVelocityX(chaseSpeed, detx);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            yield return continueState;

            if (AttackCondition)
            {
                StartCoroutine(AttackState());
                yield break;
            }
            if (BackCondition)
            {
                StartCoroutine(BackState());
                yield break;
            }
            if (HurtCondition)
            {
                StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    //攻击玩家一次
    protected IEnumerator AttackState()
    {
        attacking = true;
        curState = NpcEnemyState.Attack;
        onAttack?.Invoke();
        rb.velocity = Vector2.zero;
        //InAttack();
        while (true)
        {
            yield return continueState;
            if (!attacking)
            {
                StartCoroutine(AttackIntervalTimer());
                if (BackCondition)
                {
                    StartCoroutine(BackState());
                    yield break;
                }
                else
                {
                    StartCoroutine(ChaseState());
                    yield break;
                }
            }
            if (HurtCondition)
            {
                StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    //没追到玩家后返回继续巡逻
    protected IEnumerator BackState()
    {
        curState = NpcEnemyState.Back;
        invincible = true;
        onBack?.Invoke();
        while (true)
        {
            SetVelocityX(patrolSpeed, bornPos.x - transform.position.x);
            yield return continueState;
            if (Mathf.Abs(bornPos.x - transform.position.x) < 0.05f)
            {
                invincible = false;
                StartCoroutine(PatrolState());
                yield break;
            }

            if (HurtCondition)
            {
                StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    protected IEnumerator HurtState()
    {
        getHurt = false;
        curState = NpcEnemyState.Hurt;
        onHurt?.Invoke((Vector2)hurtInfo[0] - (Vector2)transform.position);
        GetHurt((float)hurtInfo[1]);
        StartCoroutine(InvincibleCoroutine());
        while (true)
        {
            yield return continueState;
            if (!IsAlive)
            {
                StartCoroutine(DieState());
                yield break;
            }
            else if (hurted)
            {
                hurted = false;
                StartCoroutine(ChaseState());
                yield break;
            }
        }
    }

    IEnumerator DieState()
    {
        curState = NpcEnemyState.Die;
        onDie?.Invoke();
        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        while (true)
        {
            yield return continueState;
            if (IsAlive)
            {
                GetComponent<Collider2D>().enabled = true;
                transform.position = bornPos;
                StartCoroutine(PatrolState());
                yield break;
            }
        }
    }

    public void FinishHurted()
    {
        hurted = true;
    }

    public void FinishAttacked()
    {
        attacking = false;
    }

    public virtual void InAttack()
    {

    }

    public void SetVelocityX(float maxSpeed, float dir)
    {
        velocityX = dir > 0 ? maxSpeed : -maxSpeed;
        rb.velocity = velocityX * Vector2.right;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector2(patrolPos[0], transform.position.y + 0.3f), new Vector2(patrolPos[1], transform.position.y + 0.3f));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(chaseLeftBoundary, transform.position.y), new Vector2(chaseRightBoundary, transform.position.y));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector2(detectiveRange * 2, detectiveRange * 2));
        Gizmos.color = Color.red;

    }

}
