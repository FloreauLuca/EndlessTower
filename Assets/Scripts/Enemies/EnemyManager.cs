using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
        pool[enemyId].transform.localScale = Vector3.one;
        pool[enemyId].gameObject.SetActive(false);
    }

    public List<int> GetActive()
    {
        List<int> activeId = new List<int>();
        for (int i = 0; i < poolActive.Length; i++)
        {
            if (poolActive[i])
            {
                activeId.Add(i);
            }
        }

        return activeId;
    }

    public Enemy GetEnemy(int id)
    {
        return pool[id];
    }
}

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnXRange = 2.0f;
    private EnemyPoolManager pool;
    
    [SerializeField] private float pauseDuration = 2.0f;
    private bool isWaiting = true;

    private Wave currentWave = new Wave();
    [SerializeField] private SO_Enemy bossData;
    private bool bossToSpawn = false;

    private float timer = 0.0f;

    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private ParticleSystem coinParticleSystem;


    private WaveManager waveManager;

    private GameManager gameManager;
    private AudioSource audioSource;

    void Start()
    {
        pool = new EnemyPoolManager(20, enemyPrefab, this);
        pool.SpawnPool();

        waveManager = FindObjectOfType<WaveManager>();
        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
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
        if (bossToSpawn)
        {
            SpawnBoss();
            bossToSpawn = false;
            currentWave.CurrentEnemySpawned++;
        }
        if (currentWave.EnemyNb > currentWave.CurrentEnemySpawned)
        {
            timer += Time.deltaTime;
            if (timer > currentWave.SpawnRate)
            {
                SpawnEnemy();
                timer = 0.0f;
                currentWave.CurrentEnemySpawned++;
            }
        }
        if (currentWave.EnemyNb <= currentWave.CurrentEnemyKilled)
        {
            isWaiting = true;
            timer = 0.0f;
            CalculateNewWave();
        }
        

    }
    
    public void Kill(int enemyId, Tower tower)
    {
        if (tower)
        {
            Enemy enemy = pool.GetEnemy(enemyId);
            SO_Enemy enemyData = enemy.EnemyData;

            ParticleSystem.EmitParams particles = new ParticleSystem.EmitParams();
            particles.position = enemy.transform.position - transform.position;
            particles.applyShapeToPosition = true;

            coinParticleSystem.Emit(particles, Mathf.CeilToInt(enemyData.Reward/2.0f));

            particles.startColor = enemyData.Color;
            deathParticleSystem.Emit(particles, 5);

            tower.Kill(enemyData.Reward,
                enemyData.Experience);
            currentWave.CurrentEnemyKilled++;
            gameManager.DisplayWaveProgress(currentWave.CurrentEnemyKilled,
                currentWave.EnemyNb);
            audioSource.Play();
        }

        pool.FreeObject(enemyId);
    }

    private void SpawnBoss()
    {
        Vector3 pos = transform.position;
        int enemyId = pool.ActiveObject();
        Enemy newEnemy = pool.GetEnemy(enemyId);
        newEnemy.transform.position = pos;
        SO_Enemy enemyType = bossData;
        newEnemy.Spawn(currentWave.EnemySpeed, currentWave.EnemyLife, enemyType);
    }

    private void SpawnEnemy()
    {
        Vector3 pos = transform.position + Vector3.right * Random.Range(-spawnXRange, spawnXRange);
        int enemyId = pool.ActiveObject();
        Enemy newEnemy = pool.GetEnemy(enemyId);
        newEnemy.transform.position = pos;
        SO_Enemy enemyType =
            currentWave.EnemyFormation.enemyTypes[Random.Range(0, currentWave.EnemyFormation.enemyTypes.Count)];
        newEnemy.Spawn(currentWave.EnemySpeed, currentWave.EnemyLife, enemyType);
    }

    void CalculateNewWave()
    {
        currentWave = waveManager.CalculateNextWave();
        bossToSpawn = currentWave.BossWave;
        isWaiting = true;
        timer = 0.0f;
    }

    public void ResetWave(Wave wave)
    {
        List<int> activeId = pool.GetActive();
        foreach (int id in activeId)
        {
            Kill(id, null);
        }
        currentWave = wave;
        bossToSpawn = currentWave.BossWave;
        isWaiting = true;
        timer = 0.0f;
    }
}
