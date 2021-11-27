using System;
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
    private SpriteRenderer mySpriteRenderer;

    [SerializeField] private float fireRate = 0.5f;
    private float fireTimer = 0.0f;

    [SerializeField] private float fireRange = 2.0f;
    
    [SerializeField] private LayerMask layerMask;

    private bool activated = false;
    [SerializeField] private int canonCount = 1;
    [SerializeField] private float canonSeperation = 0.05f;
    [SerializeField] private int sideCanon = 0;
    [SerializeField] private float sideCanonAngle = 20.0f;

    [SerializeField] private ProjectileData projectileData;
    private ParticleSystem particleSystem;
    
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
        UpdateAfterUpgrade();
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mySpriteRenderer.color = Color.white;
        particleSystem = GetComponent<ParticleSystem>();
        gameManager = FindObjectOfType<GameManager>();
        lvlText.text = towerLevel.ToString();
        choicePanel = FindObjectOfType<ChoicePanel>();
    }

    private void UpdateAfterUpgrade()
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
                for (int i = 1; i <= sideCanon; i++)
                {
                    emitParams.startSize = projectileData.size / 2.0f;

                    float sideAngle = Mathf.LerpAngle(0, sideCanonAngle, ((float)i / sideCanon));
                    Vector2 directionR = RotateVec(direction, -sideAngle * Mathf.Deg2Rad);
                    directionR.Normalize();
                    emitParams.velocity = directionR * projectileData.speed;
                    emitParams.position = Vector3.right * xRange;
                    particleSystem.Emit(emitParams, 1);
                    Vector2 directionL = RotateVec(direction, sideAngle * Mathf.Deg2Rad);
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
        gameManager.RemoveMoney(rateUpgrade.Price);
        rateUpgrade.AddLevel();
        fireRate -= 0.1f;
        projectileData.speed += 0.1f;
    }

    public void UpgradeDamage()
    {
        gameManager.RemoveMoney(damageUpgrade.Price);
        damageUpgrade.AddLevel();
        projectileData.damage += 0.5f;
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
        switch (upgradesChoices[choice].UpgradeType)
        {
            case SO_Upgrade.UpgradeTypeEnum.SIDE:
                sideCanon++;
                sideUpgrade.AddLevel();
                break;
            case SO_Upgrade.UpgradeTypeEnum.ADD_CANON:
                canonCount++;
                break;
            case SO_Upgrade.UpgradeTypeEnum.ADD_TOURET:
                FindObjectOfType<TowerManager>().AddTower();
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
        UpdateAfterUpgrade();
        if (upgradesChoices.Count > 2)
        {
            upgradesChoices.RemoveAt(choice);
        }
    }
}
