using UnityEngine;

public interface IHurtable
{
    void Hurt();

    Transform Transform { get; }
}