using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum DirectionType
    {
        WAVES,
        STRAIGHT
    }

    private int id = 0;

    private SpriteRenderer mySpriteRenderer;

    private SO_Enemy enemyData;
    public SO_Enemy EnemyData => enemyData;

    private float speed = 1.0f;
    [SerializeField] private Vector2 direction = Vector2.down;
    [SerializeField] private float yEndPosition = -4.0f;
    [SerializeField] private Color deathColor = Color.red;

    private float maxLife = 5;
    private float currentLife = 5;
    private EnemyManager enemyManager;

    void Awake()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Init(EnemyManager enemyManager, int id)
    {
        this.enemyManager = enemyManager;
        this.id = id;
    }

    public void Spawn(float speed, int lifes, SO_Enemy enemySO)
    {
        this.id = id;
        maxLife = lifes;
        this.speed = speed;
        currentLife = maxLife;
        enemyData = enemySO;
        mySpriteRenderer.color = Color.Lerp(deathColor, enemyData.Color, currentLife / maxLife);
        mySpriteRenderer.sprite = enemyData.Sprite;
    }

    void FixedUpdate()
    {
        if (!enemyData)
        {
            Die(null);
        }

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

    private void Update()
    {
        if (transform.position.y < yEndPosition)
        {
            Die(null);
            FindObjectOfType<GameManager>().LooseLife();
        }
    }

    public void TakeDamage(float damage, Tower tower)
    {
        currentLife -= damage;
        mySpriteRenderer.color = Color.Lerp(deathColor, enemyData.Color, currentLife / maxLife);
        if (currentLife <= 0)
        {
            Die(tower);
        }
    }

    private void Die(Tower tower)
    {
        enemyManager.Kill(id, tower);
    }

    public Vector2 Direction()
    {
        return direction * speed;
    }
}
