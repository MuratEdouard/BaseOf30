using Mono.Cecil;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using BehaviorGraph = Unity.Behavior.BehaviorGraph;
using BlackboardReference = Unity.Behavior.BlackboardReference;

public class EnemySwordsmanController : MonoBehaviour, IHurtable, IAttacker
{
    [Header("Settings")]
    public int health = 2;
    public float attackCooldown = 4f;


    private SpriteRenderer sr;
    private Animator animator;
    private PlayerController player;
    private NavMeshAgent navMeshAgent;

    private float agentSpeed;

    private BlackboardReference blackboard;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = FindFirstObjectByType<PlayerController>();
        navMeshAgent = GetComponent<NavMeshAgent>();

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
        var nextStepToPlayer = transform.position + (directionToPlayer * 2.0f);
        blackboard.SetVariableValue("nextStepToPlayer", nextStepToPlayer);

        // Prevent the nav mesh agent from rotating and flip it based on where it is moving
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        sr.flipX = navMeshAgent.velocity.x < 0;
        navMeshAgent.speed = agentSpeed;
    }

    public void Hurt()
    {
        health--;

        if (health <= 0)
        {
            blackboard.SetVariableValue("isDead", true);
        }
        else
        {
            blackboard.SetVariableValue("isHurt", true);
        }
    }

    public void Attack(IHurtable target)
    {
        target.Hurt();
        print("Is on attack cooldown");
        Invoke(nameof(ResetIsReadyToAttack), attackCooldown);
    }

    public void ResetIsReadyToAttack()
    {
        print("Is ready to attack again");
        blackboard.SetVariableValue("isReadyToAttack", true);
    }
}
