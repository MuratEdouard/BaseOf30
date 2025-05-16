using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Agent] attacks [Target]", category: "Action", id: "131bf1fc0a50efb4dfbdbb379de42453")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private IAttacker _attacker;
    private IHurtable _hurtable;

    protected override Status OnStart()
    {
        _attacker = Agent.Value.GetComponent<IAttacker>();
        _hurtable = Target.Value.GetComponent<IHurtable>();
        if (_attacker.IsAttacking)
            return Status.Failure;
        else
        {
            _attacker.Attack(_hurtable);
            return Status.Running;
        }
            
    }

    protected override Status OnUpdate()
    {
        return _attacker.IsAttacking ? Status.Running : Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

