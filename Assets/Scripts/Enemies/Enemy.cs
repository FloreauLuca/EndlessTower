using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum SpriteType
    {
        TRIANGLE,
        CIRCLE,
        SQUARE
    }

    public enum DirectionType
    {
        WAVES,
        STRAIGHT
    }

    private SpriteRenderer mySpriteRenderer;
    
    void Start()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    void Update()
    {
        
    }
}
