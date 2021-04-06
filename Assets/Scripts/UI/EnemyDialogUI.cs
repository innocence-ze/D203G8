using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyDialogUI : MonoBehaviour
{
    [ConditionalShow(true)]public TextMeshProUGUI dialogObject;
    [ConditionalShow(true)]public TextMeshProUGUI dialogContent;

    public float fadeTime;
    TalkableEnemy self;

    Graphic[] uiElements;
    float[] uiAlpha;

    // Start is called before the first frame update
    void Awake()
    {
        dialogObject = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        dialogContent = transform.Find("Content").GetComponent<TextMeshProUGUI>();
        self = GetComponentInParent<TalkableEnemy>();
        dialogObject.text = self.gameObject.name;
        self.OnShow.AddListener(OnShow);
        self.OnOff.AddListener(OnOff);
        self.OnTyping.AddListener(OnTyping);

        uiElements = GetComponentsInChildren<Graphic>();
        uiAlpha = new float[uiElements.Length];
        for(int i = 0; i < uiElements.Length; i++)
        {
            uiAlpha[i] = uiElements[i].color.a;
            uiElements[i].color = new Color(uiElements[i].color.r, uiElements[i].color.g, uiElements[i].color.b, 0);
        }
    }

    void OnShow()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(uiAlpha[i], fadeTime);
        }
    }

    void OnOff()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(0, fadeTime);
        }
    }

    void OnTyping(string content)
    {
        dialogContent.text = content;
    }
}
