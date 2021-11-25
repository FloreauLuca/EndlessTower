using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileData
{
    public float speed = 5.0f;
    public Vector2 direction = Vector2.up;
    public float lifeTime = 5.0f;
    public int damage = 1;
    public float size = 0.5f;
}

public class Tower : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;
    [SerializeField] private GameObject projectile;

    [SerializeField] private float fireRate = 0.5f;
    private float fireTimer = 0.0f;

    [SerializeField] private float fireRange = 2.0f;
    
    [SerializeField] private LayerMask layerMask;

    private bool activated = false;

    [SerializeField] private ProjectileData projectileData;
    private ParticleSystem particleSystem;

    private void Start()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mySpriteRenderer.color = Color.white;
        particleSystem = GetComponent<ParticleSystem>();
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
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                Vector3 direction = enemy.transform.position +
                                    (Vector3)(enemy.Direction() * ((enemy.transform.position.y + 4.0f) / 10.0f)) -
                                    transform.position;
                direction.Normalize();
                emitParams.velocity = direction * projectileData.speed;


                particleSystem.Emit(emitParams, 1);

                //GameObject gameObject = Instantiate(projectile, transform.position, transform.rotation, transform);
                //gameObject.GetComponent<Projectile>().Direction = direction ;

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

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("ParticleTouch", gameObject);
        other.GetComponent<Enemy>().TakeDamage(projectileData.damage);
    }
}
