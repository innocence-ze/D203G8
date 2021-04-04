using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class DialogPanel : BasePanel
{
    [ConditionalShow(true)]public TextMeshProUGUI dialogContent;

    public float fadeTime;

    Graphic[] uiElements;
    float[] uiAlpha;

    public StrEvent OnTyping;

    private void Awake()
    {
        OnInit?.AddListener(InitDialogPanel);
        OnShow?.AddListener(ShowDialogPanel);
        OnClose?.AddListener(CloseDialogPanel);
        GameManager.Singleton.dialogMgr.OnTyping.AddListener(ShowDialog);
        GameManager.Singleton.dialogMgr.OnNextSentence.AddListener(ClearDialog);
        GameManager.Singleton.dialogMgr.OnOff.AddListener(RecycleDialogPanel);
    }

    //显示到ui上
    void ShowDialog(string dialog)
    {
        dialogContent.text = dialog;
    }

    void ClearDialog()
    {
        dialogContent.text = "";
    }

    void InitDialogPanel()
    {
        dialogContent = transform.Find("DialogContent").GetComponent<TextMeshProUGUI>();

        uiElements = GetComponentsInChildren<Graphic>();
        uiAlpha = new float[uiElements.Length];
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiAlpha[i] = uiElements[i].color.a;
            uiElements[i].color = new Color(uiElements[i].color.r, uiElements[i].color.g, uiElements[i].color.b, 0);
        }
    }

    void ShowDialogPanel(object[] args)
    {
        dialogContent.text = "";
        for(int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(uiAlpha[i], fadeTime);
        }
    }

    void CloseDialogPanel()
    {
        for(int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].DOFade(0, fadeTime);
        }
    }

    void RecycleDialogPanel()
    {
        GameManager.Singleton.uiMgr.Close(GetType().ToString(), fadeTime);
    }
}
