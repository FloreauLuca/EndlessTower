using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerManager : MonoBehaviour
{
    private const int TOWER_COUNT = 5;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private float xRange;
    [SerializeField] private int activeTower;
    private GameObject[] towers = new GameObject[TOWER_COUNT];
    private List<Tower> towerToUpgrades;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        for (int i = 0; i < TOWER_COUNT; i++)
        {
            towers[i] = Instantiate(towerPrefab, transform.position, Quaternion.identity, transform);
            towers[i].name = "Tower " + i;
            towers[i].SetActive(false);
        }
        AddTower();
    }

    public void AddTower()
    {
        activeTower++;
        float xCurrentRange = (xRange/4) * (activeTower - 1);
        for (int i = 0; i < activeTower; i++)
        {
            float percent = activeTower > 1 ? (float)i / (activeTower - 1) : 0;
            float xPos = Mathf.Lerp(-xCurrentRange, xCurrentRange, percent);
            Vector3 pos = new Vector3(xPos,
                transform.position.y, transform.position.z);
            towers[i].transform.position = pos;
            towers[i].SetActive(true);
        }
    }

    public bool CheckUpdates()
    {
        towerToUpgrades = new List<Tower>();
        bool updated = false;
        for (int i = 0; i < activeTower; i++)
        {
            Tower tower = towers[i].GetComponent<Tower>();
            if (tower.CheckUpgrade())
            {
                towerToUpgrades.Add(tower);
                updated = true;
            }
        }

        if (towerToUpgrades.Count > 0)
        {
            towerToUpgrades[0].DisplayUpgrades();
        }

        return updated;
    }

    public void TowerUpdated(Tower tower, int choice)
    {
        tower.ValidateUpgrade(choice);
        for (int i = towerToUpgrades.Count - 1; i >= 0; i--)
        {
            if (towerToUpgrades[i].CheckUpgrade())
            {
                towerToUpgrades[i].DisplayUpgrades();
                return;
            }
            else
            {
                towerToUpgrades.RemoveAt(i);
            }
        }
        gameManager.UpdatesDone();
    }
}
