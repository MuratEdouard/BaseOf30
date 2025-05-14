using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Agent] attacks [Target]", category: "Action", id: "5e012a7863981064f83473c976141118")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    protected override Status OnStart()
    {
        var attacker = Agent.Value.GetComponent<IAttacker>();
        var hurtable = Target.Value.GetComponent<IHurtable>();
        if (attacker != null && hurtable != null)
        {
            attacker.Attack(hurtable);
        }
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

