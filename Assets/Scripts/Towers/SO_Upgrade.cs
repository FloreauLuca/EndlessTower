using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgrade", menuName = "ScriptableObjects/SO_Upgrade")]

public class SO_Upgrade : ScriptableObject
{
    public enum UpgradeTypeEnum
    {
        SIDE,
        ADD_CANON,
        ADD_TOURET,
        INC_SIZE,
        INC_SIDE_SIZE,
        INC_RANGE,
        INC_REVENUE
    }
    public string Text = "Choice 1";
    public UpgradeTypeEnum UpgradeType = UpgradeTypeEnum.SIDE;
}
