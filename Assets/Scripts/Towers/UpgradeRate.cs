using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeRate : UpgradeButton
{
    protected override void Start()
    {
        base.Start();
        price = tower.RatePrice;
        UpdatePrice();
    }

    protected override void Upgrade()
    {
        tower.UpgradeRate();
        price = tower.RatePrice;
        UpdatePrice();
    }
}
