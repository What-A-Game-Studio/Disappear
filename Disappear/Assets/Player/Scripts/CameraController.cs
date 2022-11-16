using UnityEngine;
using WAG.Core.Controls;

namespace WAG.Player
{
    public class CameraController : MonoBehaviour
    {

        private float xRotation;
        public Transform CameraRig { get; set; }
        private Transform cam;
        [SerializeField] private float mouseSensitivity = 22f;
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
            if (PlayerController.MainPlayer.CanMoveOrRotate)
                CameraMovement();
        }

        private void CameraMovement()
        {
            if (!cam)
                return;

            float mouseX = InputManager.Instance.Look.x;
            float mouseY = InputManager.Instance.Look.y;


            cam.position = CameraRig.position;
            xRotation -= mouseY * mouseSensitivity * Time.smoothDeltaTime;
            xRotation = Mathf.Clamp(xRotation, upperLimit, bottomLimit);
            //Up & down vision 
            cam.localRotation = Quaternion.Euler(xRotation, 0, 0);


            rb.MoveRotation(rb.rotation * (Quaternion.Euler(0, mouseX * mouseSensitivity * Time.fixedDeltaTime, 0)));
        }

    }
}