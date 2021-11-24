using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private int poolSize = 20;
    private GameObject prefab;
    private Transform spawner;


    private GameObject[] pool;
    private bool[] poolActive;

    public PoolManager(int poolSize, GameObject prefab, Transform spawner)
    {
        this.poolSize = poolSize;
        pool = new GameObject[poolSize];
        this.prefab = prefab;
        this.spawner = spawner;
    }

    void SpawnPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Object.Instantiate(prefab, spawner.position, spawner.rotation, spawner);
            poolActive[i] = false;
        }
    }

    GameObject GetNewObject()
    {
        for (int i = 0; i < poolActive.Length; i++)
        {
            if (!poolActive[i])
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }
        return null;
    }

    void FreeObject(GameObject gameObject)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i] == gameObject)
            {
                poolActive[i] = false;
                pool[i].SetActive(false);
                return;
            }
        }
    }
}
