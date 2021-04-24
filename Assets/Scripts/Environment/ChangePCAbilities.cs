using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChangePCAbilities : MonoBehaviour
{
    public bool value;
    public PlayerCharacter.PCAbilities ability;

    public SimpleEvent OnChangeAbility;

    public GameObject panelUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pc = GameManager.Singleton.pc;
        if (collision.gameObject == pc.gameObject)
        {
            if(pc.GetAbility(ability) != value)
            {
                pc.SetAbility(ability, value);
                GetComponent<SpriteRenderer>().DOFade(0, 0.3f);

                if (panelUI != null)
                {
                    panelUI.SetActive(true);
                    StartCoroutine("TextDisappear");
                }
            }
        }
    }

    private IEnumerator TextDisappear()
    {
        yield return new WaitForSeconds(5);
        panelUI.SetActive(false);
        yield break;
    }
}
