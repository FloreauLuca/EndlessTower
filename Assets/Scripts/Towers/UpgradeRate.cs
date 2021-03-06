using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeRate : UpgradeButton
{
    protected override void Start()
    {
        base.Start();
        price = tower.RateUpgrade.Price;
        UpdatePrice();
    }

    protected override void Upgrade()
    {
        tower.UpgradeRate();
        price = tower.RateUpgrade.Price;
        UpdatePrice();
    }
}
