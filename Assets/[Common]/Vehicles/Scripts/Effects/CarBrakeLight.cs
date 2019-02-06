using System;
using UnityEngine;

namespace Vehicles.Car
{
    public class CarBrakeLight : MonoBehaviour
    {

        #region Members

        [SerializeField] private CarControlSystem car; // reference to the car controller, must be dragged in inspector
        private Renderer m_Renderer;

        #endregion

        #region Actions

        private void Start()
        {
            m_Renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            // enable the Renderer when the car is braking, disable it otherwise.
            m_Renderer.enabled = car.BrakeInput > 0f;
        }

        #endregion
    }
}
