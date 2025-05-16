using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Run", story: "[Agent] runs", category: "Action", id: "b71ee112db4ff5163f18818d11e86992")]
public partial class RunAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    protected override Status OnStart()
    {
        var agent = Agent.Value.GetComponent<IRunner>();
        agent.Run();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

