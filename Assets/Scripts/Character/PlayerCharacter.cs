using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    [Header("Frame Speed--帧速度")]
    public Vector2 frameSpeed;
    public Vector2 actualDir;

    [Header("On Ground Trigger--触地判断")]
    public LayerMask groundLayer;
    public bool onGround;
    public bool nextWall;
    public bool nextLeftWall;
    public bool nextRightWall;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public Vector2 bottomSize;
    public Vector2 size;

    [Header("Jump--跳跃")]
    public bool canJump;
    public float jumpSpeed;
    bool isJumping = false;
    bool hasJumped = false;
    public bool groundTouched;
    public float bigFallTime;
    float fallTime;

    [Header("Gravity--引力")]
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float maxFallSpeed;
    bool betterJumping = true;

    [Header("Move--移动")]
    public bool canMove;
    public float moveSpeed;
    public float accelerateTime;
    public float decelerateTime;
    public Vector2 dirOffeset;
    Vector2 dir;
    int face = 1;
    float dampVelocity;     //for smooth

    [Header("Dash 冲刺")]
    public bool canDash;
    public float dashSpeed;
    public float dashDuration;
    public float dashXMultiplier;
    public float dashDrag;
    public float dragDuration;
    bool isDashing = false;
    bool hasDashed = false;

    public bool canRecoverEnergy;
    public float dashEnergy;
    public float maxDashEnergy;
    public float currentDashEnergy;
    public float dashEnergyRecoverPerSec;
    bool dashEnergyRecovering;



    [Header("Command--命令")]
    bool jumpCommand = false;
    bool dashCommand = false;
    float moveHorCommand = 0;
    float moveVerCommand = 0;
    public bool JumpCommand { set { jumpCommand = value; } get { return jumpCommand; } }
    public bool DashCommand { set { dashCommand = value; } get { return dashCommand; } }
    public float MoveHorCommand { set { moveHorCommand = value; } get { return moveHorCommand; } }
    public float MoveVerCommand { set { moveVerCommand = value; } get { return moveVerCommand; } }

    [Header("Event List")]
    [SerializeField] SimpleEvent onJump;
    [SerializeField] FloatEvent onAir;
    [SerializeField] FloatEvent onLand;
    [SerializeField] Vec2Event onDash;
    [SerializeField] Vec2Event onMove;
    [SerializeField] Vec2Event onChangeDir;
    [SerializeField] SimpleEvent onHold;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        onGround = IsOnGround();
        nextWall = IsNextWall();

        frameSpeed = rb.velocity;
        actualDir = GetActualDir(rb.velocity);

        //jump 跳跃相关
        if (jumpCommand && canJump)
        {
            if (!hasJumped && !isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                isJumping = true;
                hasJumped = true;
                onJump?.Invoke();
            }
        }

        //
        if (!onGround)
        {
            onAir?.Invoke(rb.velocity.y);
            groundTouched = false;
            fallTime += Time.fixedDeltaTime;
        }
        else
        {
            if (!jumpCommand)
            {
                isJumping = false;
                hasJumped = false;
            }
            if (!groundTouched)
            {
                onLand?.Invoke(fallTime);
                fallTime = 0;
                groundTouched = true;
            }
            if (!isDashing)
            {
                hasDashed = false;
            }
        }
        //jump && gravity optimize  跳跃与重力的优化
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
        }
        if (rb.velocity.y <= maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }

        //move
        GetMoveDir();
        if (canMove && moveHorCommand != 0)
        {
            onMove?.Invoke(dir);
            if (moveHorCommand > 0  && !nextRightWall)
            {
                if (rb.velocity.x < moveSpeed)
                {
                    rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, moveSpeed, ref dampVelocity, accelerateTime), rb.velocity.y);
                }
            }
            else if (moveHorCommand < 0 && !nextLeftWall)
            {
                if (rb.velocity.x > -moveSpeed)
                {
                    rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, -moveSpeed, ref dampVelocity, accelerateTime), rb.velocity.y);
                }
            }
        }

        if ((moveHorCommand == 0 || !canMove) && !isDashing)
        {
            rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, 0, ref dampVelocity, decelerateTime), rb.velocity.y);
        }

        //dash
        if (dashCommand && canDash)
        {
            if (!isDashing && !hasDashed && currentDashEnergy > 0)
            { 
                onDash?.Invoke(dir);
                rb.velocity = Vector2.zero;
                Vector2 dashDir = new Vector2(dir.x * dashXMultiplier, dir.y);
                rb.velocity += dashDir.normalized * dashSpeed;
                StartCoroutine(Dashing());
                hasDashed = true;
            }
        }

        if (canRecoverEnergy && !dashEnergyRecovering)
        {
            StartCoroutine(DashEnergyRecovery());
        }

    }

    IEnumerator DashEnergyRecovery()
    {
        dashEnergyRecovering = true;
        if (currentDashEnergy < maxDashEnergy)
        {
            if (isDashing)
            {
                yield return new WaitForSeconds(1.5f);
            }
            else if (currentDashEnergy == 0)
            {
                yield return new WaitForSeconds(1.5f);
                currentDashEnergy += dashEnergyRecoverPerSec * Time.fixedDeltaTime;
            }
            else
            {
                currentDashEnergy += dashEnergyRecoverPerSec * Time.fixedDeltaTime;
            }
        }
        dashEnergyRecovering = false;
    }

    IEnumerator Dashing()
    {
        canMove = false;
        canJump = false;
        isDashing = true;
        if (currentDashEnergy > dashEnergy)
        {
            currentDashEnergy -= dashEnergy;
        }
        else if (currentDashEnergy > 0)
        {
            currentDashEnergy = 0;
        }

        yield return new WaitForSeconds(dashDuration - dragDuration);
        rb.drag = dashDrag;

        yield return new WaitForSeconds(dragDuration);
        rb.drag = 0;
        canMove = true;
        canJump = true;
        isDashing = false;
    }

    void GetMoveDir()
    {
        var oldDir = dir;
        if (moveHorCommand > dirOffeset.x) { dir.x = 1; face = 1; }
        else if (moveHorCommand < -dirOffeset.x) { dir.x = -1; face = -1; }
        else dir.x = 0;

        if (moveVerCommand > dirOffeset.y) dir.y = 1;
        else if (moveVerCommand < -dirOffeset.y) dir.y = -1;
        else dir.y = 0;

        if (dir == Vector2.zero)
        {
            if (onGround)
            {
                dir.x = face;
            }
            else
            {
                dir.y = 1;
            }
        }

        if(dir != oldDir)
        {
            onChangeDir?.Invoke(dir);
        }
    }

    public bool IsOnGround()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomSize, 0, groundLayer);
    }

    public bool IsNextWall()
    {
        nextLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, size, 0, groundLayer);
        nextRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, size, 0, groundLayer);
        return nextLeftWall || nextRightWall;
    }

    Vector2 GetActualDir(Vector2 velocity)
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
}
