using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public int currentWaveCount = 0;
    public int enemyNb = 5;
    public int enemyLife = 5;
    public int currentEnemyCount = 5;
    public float spawnRate = 0.5f;
    public float enemySpeed = 5.0f;
    public SO_Enemy enemyType;
}

public class WaveManager : MonoBehaviour
{
    private int currentWaveCount = 0;
    [SerializeField] private List<SO_Enemy> enemyType = new List<SO_Enemy>();
    private GameManager gameManager;
    private EnemyManager enemyManager;

    [SerializeField] private AnimationCurve enemyNbCurve;
    [SerializeField] private AnimationCurve enemyLifeCurve;
    [SerializeField] private AnimationCurve spawnRateCurve;
    [SerializeField] private AnimationCurve enemySpeedCurve;
    [SerializeField] private AnimationCurve enemyTypeCurve;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
    }

    public Wave CalculateNextWave()
    {
        currentWaveCount++;
        Wave wave = new Wave();
        wave.currentWaveCount = currentWaveCount;
        wave.enemyNb = Mathf.RoundToInt( enemyNbCurve.Evaluate(currentWaveCount/100.0f));
        wave.enemyLife = Mathf.RoundToInt(enemyLifeCurve.Evaluate(currentWaveCount / 100.0f));
        wave.spawnRate = spawnRateCurve.Evaluate(currentWaveCount / 100.0f);
        wave.enemySpeed = enemySpeedCurve.Evaluate(currentWaveCount / 100.0f);
        wave.enemyType = enemyType[Mathf.RoundToInt(enemyTypeCurve.Evaluate(currentWaveCount / 100.0f))%3];
        wave.currentEnemyCount = 0;
        gameManager.UpdateWave(wave);
        gameManager.DisplayNewWave(currentWaveCount);
        return wave;
    }

    public void ResetWave()
    {
        currentWaveCount--;
        enemyManager.ResetWave(CalculateNextWave());
        gameManager.DisplayNewWave(currentWaveCount);
    }

}
