using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyPoolManager
{
    private int poolSize = 20;
    private GameObject prefab;
    private EnemyManager enemyManager;
    private Transform enemyManagerTransform;


    private Enemy[] pool;
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
            pool[i].Init(enemyManager, i);
            pool[i].gameObject.SetActive(false);
            poolActive[i] = false;
        }
    }

    public int ActiveObject()
    {
        for (int i = 0; i < poolActive.Length; i++)
        {
            if (!poolActive[i])
            {
                poolActive[i] = true;
                pool[i].gameObject.SetActive(true);
                return i;
            }
        }
        Debug.LogError(enemyManagerTransform.gameObject + " pool is empty");
        return -1;
    }

    public void FreeObject(int enemyId)
    {
        poolActive[enemyId] = false;
        pool[enemyId].transform.position = enemyManagerTransform.position;
        pool[enemyId].gameObject.SetActive(false);
    }

    public Enemy GetEnemy(int id)
    {
        return pool[id];
    }
}

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float xRange = 2.0f;

    [SerializeField] private float pauseDuration = 2.0f;

    private Wave currentWave = new Wave();
    private float timer = 0.0f;
    private bool isWaiting = true;
    private EnemyPoolManager pool;

    private WaveManager waveManager;

    private GameManager gameManager;

    void Start()
    {
        pool = new EnemyPoolManager(20, enemyPrefab, this);
        pool.SpawnPool();

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
        if (currentWave.enemyNb > currentWave.currentEnemySpawned)
        {
            timer += Time.deltaTime;
            if (timer > currentWave.spawnRate)
            {
                SpawnEnemy();
                timer = 0.0f;
                currentWave.currentEnemySpawned++;
            }
        }
        if (currentWave.enemyNb <= currentWave.currentEnemyKilled)
        {
            isWaiting = true;
            timer = 0.0f;
            CalculateNewWave();;
        }
        

    }
    
    public void Kill(int enemyId, Tower tower)
    {
        if (tower)
        {
            tower.Kill(pool.GetEnemy(enemyId).EnemyData.Reward, pool.GetEnemy(enemyId).EnemyData.Experience);
        }

        pool.FreeObject(enemyId);
        currentWave.currentEnemyKilled++;
        gameManager.DisplayWaveProgress(currentWave.currentEnemyKilled, currentWave.enemyNb);
    }

    private void SpawnEnemy()
    {
        Vector3 pos = transform.position + Vector3.right * Random.Range(-xRange, xRange);
        int enemyId = pool.ActiveObject();
        Enemy newEnemy = pool.GetEnemy(enemyId);
        newEnemy.transform.position = pos; 
        newEnemy.Spawn(currentWave.enemySpeed, currentWave.enemyLife, currentWave.enemyType);
    }

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
