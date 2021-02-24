using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public enum PCState
    {
        Idle,
        NextWall,
        Fall,
        Move,
        Jump,
        DoubleJump,
        WallJump,
        Dash,
        Hurt,
        Die,
    };

    [Header("Frame Speed--帧速度")]
    [ConditionalShow(true)] public Vector2 frameSpeed;
    [ConditionalShow(true)] public PCState curState;

    [Header("On Ground Trigger--触地判断")]
    [ConditionalShow(true)] public bool onGround;
    public LayerMask groundLayer;
    public Vector2 bottomOffset;
    public Vector2 bottomSize;

    [Header("Next To Wall--贴墙判断")]
    [ConditionalShow(true)] public bool nextWall;
    [ConditionalShow(true)] public bool nextLeftWall;
    [ConditionalShow(true)] public bool nextRightWall;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public Vector2 sideSize;
    public bool canWallJump;
    public Vector2 wallJumpSpeed;
    public float wallFallSpeed;

    [Header("Jump--跳跃")]
    public bool canJump;
    public float jumpSpeed;
    public float jumpMoveSpeed;
    public float fallMoveSpeed;
    bool isJumping = false;
    bool hasJumped = false;

    [Header("Double Jump--二段跳")]
    public bool canDoubleJump;
    public float doubleJumpSpeed;
    public float doubleJumpMoveSpeed;
    bool hasDoubleJump = false;

    [Header("Gravity--引力")]
    public float fallMultiplier;
    public float jumpMultiplier;
    public float lowJumpMultiplier;
    public float maxFallSpeed;
    public bool betterJumping = true;

    [Header("Move--移动")]
    public bool canMove;
    public float moveSpeed;
    public float accelerate;
    public float decelerate;
    [ConditionalShow(true)] public Vector2 moveDir;
    Vector2 inputDir = Vector2.zero;
    [ConditionalShow(true)] public int face = 1;
    float dampVelocity = 0;     //for smooth

    [Header("Dash 冲刺")]
    public bool canDash;
    public float dashSpeed;
    public float dashXMultiplier;
    public float dashDrag;
    public float withoutDragDuration;
    public float stopDashSpeed;
    bool isDashing = false;
    bool hasDashed = false;

    public bool canRecoverEnergy;
    public float dashEnergy;
    public float maxDashEnergy;
    public float curDashEnergy;
    public float dashEnergyRecover;
    bool dashEnergyRecovering = false;

    [Header("Command--命令")]
    bool jumpCommand = false;
    bool dashCommand = false;
    bool moveLeftCommand = false;
    bool moveRightCommand = false;
    bool moveUpCommand = false;
    bool moveDownCommand = false;
    public bool JumpCommand { set { jumpCommand = value; } get { return jumpCommand; } }
    public bool DashCommand { set { dashCommand = value; } get { return dashCommand; } }
    public bool MoveLeftCommand { set { moveLeftCommand = value; if (value) moveRightCommand = !value; } get { return moveLeftCommand; } }
    public bool MoveRightCommand { set { moveRightCommand = value; if (value) MoveLeftCommand = !value; } get { return moveRightCommand; } }
    public bool MoveUpCommand { set { moveUpCommand = value; if (value) moveDownCommand = !value; } get { return moveUpCommand; } }
    public bool MoveDownCommand { set { moveDownCommand = value; if (value) moveUpCommand = !value; } get { return moveDownCommand; } }

    [Header("Event List")]
    [SerializeField] SimpleEvent onJump;
    [SerializeField] SimpleEvent onDoubleJump;
    [SerializeField] SimpleEvent onWallJump;
    [SerializeField] FloatEvent onAir;
    [SerializeField] FloatEvent onLand;
    [SerializeField] Vec2Event onDash;
    [SerializeField] Vec2Event onMove;
    [SerializeField] Vec2Event onChangeDir;
    [SerializeField] SimpleEvent onIdle;

    Rigidbody2D rb;
    readonly WaitForFixedUpdate continueState = new WaitForFixedUpdate();
    Coroutine stateCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        stateCoroutine = StartCoroutine(IdleState());
        curDashEnergy = maxDashEnergy;
    }

    protected override void Update()
    {
        base.Update();
        if (nextLeftWall && nextRightWall)
        {
            hasJumped = false;
            hasDoubleJump = false;
        }

        if (!jumpCommand)
        {
            isJumping = false;
        }

        if (!dashCommand)
        {
            isDashing = false;
        }

        if (canRecoverEnergy && !dashEnergyRecovering)
        {
            StartCoroutine(DashEnergyRecovery());
        }
    }

    private void FixedUpdate()
    {
        onGround = IsOnGround();
        nextWall = IsNextWall();

        frameSpeed = rb.velocity;
        moveDir = GetMoveDir(rb.velocity);
        GetInputDir();

    }

    IEnumerator IdleState()
    {
        curState = PCState.Idle;
        onIdle?.Invoke();
        OnGround();

        while (true)
        {
            AccelerateSpeed(0, decelerate);

            yield return continueState;
            if (JumpCondition)
            {
                stateCoroutine = StartCoroutine(JumpState());
                yield break;
            }
            if (DashCondition)
            {
                stateCoroutine = StartCoroutine(DashState());
                yield break;
            }
            if (canMove && (MoveRightCommand || moveLeftCommand))
            {
                stateCoroutine = StartCoroutine(MoveState());
                yield break;
            }
            if (FallCondition)
            {
                stateCoroutine = StartCoroutine(FallState());
                yield break;
            }
            if (HurtCondition)
            {
                stateCoroutine = StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    IEnumerator MoveState()
    {
        curState = PCState.Move;
        onMove?.Invoke(moveDir);
        OnGround();
        while (true)
        {
            if (canMove && moveRightCommand && !nextRightWall)
            {
                if (rb.velocity.x < moveSpeed)
                {
                    AccelerateSpeed(moveSpeed, accelerate);
                }
            }
            else if (canMove && moveLeftCommand && !nextLeftWall)
            {
                if (rb.velocity.x > -moveSpeed)
                {
                    AccelerateSpeed(-moveSpeed, accelerate);
                }
            }
            yield return continueState;

            if (JumpCondition)
            {
                stateCoroutine = StartCoroutine(JumpState());
                yield break;
            }
            if (DashCondition)
            {
                stateCoroutine = StartCoroutine(DashState());
                yield break;
            }
            if (!(moveLeftCommand || moveRightCommand))
            {
                stateCoroutine = StartCoroutine(IdleState());
                yield break;
            }
            if (FallCondition)
            {
                stateCoroutine = StartCoroutine(FallState());
                yield break;
            }
            if (HurtCondition)
            {
                stateCoroutine = StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    IEnumerator DashState()
    {
        curState = PCState.Dash;
        onDash?.Invoke(inputDir);

        invincible = true;
        canMove = false;
        isDashing = true;
        rb.velocity = Vector2.zero;
        Vector2 dashDir = new Vector2(inputDir.x * dashXMultiplier, inputDir.y);
        rb.velocity += dashDir.normalized * dashSpeed;
        yield return new WaitForSeconds(withoutDragDuration);

        while (true)
        {
            rb.drag = dashDrag;
            yield return continueState;

            if (rb.velocity.sqrMagnitude < stopDashSpeed * stopDashSpeed)
            {
                if (onGround)
                {
                    EndDash(IdleState());
                    yield break;
                }
                else if (NextWallCondition)
                {
                    EndDash(NextWallState());
                    yield break;
                }
                else if (FallCondition)
                {
                    EndDash(FallState());
                    yield break;
                }

                if (JumpCondition)
                {
                    EndDash(JumpState());
                    yield break;
                }
                if (DoubleJumpCondition)
                {
                    EndDash(DoubleJumpState());
                    yield break;
                }

            }
        }
    }

    IEnumerator JumpState()
    {
        curState = PCState.Jump;
        onJump?.Invoke();
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        isJumping = true;
        yield return continueState;

        while (true)
        {
            if (!jumpCommand)
            {
                hasJumped = true;
            }
            BetterJump();
            MoveInAir(jumpMoveSpeed, accelerate, decelerate);

            yield return continueState;

            if (onGround)
            {
                EndJump(IdleState());
                yield break;
            }
            if (NextWallCondition)
            {
                EndJump(NextWallState());
                yield break;
            }
            if (FallCondition)
            {
                EndJump(FallState());
                yield break;
            }

            if (DashCondition)
            {
                EndJump(DashState());
                yield break;
            }

            if (DoubleJumpCondition)
            {
                EndJump(DoubleJumpState());
                yield break;
            }
            if (HurtCondition)
            {
                EndJump(HurtState());
                yield break;
            }
        }
    }

    IEnumerator DoubleJumpState()
    {
        curState = PCState.DoubleJump;
        onDoubleJump?.Invoke();
        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpSpeed);

        while (true)
        {
            if (!jumpCommand)
            {
                hasDoubleJump = true;
            }
            BetterJump();
            MoveInAir(doubleJumpMoveSpeed, accelerate, decelerate);

            yield return continueState;
            if (onGround)
            {
                EndDoubleJump(IdleState());
                yield break;
            }
            else if (NextWallCondition)
            {
                EndDoubleJump(NextWallState());
                yield break;
            }
            else if (FallCondition)
            {
                EndDoubleJump(FallState());
                yield break;
            }

            if (DashCondition)
            {
                EndDoubleJump(DashState());
                yield break;
            }
            if (HurtCondition)
            {
                EndDoubleJump(HurtState());
                yield break;
            }
        }
    }

    IEnumerator NextWallState()
    {
        curState = PCState.NextWall;
        hasJumped = false;
        hasDoubleJump = false;
        while (true)
        {

            rb.velocity = new Vector2(rb.velocity.x, wallFallSpeed);
            if (!nextLeftWall && MoveLeftCommand && canMove)
            {
                if (rb.velocity.x > -moveSpeed) AccelerateSpeed(-moveSpeed, accelerate);
            }

            if (!nextRightWall && MoveRightCommand && canMove)
            {
                if (rb.velocity.x < moveSpeed) AccelerateSpeed(moveSpeed, accelerate);
            }

            yield return continueState;
            if (onGround)
            {
                stateCoroutine = StartCoroutine(IdleState());
                yield break;
            }
            else if (!nextWall || (nextLeftWall && nextRightWall))
            {
                stateCoroutine = StartCoroutine(FallState());
                yield break;
            }

            if (DashCondition)
            {
                stateCoroutine = StartCoroutine(DashState());
                yield break;
            }
            if (WallJumpCondition)
            {
                if (nextLeftWall && !nextRightWall) rb.velocity = wallJumpSpeed;
                else if (nextRightWall && !nextLeftWall) rb.velocity = new Vector2(wallJumpSpeed.x * -1, wallJumpSpeed.y);
                stateCoroutine = StartCoroutine(WallJumpState());
                yield break;
            }
            if (HurtCondition)
            {
                stateCoroutine = StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    IEnumerator WallJumpState()
    {
        curState = PCState.WallJump;
        isJumping = true;
        onWallJump?.Invoke();
        yield return continueState;
        while (true)
        {
            if (!jumpCommand)
            {
                hasJumped = true;
            }
            BetterJump();
            //MoveInAir(jumpMoveSpeed, accelerate, 0.1f);

            yield return continueState;
            if (onGround)
            {
                EndJump(IdleState());
                yield break;
            }
            else if (NextWallCondition)
            {
                EndJump(NextWallState());
                yield break;
            }
            else if (FallCondition)
            {
                EndJump(FallState());
                yield break;
            }

            if (DashCondition)
            {
                EndJump(DashState());
                yield break;
            }
            if (HurtCondition)
            {
                EndJump(HurtState());
                yield break;
            }
        }
    }

    IEnumerator FallState()
    {
        curState = PCState.Fall;
        onAir?.Invoke(-1);
        while (true)
        {
            BetterJump();
            MoveInAir(fallMoveSpeed, accelerate, decelerate);

            yield return continueState;
            if (onGround)
            {
                stateCoroutine = StartCoroutine(IdleState());
                yield break;
            }
            else if (NextWallCondition)
            {
                stateCoroutine = StartCoroutine(NextWallState());
                yield break;
            }

            if (DashCondition)
            {
                stateCoroutine = StartCoroutine(DashState());
                yield break;
            }
            if (JumpCondition)
            {
                stateCoroutine = StartCoroutine(JumpState());
                yield break;
            }
            if (DoubleJumpCondition)
            {
                stateCoroutine = StartCoroutine(DoubleJumpState());
                yield break;
            }
            if (HurtCondition)
            {
                stateCoroutine = StartCoroutine(HurtState());
                yield break;
            }
        }
    }

    IEnumerator HurtState()
    {
        getHurt = false;
        curState = PCState.Hurt;
        OnHurt?.Invoke((Vector2)hurtInfo[0] - (Vector2)transform.position);
        GetHurt((float)hurtInfo[1]);
        //受伤后无敌
        StartCoroutine(InvincibleCoroutine());
        //受伤后硬直时间
        yield return new WaitForSeconds(hurtRecoverTime);
        while (true)
        {
            yield return continueState;
            if (!IsAlive)
            {
                stateCoroutine = StartCoroutine(DieState());
                yield break;
            }
            else
            {
                if (onGround)
                {
                    stateCoroutine = StartCoroutine(IdleState());
                    yield break;
                }
                else if (nextWall)
                {
                    stateCoroutine = StartCoroutine(NextWallState());
                    yield break;
                }
                else
                {
                    if(rb.velocity.y > 0)
                        rb.velocity = new Vector2(rb.velocity.x, 0);
                    stateCoroutine = StartCoroutine(FallState());
                    yield break;
                }
            }
        }
    }

    IEnumerator DieState()
    {
        curState = PCState.Die;
        OnDie?.Invoke();
        rb.velocity = Vector2.zero;

        while (true)
        {

            yield return continueState;
            if (IsAlive)
            {
                transform.position = bornPos;
                stateCoroutine = StartCoroutine(IdleState());
                yield break;
            }
        }
    }

    IEnumerator DashEnergyRecovery()
    {
        dashEnergyRecovering = true;
        if (curDashEnergy < maxDashEnergy)
        {
            if (isDashing)
            {
                yield return new WaitForSeconds(1.5f);
            }
            else if (curDashEnergy == 0)
            {
                yield return new WaitForSeconds(1.5f);
                curDashEnergy += dashEnergyRecover * Time.fixedDeltaTime;
            }
            else
            {
                curDashEnergy += dashEnergyRecover * Time.fixedDeltaTime;
            }
        }
        dashEnergyRecovering = false;
    }

    public bool JumpCondition => !isJumping && jumpCommand && canJump && !hasJumped;

    public bool DoubleJumpCondition => canDoubleJump && hasJumped && !hasDoubleJump && jumpCommand && !isJumping;

    public bool WallJumpCondition => canWallJump && jumpCommand && !isJumping;

    public bool DashCondition => dashCommand && canDash && !hasDashed && !isDashing;

    public bool HurtCondition => getHurt && !invincible;

    bool FallCondition => rb.velocity.y < 0 && !onGround && !nextWall;

    bool NextWallCondition
    {
        get
        {
            bool l = nextLeftWall && !nextRightWall;
            bool r = !nextLeftWall && nextRightWall;

            return rb.velocity.y < 0 && (l || r) && !onGround;
        }
    }

    void EndDash(IEnumerator nextState)
    {
        canMove = true;
        hasDashed = true;
        invincible = false;
        rb.drag = 0;
        stateCoroutine = StartCoroutine(nextState);
    }

    void EndJump(IEnumerator nextState)
    {
        //isJumping = false;
        hasJumped = true;
        stateCoroutine = StartCoroutine(nextState);
    }

    void EndDoubleJump(IEnumerator nextState)
    {
        //isDoubleJumping = false;
        hasDoubleJump = true;
        stateCoroutine = StartCoroutine(nextState);
    }

    //x轴的加减速 rb.velocity.x
    void AccelerateSpeed(float targetSpeed, float accelerate)
    {
        float time = Mathf.Abs(targetSpeed - rb.velocity.x) / accelerate;
        var x = Mathf.SmoothDamp(rb.velocity.x, targetSpeed, ref dampVelocity, time);
        rb.velocity = new Vector2(x, rb.velocity.y);
    }

    void OnGround()
    {
        hasDashed = false;
        hasJumped = false;
        hasDoubleJump = false;
    }

    //根据输入和当前朝向获取dir
    void GetInputDir()
    {
        var oldDir = inputDir;
        if (moveRightCommand) { inputDir.x = 1; face = 1; }
        else if (MoveLeftCommand) { inputDir.x = -1; face = -1; }
        else inputDir.x = 0;

        if (moveUpCommand) inputDir.y = 1;
        else if (moveDownCommand) inputDir.y = -1;
        else inputDir.y = 0;

        if (inputDir == Vector2.zero)
        {
            if (onGround)
            {
                inputDir.x = face;
            }
            else
            {
                inputDir.y = rb.velocity.y >= 0 ? 1 : -1;
            }
        }

        if(nextWall && !onGround)
        {
            if (nextLeftWall && !nextRightWall) face = 1;
            else if (nextRightWall && !nextLeftWall) face = -1;
        }

        if (inputDir != oldDir)
        {
            onChangeDir?.Invoke(inputDir);
        }
    }

    public bool IsOnGround() => Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomSize, 0, groundLayer);

    public bool IsNextWall()
    {
        nextLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, sideSize, 0, groundLayer);
        nextRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, sideSize, 0, groundLayer);
        return nextLeftWall || nextRightWall;
    }

    //根据速度获取实际运动方向
    Vector2 GetMoveDir(Vector2 velocity)
    {
        float vx = velocity.x, vy = velocity.y;
        float ax = 0, ay = 0;
        if (Mathf.Abs(vx) > Mathf.Abs(vy))
        {
            if (vx > 0.05f) ax = 1;
            else if (vx < -0.05) ax = -1;
        }
        else if (Mathf.Abs(vy) > Mathf.Abs(vx))
        {
            if (vy > 0.05f) ay = 1;
            else if (vy < -0.05) ay = -1;
        }
        else
        {
            if (vx > 0.05f) ax = 1;
            else if (vx < -0.05) ax = -1;
            if (vy > 0.05f) ay = 1;
            else if (vy < -0.05) ay = -1;
        }
        return new Vector2(ax, ay);
    }

    void BetterJump()
    {
        if (betterJumping)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0 && !jumpCommand)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
            else
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (jumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
        if (rb.velocity.y <= maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }

    void MoveInAir(float moveSpeed, float acc, float dec)
    {
        if (canMove)
        {
            if (moveRightCommand)
            {
                if (rb.velocity.x < moveSpeed) AccelerateSpeed(moveSpeed, acc);
            }
            else if (moveLeftCommand)
            {
                if (rb.velocity.x > -moveSpeed) AccelerateSpeed(-moveSpeed, acc);
            }
            else
            {
                AccelerateSpeed(0, dec);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Button(curState.ToString());
    }

}
