using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/SO_Wave")]

public class SO_Wave : ScriptableObject
{
    public int enemyCount = 5;
    public List<SO_Enemy> enemyType = new List<SO_Enemy>();
}
