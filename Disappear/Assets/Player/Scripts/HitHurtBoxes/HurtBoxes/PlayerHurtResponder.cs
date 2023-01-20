
using System;
using UnityEngine;
using WAG.HitHurtBoxes;

namespace WAG.Player
{
    public class PlayerHurtResponder : CompHurtResponder
    {
        private NGOPlayerController playerController;
        public NGOPlayerController PlayerController => playerController;
        private void Awake()
        {
            if (transform.parent.TryGetComponent<NGOPlayerController>(out playerController))
            {
                Debug.Log("This object needs to be the PlayerController’s child", this);
                // Debug.Break();
            }
        }
    }
}
