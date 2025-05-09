using UnityEngine;
using UnityHFSM;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;


    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("AfterImage Settings")]
    public GameObject afterImagePrefab;
    public float afterImageInterval = 0.05f;
    private float afterImageTimer = 0f;


    private Rigidbody2D rb;
    private Animator animator;
    private StateMachine fsm;

    private Vector2 moveInput;
    private bool dashPressed;
    private PlayerInputActions inputActions;
    private Vector3 originalScale;

    private bool isDashing = false;
    private float dashTimeRemaining = 0f;
    private float dashCooldownRemaining = 0f;


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
        dashPressed = inputActions.Player.Dash.WasPressedThisFrame();

        if (dashCooldownRemaining > 0)
            dashCooldownRemaining -= Time.deltaTime;
        else if (dashCooldownRemaining < 0)
            dashCooldownRemaining = 0;

        fsm.OnLogic();
    }

    private void OnEnterIdle()
    {
        rb.linearVelocity = Vector2.zero;
        animator?.Play("Idle");
    }

    private void OnEnterWalk()
    {
        animator?.Play("Walk");
    }
    private void OnLogicWalk()
    {
        rb.linearVelocity = moveInput * moveSpeed;
        FlipAnimation();
    }

    private void OnEnterDash()
    {
        isDashing = true;
        dashTimeRemaining = dashDuration;
        dashCooldownRemaining = dashCooldown;

        rb.AddForce(moveInput.normalized * dashForce, ForceMode2D.Impulse);
    }
    private void OnLogicDash()
    {
        dashTimeRemaining -= Time.deltaTime;

        afterImageTimer -= Time.deltaTime;
        if (afterImageTimer <= 0f)
        {
            SpawnAfterImage();
            afterImageTimer = afterImageInterval;
        }
    }
    private void OnExitDash()
    {
        isDashing = false;
        dashTimeRemaining = 0f;
    }

    private void SpawnAfterImage()
    {
        GameObject obj = Instantiate(afterImagePrefab);
        obj.GetComponent<PlayerAfterImageController>().Init(transform.position, moveInput.x >= 0f);
    }

    private void FlipAnimation()
    {
        if (moveInput.x >= 0f)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // Right
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // Left
    }


    private bool TryDash()
    {
        if (dashPressed)
        {

            if (dashCooldownRemaining == 0 && !isDashing && moveInput != Vector2.zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        
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
            onEnter: state => OnEnterWalk(),
            onLogic: state => OnLogicWalk()
        ));

        fsm.AddState("Dash", new State(
            onEnter: state => OnEnterDash(),
            onLogic: state => OnLogicDash(),
            onExit: state => OnExitDash()
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

        fsm.AddTransition(new Transition(
            "Walk",
            "Dash",
            transition => TryDash()
        ));

        fsm.AddTransition(new Transition(
           "Dash",
           "Idle",
           transition => dashTimeRemaining < 0f
        ));



        // Start the state machine from idle
        fsm.SetStartState("Idle");
        fsm.Init();
    }
}
