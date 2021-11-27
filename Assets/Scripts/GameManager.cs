using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;
    private WaveManager waveManager;
    private TowerManager towerManager;

    private int money = 0;
    public int Money => money;

    private bool playing = true;

    public bool Playing => playing;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        waveManager = FindObjectOfType<WaveManager>();
        towerManager = FindObjectOfType<TowerManager>();
        uiManager.DisplayWaveCount(0);
        uiManager.DisplayWaveProgress(0, 0);
        uiManager.DisplayMoney(money);
    }

    public void DisplayNewWave(int waveCount)
    {
        playing = !towerManager.CheckUpdates();
        uiManager.DisplayNewWave(waveCount);
        DisplayWaveCount(waveCount);
    }

    public void DisplayWaveCount(int waveCount)
    {
        uiManager.DisplayWaveCount(waveCount);
    }

    public void DisplayWaveProgress(int currentEnemyCount, int enemyNb)
    {
        uiManager.DisplayWaveProgress(currentEnemyCount, enemyNb);
    }

    public void AddMoney(int reward)
    {
        this.money += reward;
        uiManager.DisplayMoney(money);
    }

    public void RemoveMoney(int price)
    {
        this.money -= price;
        uiManager.DisplayMoney(money);
    }

    public void LooseLife()
    {
        waveManager.ResetWave();
    }

    public void UpdatesDone()
    {
        playing = true;
    }
}
