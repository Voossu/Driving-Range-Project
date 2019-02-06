using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicles.Car;
using Vehicles.Utility;

namespace Vehicles.Utility
{
    public class FollowCarSpawner : AbstractCarSpawner<FollowCarSpawner, CarFollowControl>
    {

        #region Members 

        public int number = 0;

        #endregion

        #region Methods

        public override void SpawnStart()
        {
            spawned = new CarFollowControl[number];
            base.SpawnStart();
        }

        protected override CarFollowControl GenerateCar(int i = -1)
        {
            var car = base.GenerateCar(i);
            car.GetComponent<WaypointNavigator>().Circuit = GeometryManager.instance.options.waypointCircuit;
            return car;
        }        

        #endregion

    }
}