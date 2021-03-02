using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreInfo : MonoBehaviour
{

    public string type;
    public float lifeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(lifeTime > 0)
        {
            StartCoroutine(CountRecycle(lifeTime));
        }
    }

    IEnumerator CountRecycle(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolMgr.Singleton.Recycle(gameObject);
    }

}
