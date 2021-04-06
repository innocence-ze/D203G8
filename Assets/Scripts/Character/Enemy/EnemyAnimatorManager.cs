using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : MonoBehaviour
{
    NpcEnemy enemy;
    Animator anim;
    SpriteRenderer sr;

    public string isHurt;
    public string isChase;
    public string isRecover;
    public string isPatrol;
    public string isBack;
    public string isAlive;
    public string isAttack;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<NpcEnemy>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool(isHurt, enemy.curState == NpcEnemy.NpcEnemyState.Hurt);
        anim.SetBool(isChase, enemy.curState == NpcEnemy.NpcEnemyState.Chase);
        anim.SetBool(isRecover, enemy.curState == NpcEnemy.NpcEnemyState.Recover);
        anim.SetBool(isPatrol, enemy.curState == NpcEnemy.NpcEnemyState.Patrol);
        anim.SetBool(isBack, enemy.curState == NpcEnemy.NpcEnemyState.Back);
        anim.SetBool(isAlive, enemy.IsAlive);
        anim.SetBool(isAttack, enemy.curState == NpcEnemy.NpcEnemyState.Attack);

        sr.flipX = enemy.face == -1;
    }
}
