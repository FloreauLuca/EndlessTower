using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public int CurrentWaveCount = 0;
    public int EnemyNb = 5;
    public int EnemyLife = 5;
    public int CurrentEnemySpawned = 5;
    public int CurrentEnemyKilled = 5;
    public float SpawnRate = 0.5f;
    public float EnemySpeed = 5.0f;
    public SO_EnemyFormation EnemyFormation;
    public bool BossWave;
}

public class WaveManager : MonoBehaviour
{
    private int currentWaveCount = 0;
    [SerializeField] private List<SO_EnemyFormation> enemyFormation = new List<SO_EnemyFormation>();
    private GameManager gameManager;
    private EnemyManager enemyManager;

    [SerializeField] private AnimationCurve enemyNbCurve;
    [SerializeField] private AnimationCurve enemyLifeCurve;
    [SerializeField] private AnimationCurve spawnRateCurve;
    [SerializeField] private AnimationCurve enemySpeedCurve;
    [SerializeField] private AnimationCurve enemyTypeCurve;
    [SerializeField] private int bossWave = 10;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
    }

    public Wave CalculateNextWave()
    {
        currentWaveCount++;
        Wave wave = new Wave();
        wave.CurrentWaveCount = currentWaveCount;
        wave.EnemyNb = Mathf.RoundToInt( enemyNbCurve.Evaluate(currentWaveCount/100.0f));
        wave.EnemyLife = Mathf.RoundToInt(enemyLifeCurve.Evaluate(currentWaveCount / 100.0f));
        wave.SpawnRate = spawnRateCurve.Evaluate(currentWaveCount / 100.0f);
        wave.EnemySpeed = enemySpeedCurve.Evaluate(currentWaveCount / 100.0f);
        wave.EnemyFormation = enemyFormation[Mathf.RoundToInt(enemyTypeCurve.Evaluate(currentWaveCount / 100.0f))%3];
        wave.CurrentEnemyKilled = 0;
        wave.CurrentEnemySpawned = 0;
        wave.BossWave = currentWaveCount % bossWave == 0;
        gameManager.DisplayNewWave(currentWaveCount);
        gameManager.DisplayWaveProgress(wave.CurrentEnemyKilled, wave.EnemyNb);
        return wave;
    }

    public void ResetWave()
    {
        currentWaveCount--;
        enemyManager.ResetWave(CalculateNextWave());
        gameManager.DisplayNewWave(currentWaveCount);
    }

}
