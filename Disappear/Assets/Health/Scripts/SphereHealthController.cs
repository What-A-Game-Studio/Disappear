using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAG.Core.Controls;

namespace WAG.Health
{
    public class SphereHealthController : HealthStatusController
    {
        private MeshRenderer mr;

        protected override void Awake()
        {
            mr = GetComponent<MeshRenderer>();
            base.Awake();
            InputManager.Instance.AddCallbackAction(
                ActionsControls.Catch,
                performed: context => TakeDamage()
            );
            InputManager.Instance.AddCallbackAction(
                ActionsControls.Interact,
                performed: context => HealUp()
            );
        }

        protected override void OnHealthy()
        {
            mr.material.color = Color.green;
        }

        protected override void OnWounded()
        {
            mr.material.color = Color.yellow;
        }

        protected override void OnDying()
        {
            mr.material.color = Color.red;
        }

        protected override void OnDead()
        {
            mr.material.color = Color.black;
        }
    }
}