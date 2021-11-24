using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;
    private const int POOL_SIZE = 20;
    [SerializeField] private GameObject projectile;
    private GameObject[] projectilePool = new GameObject[POOL_SIZE];

    [SerializeField] private float fireRate = 0.5f;
    private float fireTimer = 0.0f;

    [SerializeField] private float fireRange = 2.0f;
    [SerializeField] private LayerMask layerMask;

    private bool activated = false;

    private void Start()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mySpriteRenderer.color = Color.white;
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fireRange, layerMask);
        Enemy enemy = null;
        if (colliders.Length > 0)
        {
            int minIndex = 0;
            float minDist = Vector3.SqrMagnitude(colliders[minIndex].transform.position - transform.position);
            for (int i = 1; i < colliders.Length; i++)
            {
                float dist = Vector3.SqrMagnitude(colliders[i].transform.position - transform.position);
                if (dist < minDist)
                {
                    minIndex = i;
                    minDist = dist;
                }
            }

            enemy = colliders[minIndex].GetComponent<Enemy>();
        }
        activated = enemy != null;

        if (activated)
        {
            mySpriteRenderer.color = Color.blue;
            if (fireTimer >= fireRate)
            {
                GameObject gameObject = Instantiate(projectile, transform.position, transform.rotation, transform);
                
                gameObject.GetComponent<Projectile>().Direction =
                    enemy.transform.position + 
                    (Vector3)(enemy.Direction() * 0.2f) - 
                    transform.position;

                fireTimer = 0.0f;
            }
        }
        else
        {
            mySpriteRenderer.color = Color.white;
        }
    }

    private void OnMouseDown()
    {
        activated = true;
    }

    private void OnMouseUp()
    {
        activated = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = activated ? Color.blue : Color.white;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }
}
