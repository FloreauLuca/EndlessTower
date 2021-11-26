//#define USE_POOL
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


#if USE_POOL
public class EnemyPoolManager
{
    private int poolSize = 20;
    private GameObject prefab;
    private EnemyManager enemyManager;
    private Transform enemyManagerTransform;


    public Enemy[] pool;
    private bool[] poolActive;

    public EnemyPoolManager(int poolSize, GameObject prefab, EnemyManager enemyManager)
    {
        this.poolSize = poolSize;
        pool = new Enemy[poolSize];
        poolActive = new bool[poolSize];
        this.prefab = prefab;
        this.enemyManager = enemyManager;
        enemyManagerTransform = enemyManager.transform;
    }

    public void SpawnPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = GameObject.Instantiate(prefab, enemyManagerTransform.position, enemyManagerTransform.rotation, enemyManagerTransform).GetComponent<Enemy>();
            pool[i].EnemyManager = enemyManager;
            pool[i].gameObject.SetActive(false);
            poolActive[i] = false;
        }
    }

    public Enemy ActiveObject()
    {
        for (int i = 0; i < poolActive.Length; i++)
        {
            if (!poolActive[i])
            {
                poolActive[i] = true;
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }
        Debug.LogError(enemyManagerTransform.gameObject + " pool is empty");
        return null;
    }

    public void FreeObject(Enemy gameObject)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].GetInstanceID() == gameObject.GetInstanceID())
            {
                poolActive[i] = false;
                pool[i].gameObject.SetActive(false);
                return;
            }
        }
    }
}
#endif

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float xRange = 2.0f;

    [SerializeField] private float pauseDuration = 2.0f;

    private Wave currentWave = new Wave();
    private float timer = 0.0f;
    private bool isWaiting = true;
#if USE_POOL
    private EnemyPoolManager pool;
#endif

    private WaveManager waveManager;

    private GameManager gameManager;

    void Start()
    {
#if USE_POOL
        pool = new EnemyPoolManager(20, enemyPrefab, this);
        pool.SpawnPool();
#endif

        waveManager = FindObjectOfType<WaveManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (!gameManager.Playing)
        {
            return;
        }
        if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer > pauseDuration)
            {
                isWaiting = false;
                timer = 0.0f;
            }

            return;
        }
        if (currentWave.enemyNb > currentWave.currentEnemyCount)
        {
            timer += Time.deltaTime;
            if (timer > currentWave.spawnRate)
            {
                SpawnEnemy();
                timer = 0.0f;
                currentWave.currentEnemyCount++;
                gameManager.UpdateWave(currentWave);
            }
        }
        else
        {
            isWaiting = true;
            timer = 0.0f;
            CalculateNewWave();;
        }
        

    }

#if USE_POOL
    public void Kill(Enemy enemy)
    {
        gameManager.AddMoney(enemy.EnemyData.Reward);
        pool.FreeObject(enemy);
    }

    private void SpawnEnemy()
    {
        Vector3 pos = transform.position + Vector3.right * Random.Range(-xRange, xRange);
        Enemy newEnemy = pool.ActiveObject();
        newEnemy.transform.position = pos; 
        newEnemy.EnemyManager = this;
        newEnemy.Spawn(currentWave.enemySpeed, currentWave.enemyLife, currentWave.enemyType);
    }
#else

    public void Kill(Enemy enemy)
    {
        gameManager.AddMoney(enemy.EnemyData.Reward);
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemy()
    {
        Vector3 pos = transform.position + Vector3.right * Random.Range(-xRange, xRange);
        Enemy newEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity, transform).GetComponent<Enemy>();
        newEnemy.EnemyManager = this;
        newEnemy.Spawn(0, currentWave.enemySpeed, currentWave.enemyLife, currentWave.enemyType);

    }
#endif

    void CalculateNewWave()
    {
        currentWave = waveManager.CalculateNextWave();
        isWaiting = true;
        timer = 0.0f;
    }

    public void ResetWave(Wave wave)
    {
        currentWave = wave;
        isWaiting = true;
        timer = 0.0f;
    }
}
