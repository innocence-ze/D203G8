using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDir : MonoBehaviour
{
    public void ChangeDir(Vector2 dir)
    {
        transform.localPosition = dir * 0.65f;
    }
}
