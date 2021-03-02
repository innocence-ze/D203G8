using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    protected Queue<GameObject> queue;

    //初始分配数量与增加数量
    public int preAllocCount;
    public int autoIncreaseCount;

    //池中对象及名称
    public GameObject prefab;
    public string objTypeString;

    public float autoRecycleTime;


    public virtual void Init(ObjectPoolMgr.AllocObj obj)
    {
        prefab = obj.prefab;
        preAllocCount = obj.preAllocSize;
        autoIncreaseCount = obj.autoIncreaseCount;
        objTypeString = obj.type;
        autoRecycleTime = obj.autoRecycleTime;

        queue = new Queue<GameObject>(preAllocCount);
        for(int i =0; i < preAllocCount; i++)
        {
            CreateNewObj();
        }
    }

    public virtual void Recycle(GameObject obj)
    {
        if (queue.Contains(obj))
        {
            Debug.LogWarning("The object" + obj.name + "has been recycled");
            return;
        }

        queue.Enqueue(obj);
        obj.SetActive(false);
        obj.transform.parent = transform;

    }

    public virtual GameObject Utilize()
    {
        if(queue.Count <= 0)
        {
            for (int i = 0; i < autoIncreaseCount; i++)
            {
                CreateNewObj();
            }
        }

        var returnGo = queue.Dequeue();
        returnGo.SetActive(true);
        return returnGo;
    }

    void CreateNewObj()
    {
        var go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        go.SetActive(false);
        go.transform.parent = transform;
        var preInfo = go.AddComponent<PreInfo>();
        preInfo.type = objTypeString;
        preInfo.lifeTime = autoRecycleTime;
        queue.Enqueue(go);
    }
}
