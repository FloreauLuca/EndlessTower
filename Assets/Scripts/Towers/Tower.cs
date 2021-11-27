using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class ProjectileData
{
    public float speed = 5.0f;
    public Vector2 direction = Vector2.up;
    public float lifeTime = 5.0f;
    public float damage = 1;
    public float size = 0.5f;
    public float rewardMult = 1.0f;
}

public class Tower : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;

    [SerializeField] private float fireRate = 0.5f;
    private float fireTimer = 0.0f;

    [SerializeField] private float fireRange = 2.0f;
    
    [SerializeField] private LayerMask layerMask;

    private bool activated = false;
    [SerializeField] private int canonCount = 1;
    [SerializeField] private float canonSeperation = 0.05f;
    [SerializeField] private bool sideCanon = false;
    [SerializeField] private float sideCanonAngle = 45.0f;

    [SerializeField] private ProjectileData projectileData;
    private ParticleSystem particleSystem;

    private int rateLevel = 1;
    private int speedLevel = 1;
    private int damageLevel = 1;

    private int ratePrice;
    public int RatePrice => ratePrice;
    private int speedPrice;
    public int SpeedPrice => speedPrice;
    private int damagePrice;
    public int DamagePrice => damagePrice;

    private GameManager gameManager;

    private int towerLevel = 0;
    private float towerExp = 0.0f;
    [SerializeField] private float expToNextLvl = 100.0f;
    [SerializeField] private TextMeshPro lvlText;
    [SerializeField] private int levelBetweenUpgrade = 5;
    private int lastUpgradeLvl = 0;
    private ChoicePanel choicePanel;
    [SerializeField] private List<SO_Upgrade> upgradesChoices;

    private void Start()
    {
        ratePrice = rateLevel * 10;
        speedPrice = speedLevel * 10;
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mySpriteRenderer.color = Color.white;
        particleSystem = GetComponent<ParticleSystem>();
        gameManager = FindObjectOfType<GameManager>();
        lvlText.text = towerLevel.ToString();
        choicePanel = FindObjectOfType<ChoicePanel>();
    }

    public Vector2 RotateVec(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
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
                Vector3 direction = enemy.transform.position -
                                    transform.position + 
                                    (Vector3) enemy.Direction() * 0.01f;
                direction.Normalize();
                emitParams.velocity = direction * projectileData.speed;
                emitParams.startLifetime = fireRange / projectileData.speed;
                emitParams.startSize = projectileData.size;

                float xRange = canonSeperation * (canonCount - 1);
                for (int i = 0; i < canonCount; i++)
                {
                    float percent = canonCount > 1 ? (float)i / (canonCount - 1) : 0;
                    float xPos = Mathf.Lerp(-xRange, xRange, percent);
                    emitParams.position = Vector3.right * xPos;
                    particleSystem.Emit(emitParams, 1);
                }
                if (sideCanon)
                {
                    Vector2 directionR = RotateVec(direction, -sideCanonAngle * Mathf.Deg2Rad);
                    directionR.Normalize();
                    emitParams.velocity = directionR * projectileData.speed;
                    emitParams.position = Vector3.right * xRange;
                    particleSystem.Emit(emitParams, 1);

                    Vector2 directionL = RotateVec(direction, sideCanonAngle * Mathf.Deg2Rad);
                    directionL.Normalize();
                    emitParams.velocity = directionL * projectileData.speed;
                    emitParams.position = Vector3.right * -xRange;
                    particleSystem.Emit(emitParams, 1);
                }

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
        other.GetComponent<Enemy>().TakeDamage(projectileData.damage, this);
    }
    
    public void UpgradeRate()
    {
        fireRate -= 0.1f;
        projectileData.speed += 0.1f;
        gameManager.RemoveMoney(ratePrice);
        rateLevel++;
        ratePrice = rateLevel * 10;
    }

    public void UpgradeSpeed()
    {
        projectileData.speed += 0.1f;
        gameManager.RemoveMoney(speedPrice);
        speedLevel++;
        speedPrice = speedLevel * 10;
    }

    public void UpgradeDamage()
    {
        projectileData.damage += 0.5f;
        gameManager.RemoveMoney(damagePrice);
        damageLevel++;
        damagePrice = damageLevel * 10;
    }

    public void Kill (int reward, float experience)
    {
        gameManager.AddMoney(reward);
        towerExp += experience;
        if (towerExp > expToNextLvl)
        {
            NewLevel();
            towerExp = 0.0f;
        }
    }

    public void NewLevel()
    {
        towerExp = 0.0f;
        towerLevel++;
        lvlText.text = towerLevel.ToString();
    }

    public bool CheckUpgrade()
    {
        if (towerLevel - lastUpgradeLvl >= levelBetweenUpgrade)
        {
            return true;
        }

        return false;
    }

    public void DisplayUpgrades()
    {
        choicePanel.Display(this, upgradesChoices[0], upgradesChoices[1]);
    }

    public void ValidateUpgrade(int choice)
    {
        lastUpgradeLvl += levelBetweenUpgrade;
    }
}
