using UnityEngine;
using WAG.Core.Controls;
using WAG.Interactions;

public class PlayerInteraction : MonoBehaviour
{
    #region Class Members

    [SerializeField] private Transform cam;

    [Header("Interaction Parameters")] [Tooltip("Layer of objects that can be interacted with")] [SerializeField]
    private LayerMask layer;

    [Tooltip("Maximum distance to detect interactable objects")] [SerializeField]
    private int interactMaxDistance;

    [Tooltip("The size of the cube for detecting interactable objects")] [SerializeField]
    private Vector3 interactCubeSize;

    // General parameters
    private Interactable interactableObject;
    private RaycastHit hit;
    private GameObject player;

    [Header("Seeker Parameters")] [SerializeField]
    private int catchMaxDistance;

    [SerializeField] private LayerMask catchLayer;
    private Ray camRay;
    public bool isSeeker;
    bool m_HitDetect;

    // Start is called before the first frame update

    #endregion

    #region Unity Functions

    public void Init(GameObject pgo, bool seeker)
    {
        player = pgo;
        isSeeker = seeker;
        cam = transform.parent;
        InputManager.Instance.AddCallbackAction(
            ActionsControls.Interact,
            (context) =>
            {
                if (interactableObject != null)
                {
                    interactableObject.onInteract?.Invoke(player);
                }
            });
    }


    private void FixedUpdate()
    {
        m_HitDetect = Physics.BoxCast(transform.position, interactCubeSize, cam.forward, out hit,
            cam.rotation, interactMaxDistance, layer);
        if (m_HitDetect)
        {
            if (!hit.collider.TryGetComponent(out interactableObject))
            {
                interactableObject = null;
            }
        }
        else
        {
            interactableObject = null;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (m_HitDetect)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, cam.forward * hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + cam.forward * hit.distance, interactCubeSize);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, cam.forward * interactMaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + cam.forward * interactMaxDistance, interactCubeSize);
        }

        if (isSeeker)
        {
            Gizmos.DrawRay(cam.position, cam.forward * catchMaxDistance);
        }
    }

    #endregion
}