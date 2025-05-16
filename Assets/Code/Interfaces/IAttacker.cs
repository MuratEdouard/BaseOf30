using UnityEngine;

public interface IAttacker
{
    void Attack(IHurtable target);

    bool IsAttacking { get; }
}