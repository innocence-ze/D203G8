using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DisappearPlatform : MonoBehaviour
{
    public enum DisappearType
    {
        afterStepd,
        afterInterval,
    }

    public DisappearType disappearType;

    //消失所用的时间：踩上去后or间隔
    public float disappearTime;
    public float reappearTime;
    public float animTime;
    public float delayTime;

    public bool disappear = false;

    public SimpleEvent onDisappear;
    public SimpleEvent onReappear;

    WaitForSeconds disappearTimer, reappearTimer, animTimer, delayTimer;

    bool isDisappearing = false;

    SpriteRenderer sr;
    BoxCollider2D col;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();

        reappearTimer = new WaitForSeconds(reappearTime);
        disappearTimer = new WaitForSeconds(disappearTime);
        animTimer = new WaitForSeconds(animTime);
        delayTimer = new WaitForSeconds(delayTime);

        onDisappear.AddListener(Disappear);
        onReappear.AddListener(Reappear);

        if (disappearType == DisappearType.afterInterval)
        {
            StartCoroutine(IntervalCoroutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(disappearType == DisappearType.afterStepd)
        {

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (disappearType != DisappearType.afterStepd)
        {
            return;
        }

        if (isDisappearing)
        {
            return;
        }

        if(collision.gameObject == GameManager.Singleton.pc.gameObject && collision.transform.position.y > transform.position.y)
        {
            StartCoroutine(StepCoroutine());
        }
    }

    IEnumerator StepCoroutine()
    {
        yield return disappearTimer;
        onDisappear?.Invoke();
        disappear = true;

        yield return reappearTimer;
        onReappear?.Invoke();
        disappear = false;
        isDisappearing = false;

    }

    IEnumerator IntervalCoroutine()
    {
        yield return delayTimer;

        while (true)
        {
            yield return disappearTimer;

            //消失
            onDisappear?.Invoke();
            disappear = true;

            yield return reappearTimer;

            //出现
            onReappear?.Invoke();
            disappear = false;
        }
    }

    void Disappear()
    {
        sr.DOColor(new Color(1, 1, 1, 0), animTime);
        StartCoroutine(ColCoroutine(false));
    }

    IEnumerator ColCoroutine(bool status)
    {
        yield return animTimer;
        col.enabled = status;
    }

    void Reappear()
    {
        StartCoroutine(ColCoroutine(true));
        sr.DOColor(Color.white, animTime);
    }
}
