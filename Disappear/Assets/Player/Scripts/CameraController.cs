using UnityEngine;
using WAG.Core.Controls;

namespace WAG.Player
{
    public class CameraController : MonoBehaviour
    {
        private float xRotation;
        [field: SerializeField] public Transform CameraRig { get; set; }
        private Transform cam;
         public float MouseSensitivity { get; private set; } = 22f;
        [SerializeField] private float upperLimit = -40f;
        [SerializeField] private float bottomLimit = 70f;

        private Rigidbody rb;

        private void Awake()
        {
            cam = Camera.main.transform;
            cam.parent = transform;
            cam.GetComponentInChildren<PlayerInteraction>()?.Init(gameObject, true);

            if (!TryGetComponent<Rigidbody>(out rb))
            {
                Debug.LogError("Need Rigidbody", this);
                Debug.Break();
            }
        }

        private void LateUpdate()
        {
            if (NGOPlayerController.MainPlayer.CanRotate)
                CameraMovement();
        }

        private void CameraMovement()
        {
            if (!cam)
                return;

            float mouseY = InputManager.Instance.Look.y;

            cam.position = CameraRig.position;
            xRotation -= mouseY * MouseSensitivity * Time.smoothDeltaTime;
            xRotation = Mathf.Clamp(xRotation, upperLimit, bottomLimit);
            //Up & down vision 
            cam.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
    }
}