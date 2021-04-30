using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ToolTipPanel : BasePanel
{
    Graphic[] uiElements;
    float[] uiAlpha;

    public float fadeTime;

    TextMeshProUGUI toolTipContent;

    public override void Init()
    {
        OnInit.AddListener(InitToolTipPanel);
        OnShow.AddListener(ShowToolTipPanel);
        OnClose.AddListener(CloseToolTipPanel);
        base.Init();
    }

    void InitToolTipPanel()
    {
        toolTipContent = transform.Find("ToolTipContent").GetComponent<TextMeshProUGUI>();
        uiElements = GetComponentsInChildren<Graphic>();
        uiAlpha = new float[uiElements.Length];
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiAlpha[i] = uiElements[i].color.a;
            uiElements[i].color = new Color(uiElements[i].color.r, uiElements[i].color.g, uiElements[i].color.b, 0);
        }
    }

    void ShowToolTipPanel(object[] args)
    {
        toolTipContent.text = args[0] as string;
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(uiAlpha[i], fadeTime);
        }
    }

    void CloseToolTipPanel()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(0, fadeTime);
        }
    }

}
