using UnityEngine;
using UnityEngine.AI;

public class SpikesController : MonoBehaviour
{
    public LayerMask playerLayer;
    public float waitTimeBeforeAttack = 1.0f;

    private Animator animator;
    private CircleCollider2D attackCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        attackCollider = GetComponent<CircleCollider2D>();

        Invoke(nameof(Attack), waitTimeBeforeAttack);
    }

    void Attack()
    {
        animator.Play("Attack");

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
    
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
