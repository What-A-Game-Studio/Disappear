using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WAG.Core.Controls;

namespace WAG.Health
{
    public class HealthTest : MonoBehaviour
    {
        private HealthStatusController hsc;
        // Start is called before the first frame update
        void Start()
        {
            hsc = GetComponent<HealthStatusController>();
            MeshRenderer mr = GetComponent<MeshRenderer>();
            InputManager.Instance.AddCallbackAction(
                ActionsControls.Catch,
                started: context => { },
                performed: context => { hsc.TakeDamage();},
                canceled: context => { });
            InputManager.Instance.AddCallbackAction(
                ActionsControls.Interact,
                performed:context => hsc.HealUp()
                );
            
            
            StatusColor(hsc.CurrentHeathStatus, mr);
            hsc.OnHealthChanged += status => StatusColor(status, mr);
            
        }

        private void StatusColor(HeathStatus status, MeshRenderer mr)
        {
            switch (status)
            {
                case HeathStatus.Healthy:
                    mr.material.color = Color.green;
                    break;
                case HeathStatus.Wounded:
                    mr.material.color = Color.yellow;
                    break;
                case HeathStatus.Dying:
                    mr.material.color = Color.red;
                    break;             
                case HeathStatus.Dead:
                default:
                    mr.material.color = Color.black;
                    break;
            }
        }
    }
}