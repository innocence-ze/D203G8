using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolMgr : MonoBehaviour
{

    private static ObjectPoolMgr s_singleton = null;
    public static ObjectPoolMgr Singleton
    {
        get
        {
            if(s_singleton == null)
            {
                s_singleton = FindObjectOfType<ObjectPoolMgr>();
            }
            if(s_singleton == null)
            {
                Debug.LogError("cannot find object pool manager!!");
            }
            return s_singleton;
        }
    }



    private readonly Dictionary<string, ObjectPool> poolDic = new Dictionary<string, ObjectPool>();

    //在inspector面板中中初始化对象池中物体
    public List<AllocObj> objPoolList = new List<AllocObj>();

    //对象池中对象的一些属性
    [System.Serializable]
    public class AllocObj
    {
        public string type;                 //池子类型
        public int preAllocSize;            //池子创建时预申请的对象数量
        public int autoIncreaseCount;       //每次增加的对象数量
        public GameObject prefab;           //对象的应用
        public float autoRecycleTime;       //多久后自动回收
    }

    private void Awake()
    {
        for(int i = 0; i < objPoolList.Count; i++)
        {
            Init(objPoolList[i]);
        }
    }

    /// <summary>
    /// 回收物体进入对应池子
    /// </summary>
    public void Recycle(GameObject recycleObj)
    {
        GetPool(recycleObj.GetComponent<PreInfo>().type).Recycle(recycleObj);
    }

    /// <summary>
    /// 使用对象池中的物体，并返回使用的物体
    /// </summary>
    public GameObject Utilize(string utilizeType)
    {
        return GetPool(utilizeType).Utilize();
    }

    /// <summary>
    /// 初始化对象池，
    /// </summary>
    public void Init(AllocObj alloc)
    {
        ObjectPool subPool = CreatePool(alloc.type);
        subPool.Init(alloc);
    }

    ObjectPool CreatePool(string type)
    {
        string poolNameString = type + "Pool";

        GameObject subPool = new GameObject(poolNameString);
        subPool.transform.parent = transform;
        //创建对象池实例
        System.Type poolType = System.Type.GetType(poolNameString);
        ObjectPool returnPool = subPool.AddComponent(poolType) as ObjectPool;
        //若没有相应池子，则创建一个泛用的池子
        if (returnPool == null)
        {
            returnPool = subPool.AddComponent<CommonPool>();
        }

        returnPool.objTypeString = type;

        poolDic.Add(poolNameString, returnPool);
        return returnPool;
    }

    ObjectPool GetPool(string type)
    {
        ObjectPool returnPool;
        if (!poolDic.ContainsKey(type+"Pool"))
        {
            returnPool = CreatePool(type);
        }
        else
        {
            returnPool = poolDic[type + "Pool"];
        }
        return returnPool;
    }
}
