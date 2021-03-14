using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : NpcEnemy
{
    public Transform attackPos;
    public LayerMask attackableLayer;
    public float attackDamage;

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

    protected override void InAttack()
    {
        Collider2D[] coll = Physics2D.OverlapCircleAll(attackPos.position, attackRange, attackableLayer);
        for(int i = 0; i < coll.Length; i++)
        {
            OnAttackObj?.Invoke(coll[i].transform.position);
            var pc = coll[i].GetComponent<IHurtable>();
            if (pc != null)
            {
                pc.SetHurtInfo(new object[2]
                {
                    (Vector2)transform.position,
                    attackDamage
                }) ;
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
