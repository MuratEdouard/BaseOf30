using System.Collections;
using Mono.Cecil;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using BehaviorGraph = Unity.Behavior.BehaviorGraph;
using BlackboardReference = Unity.Behavior.BlackboardReference;

public class SwordsmanController : MonoBehaviour, IHurtable, IAttacker, IRunner
{
    [Header("Settings")]
    public int life = 2;
    public float attackCooldown = 2f;
    public float hurtCooldown = 1f;
    public LayerMask playerLayer;

    public Transform Transform => transform;
    public bool IsAttacking { get; private set; }


    private SpriteRenderer sr;
    private Animator animator;
    private PlayerController player;
    private NavMeshAgent navMeshAgent;
    private CircleCollider2D attackCollider;

    private float agentSpeed;

    private BlackboardReference blackboard;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = FindFirstObjectByType<PlayerController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        attackCollider = GetComponent<CircleCollider2D>();

        agentSpeed = navMeshAgent.speed;
    }

    private void Start()
    {
        var agent = GetComponent<BehaviorGraphAgent>();
        blackboard = agent.Graph.BlackboardReference;
        blackboard.SetVariableValue("player", player.gameObject);
    }

    public void Update()
    {
        var distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        blackboard.SetVariableValue("distanceToPlayer", distanceToPlayer);

        var directionToPlayer = (player.transform.position - transform.position).normalized;
        var nextStepToPlayer = transform.position + (directionToPlayer * 1.0f);
        blackboard.SetVariableValue("nextStepToPlayer", nextStepToPlayer);

        // Prevent the nav mesh agent from rotating and flip it based on where it is moving
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        sr.flipX = navMeshAgent.velocity.x < 0;
        navMeshAgent.speed = agentSpeed;
    }

    public void Hurt()
    {
        bool isHurt;
        blackboard.GetVariableValue("isHurt", out isHurt);

        if (!isHurt)
        {
            life--;

            blackboard.SetVariableValue("isHurt", true);
            blackboard.SetVariableValue("isDead", life <= 0);

            if (life <= 0)
            {
                animator.Play("Die");
                StartCoroutine(WaitToDie());
            }
            else
            {
                animator.Play("Hurt");
                StartCoroutine(ClearIsHurtAfterCooldown());
            }
        }
    }

    private IEnumerator ClearIsHurtAfterCooldown()
    {
        yield return new WaitForSeconds(hurtCooldown);
        blackboard.SetVariableValue("isHurt", false);
    }

    public void Attack(IHurtable target)
    {
        
        StartCoroutine(AttackSequence(target));
    }

    private IEnumerator AttackSequence(IHurtable target)
    {
        IsAttacking = true;

        // Play pre attack
        animator.Play("PreAttack");
        yield return new WaitForSeconds(0.5f);

        // Record where to attack
        Vector3 attackPosition = target.Transform.position;
        yield return new WaitForSeconds(0.5f);

        // Teleport to target and perform attack
        transform.position = attackPosition;
        animator.Play("PostAttack");
        AttackPlayerIfInRange();

        // Wait before being ready to attack again
        yield return new WaitForSeconds(2f);
        IsAttacking = false;
    }

    private void AttackPlayerIfInRange()
    {
        Vector2 position = attackCollider.transform.position;
        float radius = attackCollider.radius * attackCollider.transform.lossyScale.x;

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            var hurtable = hit.GetComponent<IHurtable>();
            if (hurtable != null)
            {
                hurtable.Hurt();
            }
        }
    }

    private IEnumerator WaitToDie()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        Destroy(gameObject);
    }

    public void Run()
    {
        animator.Play("Run");
    }
}
