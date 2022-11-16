using UnityEngine;
using WAG.Player.Teams;

namespace WAG.Exits
{
    public class ExitStayController : ExitController
    {
        [SerializeField] private ExitStayUIController LoadingUi;
        [SerializeField] private float timeToExit;

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out HiderController hc))
            {
                if (hc.IsMine())
                    LoadingUi.StartTimer(timeToExit, () => ExitHider(hc));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out HiderController hc))
            {
                if (hc.IsMine())
                    LoadingUi.CancelTimer();
            }
        }
    }
}