using UnityEngine;
using WAG.Core;
using WAG.Core.GM;
using WAG.Player.Teams;

namespace WAG.Exits
{
    [RequireComponent(typeof(Collider))]
    public abstract class ExitController : MonoBehaviour
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out HiderController hc))
            {
                ExitHider(hc);
            }
        }

        protected void ExitHider(HiderController hc)
        {
            // if (hc.IsMine())
            //     GameManager.Instance.HiderQuit(QuitEnum.Escape);
        }
    }
}