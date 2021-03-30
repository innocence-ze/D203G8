using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCAnimatorManager : MonoBehaviour
{
    PlayerCharacter pc;
    Animator anim;
    SpriteRenderer sr;

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
    public string ComboNum;
    public string AttackNormalizedTime;
    public string isLand;

    
    void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        anim.SetFloat(verVelocity, pc.frameSpeed.y);
        anim.SetFloat(horVelocity,Mathf.Abs(pc.frameSpeed.x));
        anim.SetBool(isFaceLeft, pc.face == -1);
        anim.SetBool(isAlive, pc.IsAlive);
        anim.SetBool(isOnGround, pc.onGround);
        anim.SetBool(isNextWall, pc.nextWall);
        anim.SetBool(isJump, pc.curState == PlayerCharacter.PCState.Jump);
        anim.SetBool(isDoubleJump, pc.curState == PlayerCharacter.PCState.DoubleJump);
        anim.SetBool(isDash, pc.curState == PlayerCharacter.PCState.Dash);
        anim.SetBool(isWallJump, pc.curState == PlayerCharacter.PCState.WallJump);
        anim.SetBool(isHurt, pc.curState == PlayerCharacter.PCState.Hurt);
        anim.SetBool(isLand, pc.isLanding);

        
        anim.SetInteger(ComboNum, pc.comboNum);


        pc.attackAnimNormalizedTime = anim.GetFloat(AttackNormalizedTime);

        sr.flipX = pc.face == -1;
    }



}
