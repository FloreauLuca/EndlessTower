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
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public Wave CalculateNextWave()
    {
        currentWaveCount++;
        Wave wave = new Wave();
        wave.currentWaveCount = currentWaveCount;
        wave.enemyNb = currentWaveCount + 5;
        wave.enemyLife = Mathf.CeilToInt(currentWaveCount / 2.0f);
        wave.spawnRate = 0.5f - currentWaveCount / 100.0f;
        wave.enemySpeed = currentWaveCount/10.0f + 5;
        wave.enemyType = enemyType[currentWaveCount%3];
        wave.currentEnemyCount = 0;
        gameManager.UpdateWave(wave);
        return wave;
    }

}
