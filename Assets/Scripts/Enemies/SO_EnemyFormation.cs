using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyFormation", menuName = "ScriptableObjects/SO_EnemyFormation")]
public class SO_EnemyFormation : ScriptableObject
{
    public List<SO_Enemy> enemyTypes;
}
