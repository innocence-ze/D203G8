using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TalkableNPCTriggerHandler : MonoBehaviour
{
    TalkableNpc npc;
    void Awake()
    {
        npc = GetComponentInParent<TalkableNpc>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!=null)
            npc.OnChildTriggerEnter2D(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
            npc.OnChildTriggerExit2D(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
            npc.OnChildTriggerStay2D(collision);
    }
}
