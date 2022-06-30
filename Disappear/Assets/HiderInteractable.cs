using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiderInteractable : Interactable
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void ActionOnInteract(GameObject sender)
    {
        Debug.Log("JE SOUIS TOUCHE");
    }
}
