using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckLineOfSightAction", story: "[Self] can see [Target]", category: "MyActions", id: "1e0c84b163ca47ad35d1c8583a0a22c6")]
public partial class CheckLineOfSightAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<string> TargetLayerName;
    [SerializeReference] public BlackboardVariable<string> EnemyLayerName;
  
      private LayerMask obstacleLayerMask;
  
      protected override Status OnStart()
      {
          //se define la capa de obstaculos
          obstacleLayerMask = ~LayerMask.GetMask(TargetLayerName.Value, EnemyLayerName.Value);
          return Status.Running;
      }
  
      protected override Status OnUpdate()
      {
          if (Target.Value == null) {Debug.Log("Objetivo no asignado para" + Self.Name); return Status.Failure;}
              //early return de emergencia

  
          // Calcula la distancia y dirección exacta hacia el Target declarado por el Chaser.
          Vector3 distanceToTarget = Target.Value.transform.position - Self.Value.transform.position;
  
          // Se comprueba si hay linea de visión
          if (Physics.Raycast(Self.Value.transform.position, distanceToTarget.normalized, distanceToTarget.magnitude, obstacleLayerMask))
          {
              // El rayo chocó con un obstáculo antes de llegar al Target
              return Status.Failure; 
          }
  
          // Si el rayo no choca con nada de la máscara de obstáculos, hay visual del objetivo.
          return Status.Success;
      }
}

