using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpeed : UpgradeButton
{
    protected override void Start()
    {
        base.Start();
        price = tower.SpeedPrice;
    }

    protected override void Upgrade()
    {
        tower.UpgradeSpeed();
        price = tower.SpeedPrice;
    }
}
