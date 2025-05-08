using UnityEngine;
using UnityHFSM;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private StateMachine fsm;

    private Vector2 moveInput;
    private PlayerInputActions inputActions;
    private Vector3 originalScale;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();


        SetupFSM();
    }

    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        fsm.OnLogic();
    }

    private void OnEnterIdle()
    {
        rb.linearVelocity = Vector2.zero;
        animator?.Play("Idle");
    }

    private void onEnterWalk()
    {
        animator?.Play("Walk");
    }
    private void onLogicWalk()
    {
        rb.linearVelocity = moveInput * moveSpeed;
        FlipAnimation();
    }

    private void FlipAnimation()
    {
        if (moveInput.x >= 0f)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // Right
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // Left
    }

    private void SetupFSM()
    {
        // Create new State Machine
        fsm = new StateMachine();


        // Add states
        fsm.AddState("Idle", new State(
            onEnter: state => OnEnterIdle()
        ));

        fsm.AddState("Walk", new State(
            onEnter: state => onEnterWalk(),
            onLogic: state => onLogicWalk()
        ));


        // Add transitions
        fsm.AddTransition(new Transition(
            "Idle",
            "Walk",
            transition => moveInput != Vector2.zero
        ));

        fsm.AddTransition(new Transition(
            "Walk",
            "Idle",
            transition => moveInput == Vector2.zero
        ));


        // Start the state machine from idle
        fsm.SetStartState("Idle");
        fsm.Init();
    }
}
