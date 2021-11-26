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

    void Start()
    {
        for (int i = 0; i < TOWER_COUNT; i++)
        {
            Vector3 pos = new Vector3(Mathf.Lerp(-xRange, xRange, (float)i / (TOWER_COUNT - 1)), 
                transform.position.y, transform.position.z);
            towers[i] = Instantiate(towerPrefab, pos, Quaternion.identity, transform);
            towers[i].SetActive(false);
        }
        towers[2].SetActive(true);
        
    }
}
