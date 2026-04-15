using System;
using Unity.Cinemachine;
using UnityEngine;

namespace PlayerComponents
{
    public class SC_CameraComponent : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField] private CinemachineCamera explorationCam;
        [SerializeField] private CinemachineCamera tatakaeCam;

        [Header("Combat Detection")]
        [SerializeField] private float combatRadius = 5f;
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private Transform player;
        private bool inCombat;
        

        private void Update()
        {
            // TODO: reactivar en siguiente commit (combat camera system)
        }

        private void CheckCombat()
        {
            inCombat = Physics.CheckSphere(
                player.position,
                combatRadius,
                whatIsEnemy
            );
        }

        private void UpdateCameraState()
        {
            if (inCombat)
            {
                explorationCam.Priority = 0;
                tatakaeCam.Priority = 10;
            }
            else
            {
                explorationCam.Priority = 10;
                tatakaeCam.Priority = 0;
            }
        }
    }
}