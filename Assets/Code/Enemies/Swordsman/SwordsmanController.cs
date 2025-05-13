using Mono.Cecil;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using BehaviorGraph = Unity.Behavior.BehaviorGraph;
using BlackboardReference = Unity.Behavior.BlackboardReference;

public class EnemySwordsmanController : MonoBehaviour
{
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

    public void Idle()
    {
        animator.Play("Idle");
    }   

    public void Run()
    {
        animator.Play("Run");
    }

    public void Attack()
    {
        animator.Play("Attack");
    }

    public void Die()
    {
        blackboard.SetVariableValue("isDead", true);
    }
}
