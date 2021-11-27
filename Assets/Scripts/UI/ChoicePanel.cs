using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoicePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI choiceText1;
    [SerializeField] private TextMeshProUGUI choiceText2;
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration;
    [SerializeField] private AnimationCurve fadeCurve;
    private Tower tower;
    private int choice = -1;

    private void Start()
    {
        canvasGroup.alpha = 0;
    }

    public void Display(Tower tower, SO_Upgrade choice1, SO_Upgrade choice2)
    {
        this.tower = tower;
        title.text = "Upgrade " + tower.name;
        choiceText1.text = choice1.Text;
        choiceText2.text = choice2.Text;
        StopAllCoroutines();
        canvasGroup.alpha = 1;
        this.choice = -1;
    }

    public void Validate(int choice)
    {
        if (this.choice == -1)
        {
            this.choice = choice;
            StartCoroutine(PanelFade());
        }
    }

    IEnumerator PanelFade()
    {
        float timer = 0.0f;
        while (timer < fadeDuration)
        {
            float alpha = fadeCurve.Evaluate(timer / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return new WaitForSeconds(0.05f);
            timer += 0.05f;
        }

        canvasGroup.alpha = 0;
        towerManager.TowerUpdated(tower, choice);
    }


}
