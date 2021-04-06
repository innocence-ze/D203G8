using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO,使用对象池优化
public class RangedEnemy : NpcEnemy
{
    public GameObject Bullet;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(PatrolState());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void InAttack()
    {
        Instantiate(Bullet, attackPos.position, Quaternion.identity);
        Bullet.GetComponent<EnemyBullet>().direction =  (target.position - transform.position).normalized;
    }
}
