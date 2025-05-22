using System;
using System.Collections;
using System.Linq;
using Mono.Cecil;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using BehaviorGraph = Unity.Behavior.BehaviorGraph;
using BlackboardReference = Unity.Behavior.BlackboardReference;
using Random = UnityEngine.Random;

public class SandmanController : MonoBehaviour, IHurtable, IAttacker, IRunner, IIdler
{
    [Header("General Settings")]
    public int life = 2;
    public float attackCooldown = 4f;
    public float hurtCooldown = 1f;
    public LayerMask playerLayer;

    [Header("Spikes Settings")]
    public GameObject spikesPrefab;
    public int nbSpikesPerAttack = 10;
    public float waitTimeBeforeSpikesAttack = 1.0f;

    [Header("Unstuck Settings")]
    Vector3 lastPosition;
    public float minMoveDistance = 0.1f;
    public float checkInterval = 1f;
    public float stuckThreshold = 3f;
    private float stuckTimer = 0f;

    public Transform Transform => transform;
    public bool IsAttacking { get; private set; }


    private SpriteRenderer sr;
    private Animator animator;
    private PlayerController player;
    private NavMeshAgent navMeshAgent;
    private CircleCollider2D attackCollider;

    private float attackTimer = 0;
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
        IsAttacking = false;
        attackTimer = attackCooldown;
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
        var nextStepAwayFromPlayer = transform.position + (directionToPlayer * -1.0f);
        blackboard.SetVariableValue("nextStepAwayFromPlayer", nextStepAwayFromPlayer);

        // Prevent the nav mesh agent from rotating and flip it based on where it is moving
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        sr.flipX = navMeshAgent.velocity.x < 0;
        navMeshAgent.speed = agentSpeed;

        bool isReadyToAttack;
        blackboard.GetVariableValue("isReadyToAttack", out isReadyToAttack);
        if (!isReadyToAttack)
        {
            if (attackTimer > 0)
                attackTimer -= Time.deltaTime;
            else
            {
                attackTimer = 0;
                blackboard.SetVariableValue("isReadyToAttack", true);
            }
        }

        if (IsStuck())
        {
            Unstuck();
        }
    }

    private bool IsStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved < minMoveDistance)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer >= stuckThreshold)
        {
            Unstuck();
            stuckTimer = 0f;
        }

        if (Time.time % checkInterval < Time.deltaTime)
            lastPosition = transform.position;
        return false;
    }

    private void Unstuck()
    {
        var randX = Random.Range(-9f, 9f);
        var randY = Random.Range(-5f, 5f);
        navMeshAgent.SetDestination(new Vector2(randX, randY));
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
        
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        IsAttacking = true;
        blackboard.SetVariableValue("isReadyToAttack", false);
        attackTimer = attackCooldown;

        // Play attack
        animator.Play("Attack");
        yield return new WaitForSeconds(0.5f);

        // Spawn spikes
        SpawnSpikes();
        IsAttacking = false;
    }

    private void SpawnSpikes()
    {
        Vector2[] positions = new Vector2[] { };

        int nbSpikesPositioned = 0;
        while (nbSpikesPositioned < nbSpikesPerAttack)
        {
            var randX = Random.Range(-9f, 9f);
            var randY = Random.Range(-5f, 5f);
            var randVector2 = new Vector2(randX, randY);

            if (positions.Contains(randVector2))
            {
                continue;
            }
            else
            {
                GameObject spikes = Instantiate(spikesPrefab, transform.parent.transform);
                spikes.transform.localPosition = randVector2;

                // Get the script and configure it
                SpikesController controller = spikes.GetComponent<SpikesController>();
                controller.playerLayer = playerLayer;
                controller.waitTimeBeforeAttack = waitTimeBeforeSpikesAttack;

                positions.Append(randVector2);
                nbSpikesPositioned++;
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

    public void Idle()
    {
        animator.Play("Idle");
    }
}
