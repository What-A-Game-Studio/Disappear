
using UnityEngine;
using WAG.Player;

public class LockController : MonoBehaviour
{
 
    private void OnEnable()
    {
        PlayerController.MainPlayer.CanRotate = false;
        PlayerController.MainPlayer.CanMove = false;
    }

    private void OnDisable()
    {
        if (PlayerController.MainPlayer)
        {
           
            PlayerController.MainPlayer.CanRotate = true;
            PlayerController.MainPlayer.CanMove = true;
        }
    }
}