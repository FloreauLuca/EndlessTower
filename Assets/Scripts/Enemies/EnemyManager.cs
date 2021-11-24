using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float xRange = 2.0f;
    [SerializeField] private List<SO_Wave> waves;

    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(enemyPrefab, 
                transform.position + Vector3.right * Random.Range(-xRange, xRange),
                Quaternion.identity, transform);
        }
    }
}
