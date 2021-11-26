using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Image image;
    [SerializeField] private float duration = 1;
    private float timer = 0.0f;
    private Color imageColor;
    private Color textColor;
    [SerializeField] private AnimationCurve fade;

    private void Start()
    {
        image = GetComponent<Image>();
        imageColor = image.color;
        textColor = text.color;
    }

    public void Display(int waveCount)
    {
        text.text = "Wave : " + waveCount.ToString();
        StartCoroutine(WaveFade());
    }

    IEnumerator WaveFade()
    {
        timer = 0.0f;
        while (timer < duration)
        {
            float alpha = fade.Evaluate(timer / duration);
            text.color = textColor * new Color(1, 1, 1, alpha);
            image.color = imageColor * new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(0.05f);
            timer += 0.05f;
        }
        text.color = Color.clear;
        image.color = Color.clear;
    }
}
