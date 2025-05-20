using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityHFSM;

public class PlayerController : MonoBehaviour, IHurtable
{
    [Header("Life settings")]
    public int nbLives = 3;

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

    [Header("Hurt Settings")]
    public float hurtTimer = 0f;
    public float hurtCooldown = 0.25f;
    public float hurtPushForce = 5f;

    [Header("Die Settings")]
    public float dieTimer = 0f;
    public float dieCooldown = 3f;

    [Header("Enemy Settings")]
    public LayerMask enemyLayer;

    public Transform Transform => transform;



    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private StateMachine fsm;
    private CircleCollider2D attackCollider;

    private Vector2 moveInput;
    private bool dashPressed;
    private bool attackPressed;
    private PlayerInputActions inputActions;

    private bool isDashing = false;
    private float dashTimeRemaining = 0f;
    private float dashCooldownRemaining = 0f;

    private bool isAttacking = false;

    private bool shouldHurt = false;
    private bool isHurting = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackCollider = GetComponent<CircleCollider2D>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        SetupFSM();
    }

    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        dashPressed = inputActions.Player.Dash.WasPressedThisFrame();
        attackPressed = inputActions.Player.Attack.WasPressedThisFrame();

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
        sr.flipX = (moveInput.x < 0);       
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

    private void OnEnterAttack()
    {
        isAttacking = true;
        animator?.Play("Attack");

        Vector2 position = attackCollider.transform.position;
        float radius = attackCollider.radius * attackCollider.transform.lossyScale.x;

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            var hurtable = hit.GetComponent<IHurtable>();
            if (hurtable != null)
            {
                hurtable.Hurt();
            }
        }
    }
    private void OnExitAttack()
    {
        isAttacking = false;
    }

    private bool TryAttack()
    {
        return (attackPressed && !isAttacking);
    }

    private bool AttackAnimationFinished()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1f);
    }


    private void OnEnterHurt()
    {
        shouldHurt = false;
        animator.Play("Hurt");
        hurtTimer = hurtCooldown;
        isHurting = true;

        // Disable inputs while hurt
        inputActions.Player.Disable();

        // Push back player randomly
        Vector2 hitDirection = Random.insideUnitCircle.normalized;
        rb.AddForce(hitDirection.normalized * hurtPushForce, ForceMode2D.Impulse);
    }

    private void OnLogicHurt()
    {
        hurtTimer -= Time.deltaTime;
        if (hurtTimer <= (hurtCooldown / 2f))
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (hurtTimer <= 0f)
        {
            // Reenable inputs
            inputActions.Player.Enable();
            isHurting = false;
        }
    }

    private void OnEnterDie()
    {
        animator.Play("Die");
        dieTimer = dieCooldown;
    }

    private void OnLogicDie()
    {
        rb.linearVelocity = Vector2.zero;
        dieTimer -= Time.deltaTime;
        if (dieTimer <= 0f)
        {
            inputActions.Player.Disable();
            Utils.RestartScene();
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

        fsm.AddState("Attack", new State(
            onEnter: state => OnEnterAttack(),
            onExit: state => OnExitAttack()
        ));

        fsm.AddState("Hurt", new State(
            onEnter: state => OnEnterHurt(),
            onLogic: state => OnLogicHurt()
        ));

        fsm.AddState("Die", new State(
            onEnter: state => OnEnterDie(),
            onLogic: state => OnLogicDie()
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

        fsm.AddTransition(new Transition(
            "Idle",
            "Attack",
            transition => TryAttack()
        ));

        fsm.AddTransition(new Transition(
            "Walk",
            "Attack",
            transition => TryAttack()
        ));

        fsm.AddTransition(new Transition(
            "Attack",
            "Idle",
            transition => AttackAnimationFinished()
        ));

        fsm.AddTransitionFromAny(new Transition(
            "",
            "Hurt",
            transition => shouldHurt && !isHurting
        ));

        fsm.AddTransition(new Transition(
            "Hurt",
            "Idle",
            transition => !isHurting
        ));

        fsm.AddTransitionFromAny(new Transition(
            "",
            "Die",
            transition => nbLives <= 0
        ));



        // Start the state machine from idle
        fsm.SetStartState("Idle");
        fsm.Init();
    }

    public void Hurt()
    {
        if (!shouldHurt)
        {
            nbLives--;

            if (nbLives > 0)
            {
                shouldHurt = true;
            }
        }
    }
}
