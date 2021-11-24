using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/SO_Enemy")]

public class SO_Enemy : ScriptableObject
{
    public Enemy.SpriteType EnemyType = Enemy.SpriteType.CIRCLE;

    public Enemy.DirectionType DirectionType = Enemy.DirectionType.STRAIGHT;

    public float Speed = 10.0f;

    public float Damage = 10.0f;
}
