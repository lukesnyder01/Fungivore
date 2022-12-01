using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string poolName;
    public GameObject pooledObject;
    public int poolSize;
    public bool canGrow;
    public List<GameObject> pooledObjects;
}



public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler current;

    //public List<int> testInts;
    [SerializeField]
    public List<Pool> objectPools;


    void Awake()
    {
        current = this;
    }

    void Start()
    {
        for (int i = 0; i < objectPools.Count; i++)
        {
            objectPools[i].pooledObjects = new List<GameObject>();

            for (int j = 0; j < objectPools[i].poolSize; j++)
            {
                GameObject obj = Instantiate(objectPools[i].pooledObject);
                obj.SetActive(false);
                objectPools[i].pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(int poolIndex)
    {
        var selectedPool = objectPools[poolIndex];

        for (int i = 0; i < selectedPool.pooledObjects.Count; i++)
        {
            if (!selectedPool.pooledObjects[i].activeInHierarchy)
            {
                return selectedPool.pooledObjects[i];
            }
        }

        if (selectedPool.canGrow)
        {
            selectedPool.poolSize++;
            GameObject obj = Instantiate(selectedPool.pooledObject);
            selectedPool.pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }


}
