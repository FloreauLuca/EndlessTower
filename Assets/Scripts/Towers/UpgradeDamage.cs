using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDamage : UpgradeButton
{
    protected override void Start()
    {
        base.Start();
        price = tower.DamagePrice;
    }

    protected override void Upgrade()
    {
        tower.UpgradeDamage();
        price = tower.DamagePrice;
    }
}
