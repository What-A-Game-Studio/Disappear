using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiderHealthSystem : Interactable
{
    private int HiderLife;
    private PlayerController pc;
    public void Init(int maxLife)
    {
        HiderLife = maxLife;
        pc = GetComponent<PlayerController>();
    }

    protected override void ActionOnInteract(GameObject sender)
    {
        HiderLife--;
        if (HiderLife > 0)
        {
            pc.Teleport();
        }
        else
        {
            pc.Defeat();
        }
    }

   
}
