using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region StateVariables

    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }

    [SerializeField] private PlayerData playerData;

    #endregion

    #region Components

    public Animator Animator { get; private set; }

    public PlayerInputHandler InputHandler { get; private set; }

    public Rigidbody2D PlayerRigidbody2D { get; private set; }

    #endregion

    #region Other Variables

    public Vector2 CurrentVelocity { get; private set; }

    public int FacingDirection { get; private set; }      

    private Vector2 workspace;

    private string idleAnimName = "idle";
    private string moveAnimName = "move";

    #endregion

    #region Unity Callback Functions

    private void Awake()
    {        
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, idleAnimName);
        MoveState = new PlayerMoveState(this, StateMachine, playerData, moveAnimName);
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

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        PlayerRigidbody2D.velocity = workspace;
        CurrentVelocity = workspace;
    }

    #endregion

    #region CheckFunctions

    public void CheckIfShouldFlip(int xInput)
    {
        if(xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    #endregion

    #region Other Functions

    private void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    #endregion
}
