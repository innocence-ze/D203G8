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
        land,
        Dash,
        Hurt,
        Die,
        Attack1,
        Attack2,
        Attack3,
    };

    public float rebornTime; //死亡后多久重生

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
    public bool canNextWall;
    public bool canWallJump;
    public Vector2 wallJumpSpeed;
    public float wallFallSpeed;

    [Header("Jump--跳跃")]
    public bool canJump;
    public float jumpSpeed;
    public float jumpMoveSpeed;
    public float fallMoveSpeed;
    public float bigFallTime;
    public bool isLanding;
    float fallTime = 0;
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

    [Header("Attack--攻击")]
    public bool canAttack;
    public float attackMoveSpeed;
    public List<float> attackDamages = new List<float>();
    public List<Transform> attackPos = new List<Transform>();
    public List<float> attackRadius = new List<float>();
    public LayerMask attackableLayer;
    [ConditionalShow(true)] public int comboNum = 0;
    List<Collider2D> totalHitCollider = new List<Collider2D>();
    [ConditionalShow(true)] [SerializeField] bool inCombo = false;
    [ConditionalShow(true)] public float attackAnimNormalizedTime;
    public bool nextCombo = false;

    [Header("Command--命令")]
    bool moveLeftCommand = false;
    bool moveRightCommand = false;
    bool moveUpCommand = false;
    bool moveDownCommand = false;
    public bool MoveLeftCommand { set { moveLeftCommand = value; if (value) moveRightCommand = !value; } get { return moveLeftCommand; } }
    public bool MoveRightCommand { set { moveRightCommand = value; if (value) MoveLeftCommand = !value; } get { return moveRightCommand; } }
    public bool MoveUpCommand { set { moveUpCommand = value; if (value) moveDownCommand = !value; } get { return moveUpCommand; } }
    public bool MoveDownCommand { set { moveDownCommand = value; if (value) moveUpCommand = !value; } get { return moveDownCommand; } }
    public bool JumpCommand { get; set; }
    public bool DashCommand { private get => GetCommand("Dash"); set => SetCommand("Dash"); }
    public bool AttackCommand { private get => GetCommand("Attack"); set => SetCommand("Attack"); }
    public bool InteractCommand { get => GetCommand("Interact"); set => SetCommand("Interact"); }

    HashSet<string> commandSet = new HashSet<string>();
    public void ClearCommandSet() => commandSet.Clear();
    bool GetCommand(string command) => commandSet.Remove(command);
    public void SetCommand(string command) => commandSet.Add(command);

    [Header("Event List")]
    [SerializeField] SimpleEvent onJump;
    [SerializeField] SimpleEvent onDoubleJump;
    [SerializeField] SimpleEvent onWallJump;
    [SerializeField] FloatEvent onAir;
    [SerializeField] Vec2Event onDash;
    [SerializeField] Vec2Event onMove;
    [SerializeField] SimpleEvent onIdle;
    [SerializeField] IntEvent onAttack;//开始攻击时，无论是否攻击到
    [SerializeField] Vec2Event onAttackObj;//攻击到物体时

    Coroutine stateCoroutine;

    /// <summary>
    /// walk, next wall, wall jump, jump, double jump, dash, attack
    /// 走，贴墙，墙跳，跳，双跳，冲，攻击
    /// </summary>
    public bool[] Abilities { get; private set; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        stateCoroutine = StartCoroutine(IdleState());
        curDashEnergy = maxDashEnergy;

        onChangeDir.AddListener(ChangeAttackPos);
        Abilities = new bool[7]
        {
            canMove, canNextWall, canWallJump, canJump, canDoubleJump, canDash, canAttack,
        };
    }

    protected override void Update()
    {
        base.Update();
        if (nextLeftWall && nextRightWall)
        {
            hasJumped = false;
            hasDoubleJump = false;
        }

        if (!JumpCommand)
        {
            isJumping = false;
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


    /////////////////////////////////////////////////////
    /// 状态机
    /////////////////////////////////////////////////////


    IEnumerator IdleState()
    {
        OnEnterState(PCState.Idle);
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
            if (AttackCondition)
            {
                StartCoroutine(Attack01State());
                yield break;
            }
        }
    }

    IEnumerator MoveState()
    {
        OnEnterState(PCState.Move);
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
            if (AttackCondition)
            {
                StartCoroutine(Attack01State());
                yield break;
            }
        }
    }

    IEnumerator DashState()
    {
        OnEnterState(PCState.Dash);
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
        OnEnterState(PCState.Jump);
        onJump?.Invoke();
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        isJumping = true;
        yield return continueState;

        while (true)
        {
            if (!JumpCommand)
            {
                hasJumped = true;
            }
            BetterJump();
            PlayerMove(jumpMoveSpeed, accelerate, decelerate);

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
            if (AttackCondition)
            {
                EndJump(Attack01State());
                yield break;
            }
        }
    }

    IEnumerator DoubleJumpState()
    {
        OnEnterState(PCState.DoubleJump);
        onDoubleJump?.Invoke();
        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpSpeed);

        while (true)
        {
            if (!JumpCommand)
            {
                hasDoubleJump = true;
            }
            BetterJump();
            PlayerMove(doubleJumpMoveSpeed, accelerate, decelerate);

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
            if (AttackCondition)
            {
                EndDoubleJump(Attack01State());
                yield break;
            }
        }
    }

    IEnumerator NextWallState()
    {
        OnEnterState(PCState.NextWall);
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
        OnEnterState(PCState.WallJump);
        isJumping = true;
        onWallJump?.Invoke();
        yield return continueState;
        while (true)
        {
            if (!JumpCommand)
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
            if (AttackCondition)
            {
                EndJump(Attack01State());
                yield break;
            }
        }
    }

    IEnumerator FallState()
    {
        OnEnterState(PCState.Fall);
        onAir?.Invoke(-1);
        while (true)
        {
            BetterJump();
            PlayerMove(fallMoveSpeed, accelerate, decelerate);
            fallTime += Time.fixedDeltaTime;

            yield return continueState;
            if (onGround)
            {
                if (fallTime >= bigFallTime)
                {
                    isLanding = true;
                    fallTime = 0;
                    stateCoroutine = StartCoroutine(LandState());
                    yield break;
                }
                fallTime = 0;
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
            if (AttackCondition)
            {
                StartCoroutine(Attack01State());
                yield break;
            }
        }
    }

    IEnumerator LandState()
    {
        while (true)
        {
            yield return continueState;
            if (canMove && (MoveRightCommand || moveLeftCommand))
            {
                stateCoroutine = StartCoroutine(MoveState());
                yield break;
            }
            if(!isLanding)
            {
                stateCoroutine = StartCoroutine(IdleState());
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
            if (HurtCondition)
            {
                stateCoroutine = StartCoroutine(HurtState());
                yield break;
            }
            if (AttackCondition)
            {
                StartCoroutine(Attack01State());
                yield break;
            }
        }
    }

    IEnumerator HurtState()
    {
        getHurt = false;
        OnEnterState(PCState.Hurt);
        onHurt?.Invoke((Vector2)hurtInfo[0] - (Vector2)transform.position);
        GetHurt((float)hurtInfo[1]);
        rb.velocity = Vector2.zero;
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
                    if (rb.velocity.y > 0)
                        rb.velocity = new Vector2(rb.velocity.x, 0);
                    stateCoroutine = StartCoroutine(FallState());
                    yield break;
                }
            }
        }
    }

    IEnumerator DieState()
    {
        OnEnterState(PCState.Die);
        onDie?.Invoke();
        rb.velocity = Vector2.zero;
        invincible = true;

        while (true)
        {

            yield return continueState;
            if (IsAlive)
            {
                invincible = false;
                transform.position = bornPos;
                stateCoroutine = StartCoroutine(IdleState());
                yield break;
            }
        }
    }

    IEnumerator Attack01State()
    {
        comboNum = 1;
        EnterAttack(1);
        var timer = StartCoroutine(ResetAttackTimer());
        while (true)
        {
            InAttack(1);

            yield return 0;
            if (comboNum == 2)
            {
                StopCoroutine(timer);
                totalHitCollider.Clear();
                StartCoroutine(Attack02State());
                yield break;
            }
            if (JumpCondition)
            {
                EndAttack(JumpState());
                yield break;
            }
            if (DoubleJumpCondition)
            {
                EndAttack(DoubleJumpState());
                yield break;
            }
            if (DashCondition)
            {
                EndAttack(DashState());
                yield break;
            }
            if (HurtCondition)
            {
                EndAttack(HurtState());
                yield break;
            }
            if (!inCombo)
            {
                if (onGround)
                {
                    EndAttack(IdleState());
                    yield break;
                }
                if (NextWallCondition)
                {
                    EndAttack(NextWallState());
                    yield break;
                }
                if (FallCondition)
                {
                    EndAttack(FallState());
                    yield break;
                }
            }

        }
    }

    IEnumerator Attack02State()
    {
        EnterAttack(2);
        var timer = StartCoroutine(ResetAttackTimer());
        while (true)
        {
            InAttack(2);

            yield return 0;
            if (comboNum == 3)
            {
                StopCoroutine(timer);
                totalHitCollider.Clear();
                StartCoroutine(Attack03State());
                yield break;
            }
            if (JumpCondition)
            {
                EndAttack(JumpState());
                yield break;
            }
            if (DoubleJumpCondition)
            {
                EndAttack(DoubleJumpState());
                yield break;
            }
            if (DashCondition)
            {
                EndAttack(DashState());
                yield break;
            }
            if (HurtCondition)
            {
                EndAttack(HurtState());
                yield break;
            }
            if (!inCombo)
            {
                if (onGround)
                {
                    EndAttack(IdleState());
                    yield break;
                }
                if (NextWallCondition)
                {
                    EndAttack(NextWallState());
                    yield break;
                }
                if (FallCondition)
                {
                    EndAttack(FallState());
                    yield break;
                }
            }

        }
    }

    IEnumerator Attack03State()
    {
        EnterAttack(3);
        var timer = StartCoroutine(ResetAttackTimer());
        while (true)
        {
            InAttack(3);

            yield return 0;
            if (comboNum == 1)
            {
                StopCoroutine(timer);
                totalHitCollider.Clear();
                StartCoroutine(Attack01State());
                yield break;
            }
            if (JumpCondition)
            {
                EndAttack(JumpState());
                yield break;
            }
            if (DoubleJumpCondition)
            {
                EndAttack(DoubleJumpState());
                yield break;
            }
            if (DashCondition)
            {
                EndAttack(DashState());
                yield break;
            }
            if (HurtCondition)
            {
                EndAttack(HurtState());
                yield break;
            }
            if (!inCombo)
            {
                if (onGround)
                {
                    EndAttack(IdleState());
                    yield break;
                }
                if (NextWallCondition)
                {
                    EndAttack(NextWallState());
                    yield break;
                }
                if (FallCondition)
                {
                    EndAttack(FallState());
                    yield break;
                }
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

    //重置连击时间
    IEnumerator ResetAttackTimer()
    {
        inCombo = true;
        while (attackAnimNormalizedTime < 1)
        {
            yield return 0;
        }
        inCombo = false;
    }


    //////////////////////////////////////////////////////////////
    ///进入状态机的条件
    //////////////////////////////////////////////////////////////


    public bool JumpCondition => !isJumping && JumpCommand && canJump && !hasJumped;

    public bool DoubleJumpCondition => canDoubleJump && hasJumped && !hasDoubleJump && JumpCommand && !isJumping;

    public bool WallJumpCondition => canWallJump && JumpCommand && !isJumping;

    public bool DashCondition => DashCommand && canDash && !hasDashed && !isDashing;

    public bool HurtCondition => getHurt && !invincible;

    bool FallCondition => rb.velocity.y < 0 && !onGround && !nextWall;

    bool NextWallCondition
    {
        get
        {
            bool l = nextLeftWall && !nextRightWall;
            bool r = !nextLeftWall && nextRightWall;

            return rb.velocity.y < 0 && (l || r) && !onGround && canNextWall;
        }
    }

    bool AttackCondition => canAttack && AttackCommand;


    /////////////////////////////////////////////////////////////////
    ///进入状态机的动作
    /////////////////////////////////////////////////////////////////


    void OnEnterState(PCState cur)
    {
        curState = cur;
        ClearCommandSet();
    }

    void EnterAttack(int attackNum)
    {
        OnEnterState(PCState.Attack1 + attackNum - 1);
        onAttack?.Invoke(attackNum);
    }

    void InAttack(int attackNum)
    {
        BetterJump();
        PlayerMove(attackMoveSpeed, accelerate, decelerate);

        //物理判断
        Collider2D[] coll = Physics2D.OverlapCircleAll(attackPos[attackNum - 1].position, attackRadius[attackNum - 1], attackableLayer);

        for (int i = 0; i < coll.Length; i++)
        {
            if (!CompareAttackObject(totalHitCollider, coll[i]))
            {
                totalHitCollider.Add(coll[i]);
                onAttackObj?.Invoke(coll[i].transform.position);

                var enemy = coll[i].GetComponent<IHurtable>();
                if (enemy != null)
                {
                    enemy.SetHurtInfo(new object[2]
                    {
                        (Vector2)transform.position,
                        attackDamages[attackNum-1]
                    });
                }
            }
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    ///结束状态机的动作
    ////////////////////////////////////////////////////////////////////////////////////////


    void EndDash(IEnumerator nextState)
    {
        canMove = true;
        hasDashed = true;
        invincible = false;
        isDashing = false;
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

    void EndAttack(IEnumerator nextState)
    {
        comboNum = 0;
        inCombo = false;
        totalHitCollider.Clear();
        StartCoroutine(nextState);
    }





    //转身时，修改attack的localpos
    void ChangeAttackPos(Vector2 dir)
    {
        if (dir.x * attackPos[0].localPosition.x < 0)
        {
            for (int i = 0; i < attackPos.Count; i++)
            {
                attackPos[i].localPosition = new Vector3(-attackPos[i].localPosition.x, attackPos[i].localPosition.y, 0);
            }
        }
    }

    bool CompareAttackObject(List<Collider2D> total, Collider2D other)
    {
        for (int i = 0; i < total.Count; i++)
        {
            if (total[i] == other)
            {
                return true;
            }
        }
        return false;
    }

    //x轴的加减速 rb.velocity.x
    void AccelerateSpeed(float targetSpeed, float accelerate)
    {
        float time = Mathf.Abs(targetSpeed - rb.velocity.x) / accelerate;
        float dampVelocity = 0;
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

        if (nextWall && !onGround)
        {
            int oldface = face;
            if (nextLeftWall && !nextRightWall) face = 1;
            else if (nextRightWall && !nextLeftWall) face = -1;
            if (oldface != face)
            {
                inputDir = new Vector2(face, -1);
            }
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
            else if (rb.velocity.y > 0 && !JumpCommand)
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

    void PlayerMove(float moveSpeed, float acc, float dec)
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

    public void FinishLand()
    {
        isLanding = false;
    }

    public void GotoNextCombo(int nextCombo)
    {
        if (AttackCondition)
        {
            comboNum = nextCombo;
        }
    }

    public void Reborn()
    {
        curHp = maxHp;
    }

    public void Lock()
    {
        canMove = canNextWall = canWallJump = canJump = canDoubleJump = canDash = canAttack = false;
    }

    public void Unlock()
    {
        canMove = Abilities[0];
        canNextWall = Abilities[1]; 
        canWallJump = Abilities[2]; 
        canJump = Abilities[3];
        canDoubleJump = Abilities[4]; 
        canDash = Abilities[5]; 
        canAttack = Abilities[6];
    }

    public void SetAbilities(int index, bool value)
    {
        if (index > 6 || index < 0)
            return;
        Abilities[index] = value;
        Unlock();
    }

    public bool GetAbilities(int index)
    {
        if (index > 6 || index < 0)
            return false;
        return Abilities[index];
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.Button(curState.ToString());
        GUILayout.Label($"inCombo:{inCombo}");
        GUILayout.Label($"Combo:{comboNum}");
        GUILayout.Label($"AttTime:{attackAnimNormalizedTime}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, sideSize);
        Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, sideSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube((Vector2)transform.position + bottomOffset, bottomSize);

        for (int i = 0; i < attackPos.Count; i++)
        {
            Gizmos.DrawWireSphere(attackPos[i].position, attackRadius[i]);
        }
    }
#endif
}
