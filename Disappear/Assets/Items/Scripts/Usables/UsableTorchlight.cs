using UnityEngine;
using WAG.Interactions;

namespace WAG.Items
{
    public class UsableTorchlight : Interactable
    {
        private Light torch;

        protected override void Awake()
        {
            base.Awake();
            if (!transform.GetChild(0).TryGetComponent(out torch))
                Debug.LogError("Can't find light on torchlight");


            torch.enabled = false;
        }


        /// <summary>
        /// Turn on/off the torch light
        /// </summary>
        protected override void ActionOnInteract(GameObject sender)
        {
            torch.enabled = !torch.enabled;
        }
    }
}