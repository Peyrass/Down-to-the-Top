using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetRandomPointAction", story: "Find random point near [Self] within [Radius] and save to [Result]", category: "MyActions", id: "9ee8f7c1fcb5dd13722e76a8b2c1d554")]
public partial class GetRandomPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<Vector3> Result;

    protected override Status OnUpdate()
    {
        // Generamos un punto aleatorio en una esfera
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * Radius.Value;
        randomDir += Self.Value.transform.position;

        // Buscamos el punto más cercano válido en el NavMesh
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, Radius.Value, 1))
        {
            Result.Value = hit.position;
            return Status.Success;
        }

        return Status.Running;
    }
}