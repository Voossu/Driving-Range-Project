using System;
using UnityEngine;
using Vehicles.Utility;

namespace Vehicles.Car
{
    public class CarInputsControl : CarAbstractControl
    {

        #region Actions

        private void FixedUpdate()
        {
            if (controlSystem.driving)
            {
                // pass the input to the car!
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                float handbrake = Input.GetAxis("Jump");
                controlSystem.Move(h, v, v, handbrake);
            }
        }

        public override void KillCar()
        {
            InputsCarSpawner.instance.OnCarDie(this);
            base.KillCar();
        }

        #endregion
    }
}
