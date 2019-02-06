using System;
using System.Collections;
using UnityEngine;

namespace Vehicles.Car
{
    public class CarSkidTrail : MonoBehaviour
    {

        #region Members

        [SerializeField] private float m_PersistTime = 0;

        #endregion

        #region Actions

        private IEnumerator Start()
        {
			while (true)
            {
                yield return null;

                if (transform.parent.parent == null)
                {
					Destroy(gameObject, m_PersistTime);
                }
            }
        }

        #endregion
    }
}
