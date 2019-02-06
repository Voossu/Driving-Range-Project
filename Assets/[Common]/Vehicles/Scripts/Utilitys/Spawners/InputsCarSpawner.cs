using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicles.Car;
using Vehicles.Utility;

namespace Vehicles.Utility
{
    public class InputsCarSpawner : AbstractCarSpawner<InputsCarSpawner, CarInputsControl>
    {

        #region Members

        #endregion

        #region Actions

        #endregion

        #region Methods

        public override void SpawnStart()
        {
            spawned = new CarInputsControl[1];
            base.SpawnStart();
        }

        #endregion

    }
}