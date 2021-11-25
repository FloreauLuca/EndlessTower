using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI moneyText;

    public void DisplayWave(int waveCount, int enemyCount, int totalEnemy)
    {
        waveText.text = "Wave : " + waveCount.ToString() + " " + enemyCount + "/" + totalEnemy;
    }

    public void DisplayMoney(int money)
    {
        moneyText.text = ("Money : " + money);
    }
}
