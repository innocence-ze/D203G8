using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuMask : MonoBehaviour
{
    Image[] masks;
    float[] fadeTime, endValue;

    public float minInterval, maxInterval;

    private void Start()
    {
        masks = GetComponentsInChildren<Image>();
        fadeTime = new float[masks.Length];
        endValue = new float[masks.Length];

        for(int i = 0; i < masks.Length; i++)
        {
            fadeTime[i] = Random.Range(minInterval, maxInterval);
            endValue[i] = Random.Range(0.1f, 0.8f);
        }

        for (int i = 0; i < masks.Length; i++)
        {
            masks[i].DOFade(endValue[i], fadeTime[i]).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
