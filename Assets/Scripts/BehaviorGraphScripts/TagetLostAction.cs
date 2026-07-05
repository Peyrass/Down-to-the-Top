using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TagetLostAction", story: "Check if [target] is lost by [self]", category: "MyActions", id: "a735ba0fbf8baee25651e6c28b3f3493")]
public partial class TargetLostAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [FormerlySerializedAs("DetectionRadius")] [SerializeReference] public BlackboardVariable<float> exitDetectionRadius;
    [FormerlySerializedAs("DetectionAngle")] [SerializeReference] public BlackboardVariable<float> exitDetectionAngle;
    [SerializeReference] public BlackboardVariable<string> TargetLayerName;
    [SerializeReference] public BlackboardVariable<string> EnemyLayerName;

    private LayerMask obstacleLayerMask;

    protected override Status OnStart()
    {
        // Cualquier objeto que no tenga layer Player o Enemy bloqueará la visión
        obstacleLayerMask = ~LayerMask.GetMask(TargetLayerName.Value, EnemyLayerName.Value);
        
        // Early return que evita errores en caso de que no haya un objetivo definido
        if (Target.Value == null) return Status.Success;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null){ Debug.Log("Objetivo no asignado para" + Self.Name); return Status.Success;}

        Vector3 directionToTarget = Target.Value.transform.position - Self.Value.transform.position;
        float distanceBetween = directionToTarget.magnitude;

        // 1. ¿Target salió del radio?
        if (distanceBetween > exitDetectionRadius.Value) return Status.Success; //éxito -> empieza a investigar

        // 2. ¿Se salió del ángulo de visión?
        if (Vector3.Angle(directionToTarget, Self.Value.transform.forward) > exitDetectionAngle.Value / 2f)
            return Status.Success;

        // 3. ¿Hay un obstáculo en medio? (Raycast)
        if (Physics.Raycast(Self.Value.transform.position, directionToTarget, distanceBetween, obstacleLayerMask))
            return Status.Success;

        // Si ninguna de las anteriores se cumple, el objetivo SIGUE a la vista
        return Status.Running;
    }
}
