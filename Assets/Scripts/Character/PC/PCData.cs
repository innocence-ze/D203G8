using UnityEngine;

[CreateAssetMenu()]
public class PCData : ScriptableObject
{
    public float currentHP;
    public float maxHP;
    
    [Header("0-walk 1-nextWall 2-wallJump 3-jump 4-doubleJump 5-dash 6-attack")]
    public bool[] Abilities = new bool[7];

    public int continueSceneIndex;

    public float x, y, z;

    public void Copy(PCData from)
    {
        currentHP = from.currentHP;
        maxHP = from.maxHP;
        for(int i = 0; i < Abilities.Length; i++)
        {
            Abilities[i] = from.Abilities[i];
        }
        continueSceneIndex = from.continueSceneIndex;
        x = from.x;
        y = from.y;
        z = from.z;
    }

}
