using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthPanel : BasePanel
{
    [Header("heater sprite")]
    public Sprite full, half, empty;
    [Header("face sprite")]
    public Sprite openEyes, halfClosedEyes, closeEyes;

    [Header("valve1 for open&half,valve2 for half&close")]
    public int faceValve1,faceValve2;

    public List<Image> hearts = new List<Image>();

    [ConditionalShow(true)] Image face;

    Graphic[] uiElements;

    //判断是否需要震动
    int curShowHp = 0;

    public void DoShake()
    {
        for(int i = 0; i< uiElements.Length; i++)
        {
            uiElements[i].transform.DOShakePosition(0.2f,30,300);
        }
    }


    public override void Init()
    {
        OnInit.AddListener(InitHealthPanel);
        OnShow.AddListener(ShowHealthPanel);
        OnClose.AddListener(CloseHealthPanel);
        base.Init();
    }

    void InitHealthPanel()
    {
        face = transform.Find("face").GetComponent<Image>();
        uiElements = GetComponentsInChildren<Graphic>();
        ChangeHealth(10);
    }

    void ShowHealthPanel(object[] args)
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(1, 0.1f);
        }
    }

    void CloseHealthPanel()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(0, 0.1f);
        }
    }

    public void ChangeHealth(int value)
    {
        if (value > 10) value = 10;

        int f = value / 2, h = value % 2;
        for(int i = 0; i < hearts.Count; i++)
        {
            if (i < f) hearts[i].sprite = full;
            else hearts[i].sprite = empty;
        }
        if (h != 0)
        {
            hearts[f].sprite = half;
        }

        ChangeFace(value);

        if(curShowHp > value)
        {
            DoShake();
        }
        curShowHp = value;
    }

    void ChangeFace(int value)
    {
        if (value > faceValve1) face.sprite = openEyes;
        else if (value > faceValve2) face.sprite = halfClosedEyes;
        else face.sprite = closeEyes;
            
    }

}
