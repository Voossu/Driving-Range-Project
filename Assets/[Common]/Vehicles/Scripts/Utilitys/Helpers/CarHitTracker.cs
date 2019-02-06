using UnityEngine;
using UnityEngine.Events;

namespace Vehicles.Utility
{
    public class CarHitTracker : MonoBehaviour
    {

        #region Members

        public LayerMask layerMask;

        public UnityEvent onCarHit;

        #endregion

        #region Actions

        private void OnCollisionEnter(Collision other)
        {
            int layer = other.gameObject.layer;
            if (!other.collider.isTrigger && layerMask == (layerMask | (1 << layer)))
            {
                onCarHit.Invoke();
            }
        }

        #endregion

    }
}