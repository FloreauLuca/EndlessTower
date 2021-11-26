using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            fpsText.text = (1.0f / Time.deltaTime).ToString("0") + "FPS";
            timer = 0.0f;
        }
    }
}
