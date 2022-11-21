
using System;
using UnityEngine;
using WAG.HitHurtBoxes;

namespace WAG.Player
{
    public class PlayerHurtResponder : CompHurtResponder
    {
        private PlayerController playerController;
        public PlayerController PlayerController => playerController;
        private void Awake()
        {
            if (transform.parent.TryGetComponent<PlayerController>(out playerController))
            {
                Debug.Log("This object needs to be the PlayerController’s child", this);
                // Debug.Break();
            }
        }
    }
}
