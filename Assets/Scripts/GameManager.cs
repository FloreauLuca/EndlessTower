using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;

    private int lifes = 5;
    private int money = 0;

    private bool playing = true;

    public bool Playing => playing;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        uiManager.DisplayWave(0, 0, 0);
        uiManager.DisplayMoney(money);
        uiManager.DisplayLife(lifes);
    }

    public void UpdateWave(Wave wave)
    {
        uiManager.DisplayWave(wave.currentWaveCount, wave.currentEnemyCount, wave.enemyNb);
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
        lifes--;
        uiManager.DisplayLife(lifes);
        if (lifes <= 0)
        {
            playing = false;
        }
    }

}
