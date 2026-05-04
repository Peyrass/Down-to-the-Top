using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchingAction", story: "[Self] is searching for [Target]", category: "MyActions", id: "50d5df83b40284ce4283b21bc4342edf")]
public partial class SearchingAction : Action
{
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> DetectionRadius;
        [SerializeReference] public BlackboardVariable<float> DetectionAngle;
        [SerializeReference] public BlackboardVariable<string> TargetLayerName;
        [SerializeReference] public BlackboardVariable<string> EnemyLayerName;

        private LayerMask targetLayerMask;
        private LayerMask obstacleLayerMask;

        private Collider[] results = new Collider[1];
        protected override Status OnStart()
        {
            targetLayerMask = LayerMask.GetMask(TargetLayerName.Value); //Capa objetivo.
            obstacleLayerMask = ~LayerMask.GetMask(TargetLayerName, EnemyLayerName.Value); //Cualquier capa que no sea la objetivo o la enemigo
            return Status.Running; //La tarea continua
        }
        
        protected override Status OnUpdate()
        {
            if (Physics.OverlapSphereNonAlloc(Self.Value.transform.position, DetectionRadius.Value, results,
                    targetLayerMask) <= 0) return Status.Running; 
            //Detecta datos dentro del Sphere, si un dato con la layer "target" es identíficado se obtiene toda su información.
            //Se extrae la dirección del objetivo
        
        
            Vector3 distanceToTarget = results[0].transform.position - Self.Value.transform.position;
        
            //se comprueba que el Target está en el rango de detección
            if (!(Vector3.Angle(distanceToTarget, Self.Value.transform.forward) < DetectionAngle.Value / 2))  
                return Status.Running; //early return.
        
        
            if (Physics.Raycast(Self.Value.transform.position, distanceToTarget, distanceToTarget.magnitude,
                    obstacleLayerMask)) return Status.Running;
        
            Target.Value = results[0].gameObject; //Objetivo setteado
            return Status.Success;
        }
    }