using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveProgressText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private WaveDisplay waveDisplay;

    public void DisplayWaveCount(int waveCount)
    {
        waveText.text = "Wave : " + waveCount.ToString();
    }

    public void DisplayWaveProgress(int enemyCount, int totalEnemy)
    {
        waveProgressText.text = enemyCount + "/" + totalEnemy;
    }

    public void DisplayMoney(int money)
    {
        moneyText.text = ("Money : " + money);
    }

    public void DisplayNewWave(int waveCount)
    {
        waveDisplay.Display(waveCount);
    }

}
