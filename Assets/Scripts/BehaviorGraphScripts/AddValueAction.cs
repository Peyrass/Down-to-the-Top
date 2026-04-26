using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AddValueAction", story: "Increase [Repeat] value in 1", category: "MyActions", id: "3fd87f466eb956c9d5417a4b516fb17e")]
public partial class AddValueAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Repeat;

    protected override Status OnStart()
    {
        Repeat.Value++;

        return Status.Success;
    }
}

