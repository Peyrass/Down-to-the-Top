using UnityEngine;

namespace HUD_UI.PlayerRelated
{
    public class SC_FallBillboard : MonoBehaviour
    {
        private Transform mainCameraTransform;

        private void Start()
        {
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
        }
        
        // Late Update para asegurarme de que el Canvas rote DESPUÉS de que la cámara se haya movido (que va antes con u Update normal)
        private void LateUpdate()
        {
            if (mainCameraTransform == null) return;
            transform.forward = mainCameraTransform.forward;
        }
    }
}