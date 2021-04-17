using UnityEngine;

[CreateAssetMenu()]
public class PCData : ScriptableObject
{
    public float currentHP;
    public float maxHP;
    
    [Header("0-walk 1-nextWall 2-wallJump 3-jump 4-doubleJump 5-dash 6-attack")]
    public bool[] Abilities = new bool[7];

}
