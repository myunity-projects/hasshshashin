using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region StateVariables

    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerAttackState PrimaryAttackState { get; private set; }
    public PlayerAttackState SecondaryAttackState { get; private set; }

    [SerializeField] private PlayerData playerData;

    #endregion

    #region Components

    public Animator Animator { get; private set; }

    public PlayerInputHandler InputHandler { get; private set; }

    public Rigidbody2D PlayerRigidbody2D { get; private set; }

    #endregion

    #region Check Transforms

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;

    #endregion

    #region Other Variables

    public Vector2 CurrentVelocity { get; private set; }

    public int FacingDirection { get; private set; }      

    private Vector2 workspace;

    #endregion

    #region Animations Names Variables

    private string idleAnimName = "idle";
    private string moveAnimName = "move";
    private string jumpAnimName = "inAir";
    private string landAnimName = "land";
    private string wallGrabAnimName = "wallGrab";
    private string wallClimbAnimName = "wallClimb";
    private string wallSlideAnimName = "wallSlide";    
    private string ledgeClimbAnimName = "ledgeClimbState";
    private string attackAnimName = "attack";    

    #endregion

    #region Unity Callback Functions

    private void Awake()
    {        
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, idleAnimName);
        MoveState = new PlayerMoveState(this, StateMachine, playerData, moveAnimName);
        JumpState = new PlayerJumpState(this, StateMachine, playerData, jumpAnimName);
        InAirState = new PlayerInAirState(this, StateMachine, playerData, jumpAnimName);
        LandState = new PlayerLandState(this, StateMachine, playerData, landAnimName);
        WallGrabState = new PlayerWallGrabState(this, StateMachine, playerData, wallGrabAnimName);
        WallClimbState = new PlayerWallClimbState(this, StateMachine, playerData, wallClimbAnimName);
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, wallSlideAnimName);
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, jumpAnimName);
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, ledgeClimbAnimName);
        PrimaryAttackState = new PlayerAttackState(this, StateMachine, playerData, attackAnimName);
        SecondaryAttackState = new PlayerAttackState(this, StateMachine, playerData, attackAnimName);
    }

    private void Start()
    {
        Animator = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        PlayerRigidbody2D = GetComponent<Rigidbody2D>();

        FacingDirection = 1;

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        CurrentVelocity = PlayerRigidbody2D.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Set Functions

    public void SetVelocityZero()
    {
        PlayerRigidbody2D.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        PlayerRigidbody2D.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        PlayerRigidbody2D.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        PlayerRigidbody2D.velocity = workspace;
        CurrentVelocity = workspace;
    }

    #endregion

    #region Check Functions

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    public bool CheckIfTouchingWall()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingWallBack()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if(xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    #endregion

    #region Other Functions

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
        float xDistance = xHit.distance;
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)(workspace), Vector2.down, ledgeCheck.position.y - wallCheck.position.y, playerData.whatIsGround);
        float yDistance = yHit.distance;

        workspace.Set(wallCheck.position.x + (xDistance * FacingDirection), ledgeCheck.position.y - yDistance);
        return workspace;
    }

    private void AnimationTrigger()
    {
        StateMachine.CurrentState.AnimationTrigger();
    }

    private void AnimationFinishTrigger()
    {
        StateMachine.CurrentState.AnimationFinishTrigger();
    }

    private void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    #endregion
}
