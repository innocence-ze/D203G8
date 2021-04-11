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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pc = GameManager.Singleton.pc;
        if (collision.gameObject == pc.gameObject)
        {
            if(pc.GetAbilities(ability) != value)
            {
                pc.SetAbilities(ability, value);
                GetComponent<SpriteRenderer>().DOFade(0, 0.3f);
            }
        }
    }

}
