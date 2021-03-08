using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float flySpeed;
    public Vector2 direction;
    public float existTime;
    public float damage;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DeleteBullet());
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = direction * flySpeed;
    }

    //TODO 对象池优化
    IEnumerator DeleteBullet()
    {
        yield return new WaitForSeconds(existTime);
        OnDistroy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerCharacter>().SetHurtInfo(new object[2] { transform.position, damage });
            OnDistroy();
        }
    }

    private void OnDistroy()
    {
        Destroy(gameObject);
    }

}
