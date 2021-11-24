using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector2 direction = Vector2.up;
    public Vector2 Direction
    {
        get => direction;
        set => direction = value;
    }
    [SerializeField] private float maxDist = 5.0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float size = 0.5f;
    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        direction.Normalize();
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.position += (Vector3)direction * (speed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if ((transform.position - startPosition).magnitude > maxDist)
        {
            DestroyObj();
        }

        Collider2D collider = Physics2D.OverlapCircle(transform.position, size, layerMask);

        if (collider)
        {
            collider.GetComponent<Enemy>().TakeDamage(damage);
            DestroyObj();
        }
    }

    void DestroyObj()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, size);
    }
}
