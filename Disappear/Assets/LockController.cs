
using UnityEngine;
using WAG.Player;

public class LockController : MonoBehaviour
{
 
    private void OnEnable()
    {
        NGOPlayerController.MainPlayer.CanRotate = false;
        NGOPlayerController.MainPlayer.CanMove = false;
    }

    private void OnDisable()
    {
        if (NGOPlayerController.MainPlayer)
        {
           
            NGOPlayerController.MainPlayer.CanRotate = true;
            NGOPlayerController.MainPlayer.CanMove = true;
        }
    }
}