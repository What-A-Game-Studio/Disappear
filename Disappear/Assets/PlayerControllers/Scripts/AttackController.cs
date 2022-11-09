using Photon.Pun;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] private AttackHitBoxController loadedAttack;
    [SerializeField] private AttackHitBoxController normalAttack;
    [Header("Values")]
    [SerializeField] private float attackMissCooldown = 2f;
    [SerializeField] private float attackMissSpeedModifier = -0.8f;
    [SerializeField] private float attackHitCooldown = 2.6f;
    [SerializeField] private float attackHitSpeedModifier = -0.9f;
    [SerializeField] private float chargeTime = 0.3f;
    [SerializeField] private float chargedAttackSpeedModifier = 0.5f;

    private float timeSinceStartedAttack = 0f;
    private bool attackStarted;

    private PhotonView pv;

    private PlayerController pc;

    //camRay = new Ray(cam.position, cam.forward);
    // if (isSeeker && InputManager.Instance.Catch && Physics.Raycast(camRay, out hit, catchMaxDistance, catchLayer))
    // {
    //     if (hit.collider.TryGetComponent(out Interactable interactableHider))
    //     {
    //         interactableHider.onInteract?.Invoke(player);
    //     }
    // }
    private void Awake()
    {
        ///TODO: Pas hyper clean je pense
        if (!transform.parent.parent.TryGetComponent<PhotonView>(out pv))
        {
            Debug.LogError("Need PhotonView", this);
            Debug.Break();
        }

        if (!transform.parent.parent.TryGetComponent<PlayerController>(out pc))
        {
            Debug.LogError("Need PlayerController", this);
            Debug.Break();
        }

        // InputManager.Instance.AddCallbackAction(
        //     ActionsControls.Catch,
        //     started: context => StartAttack(),
        //     performed: context => { pc.TemporarySpeedModifier = chargedAttackSpeedModifier; },
        //     canceled: context => Attacked());
    }

}