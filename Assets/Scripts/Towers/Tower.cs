using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;

    private void Start()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mySpriteRenderer.color = Color.white;
    }

    private void OnMouseDown()
    {
        mySpriteRenderer.color = Color.blue;
    }

    private void OnMouseUp()
    {
        mySpriteRenderer.color = Color.white;
    }
}
