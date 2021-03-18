using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrpt_particle : MonoBehaviour
{
    Vector3 direction;
    float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        direction = direction.normalized;
        speed = Random.Range(0.01f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
