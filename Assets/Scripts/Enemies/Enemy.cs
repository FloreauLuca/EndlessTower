using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum SpriteType
    {
        TRIANGLE,
        CIRCLE,
        SQUARE
    }

    public enum DirectionType
    {
        WAVES,
        STRAIGHT
    }

    private SpriteRenderer mySpriteRenderer;

    private SO_Enemy enemyData;

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector2 direction = Vector2.down;
    [SerializeField] private float yEndPosition = -4.0f;

    [SerializeField] private float maxLife = 5;
    private float currentLife = 5;

    void Start()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentLife = maxLife;
        mySpriteRenderer.color = Color.Lerp(Color.red, Color.yellow, currentLife / maxLife);
    }

    void FixedUpdate()
    {
        if (!enemyData)
        {
            transform.position += (Vector3)direction * (speed * Time.fixedDeltaTime);
        }
        else
        {
            switch(enemyData.DirectionType)
            {
                case DirectionType.STRAIGHT:
                    transform.position += (Vector3) direction * (speed * Time.fixedDeltaTime);
                break;
                case DirectionType.WAVES:
                    //transform.position += (Vector3)direction * (speed * Time.fixedDeltaTime);
                    break;
                default:
                    transform.position += (Vector3)direction * (speed * Time.fixedDeltaTime);
                    break;
            }
        }

    }

    private void Update()
    {
        if (transform.position.y < yEndPosition)
        {
            Die();
        }
    }

    public void TakeDamage(float damage)
    {
        currentLife -= damage;
        mySpriteRenderer.color = Color.Lerp(Color.red, Color.yellow, currentLife / maxLife);
        if (currentLife <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public Vector2 Direction()
    {
        return direction * speed;
    }
}
