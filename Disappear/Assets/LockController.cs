
using UnityEngine;
public class LockController : MonoBehaviour
{
 
    private void OnEnable()
    {
        PlayerController.MainPlayer.CanMoveOrRotate = false;
    }

    private void OnDisable()
    {
        if (PlayerController.MainPlayer)
        {
           
            PlayerController.MainPlayer.CanMoveOrRotate = true;
        }
    }
}