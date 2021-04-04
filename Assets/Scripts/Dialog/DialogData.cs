using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DialogData : ScriptableObject
{
    public List<DialogContent> contents;
}

[System.Serializable]
public class DialogContent
{
    [TextArea]
    public string dialogText;
}