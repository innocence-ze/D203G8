using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCAnimatorManager : MonoBehaviour
{
    PlayerCharacter pc;
    Animator anim;

    public string isOnGround;
    public string isNextWall;
    public string isFaceLeft;
    public string isDash;
    public string isJump;
    public string isWallJump;
    public string isDoubleJump;
    public string isHurt;
    public string isAlive;
    public string horVelocity;
    public string verVelocity;

    void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat(verVelocity, pc.frameSpeed.y);
        anim.SetFloat(horVelocity, pc.frameSpeed.x);
        anim.SetBool(isFaceLeft, pc.face == -1);
        anim.SetBool(isAlive, pc.curHp > 0);
        anim.SetBool(isOnGround, pc.onGround);
        anim.SetBool(isNextWall, pc.nextWall);
        anim.SetBool(isJump, pc.curState == PlayerCharacter.PCState.Jump);
        anim.SetBool(isDoubleJump, pc.curState == PlayerCharacter.PCState.DoubleJump);
        anim.SetBool(isDash, pc.curState == PlayerCharacter.PCState.Dash);
        anim.SetBool(isWallJump, pc.curState == PlayerCharacter.PCState.WallJump);
    }

}
