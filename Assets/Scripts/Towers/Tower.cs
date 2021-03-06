using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class ProjectileData
{
    public float speed = 5.0f;
    public Vector2 direction = Vector2.up;
    public float damage = 1;
    public float size = 0.5f;
    public float rewardMult = 1.0f;
}

[Serializable]
public class UpgradeData
{
    [SerializeField] private AnimationCurve upgradeCurve;
    private int level = 1;
    public int Level => level;

    public virtual void AddLevel()
    {
        level++;
    }

    public virtual float GetUpgradeValue()
    {
        return upgradeCurve.Evaluate(level);
    }
}

[Serializable]
public class UpgradePricedData : UpgradeData
{
    [SerializeField] private AnimationCurve priceCurve;
    private int price = 10;
    public int Price => price;
    
    public void UpdatePrice()
    {
        price = Mathf.FloorToInt(priceCurve.Evaluate(Level));
    }

    public override void AddLevel()
    {
        base.AddLevel();
        UpdatePrice();
    }
}

public class Tower : MonoBehaviour
{
    [SerializeField] private SpriteRenderer towerSpriteRenderer;
    [SerializeField] private Color fireColor;

    private GameManager gameManager;
    private AudioSource audioSource;
    private TowerManager towerManager;

    private bool activated = false;

    [Header("Canon Parameter")]
    private ParticleSystem projectileParticleSystem;
    [SerializeField] private int canonCount = 1;
    [SerializeField] private float canonSeperation = 0.05f;
    [SerializeField] private int sideCanon = 0;
    private float sideCanonAngle = 20.0f;
    private float fireRate = 0.5f;
    private float fireTimer = 0.0f;
    private float fireRange = 2.0f;
    
    [SerializeField] private LayerMask enemyLayerMask;
    private Enemy nearestEnemy = null;

    [Header("Levels Parameter")]
    private int towerLevel = 1;
    private float towerExp = 0.0f;
    [SerializeField] private AnimationCurve expToNextLvl;
    [SerializeField] private TextMeshPro lvlText;
    [SerializeField] private AnimationCurve levelBetweenUpgrade;
    private int lastUpgradeLvl = 0;

    [Header("Upgrade Parameter")]
    private ProjectileData projectileData = new ProjectileData();
    [SerializeField] private UpgradePricedData rateUpgrade;
    public UpgradePricedData RateUpgrade => rateUpgrade;
    [SerializeField] private UpgradePricedData damageUpgrade;
    public UpgradePricedData DamageUpgrade => damageUpgrade;
    [SerializeField] private UpgradeData sizeUpgrade;
    public UpgradeData SizeUpgrade => sizeUpgrade;
    [SerializeField] private UpgradeData rangeUpgrade;
    public UpgradeData RangeUpgrade => rangeUpgrade;
    [SerializeField] private UpgradeData revenueUpgrade;
    public UpgradeData RevenueUpgrade => revenueUpgrade;
    [SerializeField] private UpgradeData speedUpgrade;
    public UpgradeData SpeedUpgrade => speedUpgrade;
    [SerializeField] private UpgradeData sideUpgrade;
    public UpgradeData SideUpgrade => sideUpgrade;
    private ChoicePanel choicePanel;
    [FormerlySerializedAs("upgradesChoices")] [SerializeField] private List<SO_Upgrade> upgradesOrder;
    [SerializeField] private List<SO_Upgrade> infiniteUpgrades;

    private void Start()
    {
        towerSpriteRenderer.color = Color.white;
        projectileParticleSystem = GetComponent<ParticleSystem>();

        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        towerManager = FindObjectOfType<TowerManager>();

        lvlText.text = towerLevel.ToString();
        choicePanel = FindObjectOfType<ChoicePanel>();

        UpdateFromLevel();
    }

    private void UpdateFromLevel()
    {
        rateUpgrade.UpdatePrice();
        fireRate = rateUpgrade.GetUpgradeValue();
        damageUpgrade.UpdatePrice();
        projectileData.damage = damageUpgrade.GetUpgradeValue();
        projectileData.size = sizeUpgrade.GetUpgradeValue();
        fireRange = rangeUpgrade.GetUpgradeValue();
        projectileData.rewardMult = revenueUpgrade.GetUpgradeValue();
        projectileData.speed = speedUpgrade.GetUpgradeValue();
        sideCanonAngle = sideUpgrade.GetUpgradeValue();
    }

    public static Vector2 RotateVec(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    //Find the enemy closest to the line
    private Enemy FindNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fireRange, enemyLayerMask);
        nearestEnemy = null;
        if (colliders.Length > 0)
        {
            int minIndex = 0;
            float minDist = Mathf.Abs(colliders[minIndex].transform.position.y - transform.position.y);
            for (int i = 1; i < colliders.Length; i++)
            {
                float dist = Mathf.Abs(colliders[i].transform.position.y - transform.position.y);
                if (dist < minDist)
                {
                    minIndex = i;
                    minDist = dist;
                }
            }

            nearestEnemy = colliders[minIndex].GetComponent<Enemy>();
        }

        return nearestEnemy;
    }

    private void ShootProjectile(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90;
        towerSpriteRenderer.transform.eulerAngles = Vector3.forward * angle;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
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
            projectileParticleSystem.Emit(emitParams, 1);
        }

        //Shoot side projectile
        for (int i = 1; i <= sideCanon; i++)
        {
            emitParams.startSize = projectileData.size / 2.0f;

            float sideAngle = Mathf.LerpAngle(0, sideCanonAngle, ((float)i / sideCanon));
            Vector2 directionR = RotateVec(direction, -sideAngle * Mathf.Deg2Rad);
            directionR.Normalize();
            emitParams.velocity = directionR * projectileData.speed;
            emitParams.position = Vector3.right * xRange;
            projectileParticleSystem.Emit(emitParams, 1);
            Vector2 directionL = RotateVec(direction, sideAngle * Mathf.Deg2Rad);
            directionL.Normalize();
            emitParams.velocity = directionL * projectileData.speed;
            emitParams.position = Vector3.right * -xRange;
            projectileParticleSystem.Emit(emitParams, 1);
        }
        audioSource.Play();
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        nearestEnemy = FindNearestEnemy();
        activated = nearestEnemy != null;

        if (activated)
        {
            towerSpriteRenderer.color = fireColor;
            if (fireTimer >= fireRate)
            {
                Vector3 direction = nearestEnemy.transform.position -
                                    transform.position +
                                    (Vector3)nearestEnemy.Direction() * 0.01f;
                ShootProjectile(direction);
                fireTimer = 0.0f;
            }
        }
        else
        {
            towerSpriteRenderer.color = Color.white;
        }
    }

    //Also active for OnTouchDown
    private void OnMouseDown()
    {
        if (activated)
        {
            Vector3 direction = nearestEnemy.transform.position -
                                transform.position +
                                (Vector3)nearestEnemy.Direction() * 0.01f;
            ShootProjectile(direction);
        }
        else
        {
            ShootProjectile(Vector3.up);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = activated ? fireColor : Color.white;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }

    private void OnParticleCollision(GameObject other)
    {
        other.GetComponent<Enemy>().TakeDamage(projectileData.damage, this);
    }
    
    public void UpgradeRate()
    {
        gameManager.RemoveMoney(rateUpgrade.Price);
        rateUpgrade.AddLevel();
        UpdateFromLevel();
    }

    public void UpgradeDamage()
    {
        gameManager.RemoveMoney(damageUpgrade.Price);
        damageUpgrade.AddLevel();
        UpdateFromLevel();
    }

    public void Kill(int reward, float experience)
    {
        gameManager.AddMoney(Mathf.FloorToInt(reward * projectileData.rewardMult));
        towerExp += experience;
        if (towerExp > expToNextLvl.Evaluate(towerLevel))
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
        if (towerLevel - lastUpgradeLvl >= Mathf.FloorToInt( levelBetweenUpgrade.Evaluate(towerLevel)))
        {
            return true;
        }

        return false;
    }

    public void DisplayUpgrades()
    {
        if (upgradesOrder[0].UpgradeType == SO_Upgrade.UpgradeTypeEnum.ADD_TOURET && towerManager.NewTowerAvailable())
        {
            upgradesOrder.RemoveAt(0);
        }
        if (upgradesOrder[1].UpgradeType == SO_Upgrade.UpgradeTypeEnum.ADD_TOURET && towerManager.NewTowerAvailable())
        {
            upgradesOrder.RemoveAt(1);
        }

        choicePanel.Display(this, upgradesOrder[0], upgradesOrder[1]);
    }

    public void ValidateUpgrade(int choice)
    {
        lastUpgradeLvl += Mathf.FloorToInt(levelBetweenUpgrade.Evaluate(towerLevel));
        switch (upgradesOrder[choice].UpgradeType)
        {
            case SO_Upgrade.UpgradeTypeEnum.SIDE:
                sideCanon++;
                sideUpgrade.AddLevel();
                break;
            case SO_Upgrade.UpgradeTypeEnum.ADD_CANON:
                canonCount++;
                break;
            case SO_Upgrade.UpgradeTypeEnum.ADD_TOURET:
                towerManager.AddTower();
                break;
            case SO_Upgrade.UpgradeTypeEnum.INC_SIZE:
                sizeUpgrade.AddLevel();
                break;
            case SO_Upgrade.UpgradeTypeEnum.INC_RANGE:
                rangeUpgrade.AddLevel();
                break;
            case SO_Upgrade.UpgradeTypeEnum.INC_REVENUE:
                revenueUpgrade.AddLevel();
                break;
            default:
                break;
        }
        UpdateFromLevel();
        upgradesOrder.RemoveAt(choice);
        if (upgradesOrder.Count < 2)
        {
            upgradesOrder.Add(infiniteUpgrades[Random.Range(0, infiniteUpgrades.Count-1)]);
        }
    }
}
