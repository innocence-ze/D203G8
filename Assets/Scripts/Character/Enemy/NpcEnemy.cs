﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcEnemy : Enemy
{
    public enum NpcEnemyState
    {
        patrol,
        chase,
        attack,
        back,
        recover,
        hurt,
        die,
    }

    public NpcEnemyState state;

    //巡逻状态，朝着巡逻点数组的下一个地点移动
    protected IEnumerator PatrolState()
    {
        state = NpcEnemyState.patrol;
        onPatrol?.Invoke();
        while (true)
        {
            var dir = patrolPos[targetIndex] - transform.position;
            if (dir.x > 0) { rb.velocity = patrolSpeed * Vector2.right; }
            else { rb.velocity = patrolSpeed * Vector2.left; }
            if (dir.sqrMagnitude < 0.01f)
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
        state = NpcEnemyState.chase;
        onChase?.Invoke();
        while (true)
        {
            var dir = target.position - transform.position;
            if(dir.x > 0) { rb.velocity = chaseSpeed * Vector2.right; }
            else { rb.velocity = chaseSpeed * Vector2.left; }

            yield return continueState;

            if (AttackCondition)
            {
                StartCoroutine(AttackState());
            }
            if (BackCondition)
            {
                StartCoroutine(BackState());
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
        state = NpcEnemyState.attack;
        onAttack?.Invoke();
        InAttack();
        StartCoroutine(AttackIntervalTimer());
        while (true)
        {
            yield return continueState;
            if (!inAttack)
            {
                if (BackCondition)
                {
                    StartCoroutine(BackState());
                    yield break;
                }
                else
                {
                    StartCoroutine(ChaseState());
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
        state = NpcEnemyState.back;
        invincible = true;
        onBack?.Invoke();
        while (true)
        {
            rb.velocity = (bornPos - (Vector2)transform.position).x > 0 ? Vector2.right * patrolSpeed : Vector2.left * patrolSpeed;
            yield return continueState;
            if(Vector3.SqrMagnitude(bornPos - (Vector2)transform.position) < 0.01)
            {
                invincible = false;
                if (curHp < maxHp)
                {
                    StartCoroutine(RecoverState());
                }
                else
                {
                    StartCoroutine(PatrolState());
                }
            }

            if (HurtCondition)
            {
                StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    //在返回后如果残血则恢复血量
    protected IEnumerator RecoverState()
    {
        state = NpcEnemyState.recover;
        onRecover?.Invoke();
        while (true)
        {
            if (curHp < maxHp)
            {
                curHp += Time.deltaTime * recoverHp;
            }
            yield return continueState;
            if (curHp >= maxHp)
            {
                StartCoroutine(PatrolState());
            }
            if (ChaseCondition)
            {
                StartCoroutine(ChaseState());
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
        state = NpcEnemyState.hurt;
        onHurt?.Invoke((Vector2)hurtInfo[0] - (Vector2)transform.position);
        GetHurt((float)hurtInfo[1]);
        StartCoroutine(InvincibleCoroutine());
        yield return new WaitForSeconds(hurtRecoverTime);
        while (true)
        {
            yield return continueState;
            if (!IsAlive)
            {
                StartCoroutine(DieState());
                yield break;
            }
            else
            {
                StartCoroutine(ChaseState());
                yield break;
            }
        }
    }

    IEnumerator DieState()
    {
        state = NpcEnemyState.die;
        onDie?.Invoke();
        rb.velocity = Vector2.zero;

        while (true)
        {
            yield return continueState;
            if (IsAlive)
            {
                transform.position = bornPos;
                StartCoroutine(PatrolState());
                yield break;
            }
        }
    }

    protected virtual void InAttack()
    {

    }

}