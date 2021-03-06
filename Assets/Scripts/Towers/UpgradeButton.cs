using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] protected Tower tower;
    private SpriteRenderer mySpriteRenderer;
    private Color baseColor;
    [SerializeField] private Color pressedColor = Color.white;
    [SerializeField] private Color unAvailableColor = Color.white;
    private GameManager gameManager;
    protected int price;
    private bool available = true;
    [SerializeField] private TextMeshPro priceText;

    protected virtual void Start()
    {
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        baseColor = mySpriteRenderer.color;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        available = gameManager.Money >= price;
        if (available)
        {
            mySpriteRenderer.color = baseColor;
        }
        else
        {
            mySpriteRenderer.color = unAvailableColor;
        }
    }

    protected virtual void Upgrade()
    {
    }

    protected void UpdatePrice()
    {
        priceText.text = price.ToString();
    }

    private void OnMouseDown()
    {
        if (available)
        {
            Upgrade();
            mySpriteRenderer.color = pressedColor;
        }
    }

    private void OnMouseUp()
    {
        if (available)
        {
            mySpriteRenderer.color = baseColor;
        }
    }
}
