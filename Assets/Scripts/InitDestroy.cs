using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use to destroy prefabs at te start of the game
public class InitDestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject);
    }
}
