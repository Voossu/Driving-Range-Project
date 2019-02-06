using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vehicles.Utility;

namespace Vehicles.Car
{
    [RequireComponent(typeof(CarControlSystem))]
    [DisallowMultipleComponent]
    public abstract class CarAbstractControl : MonoBehaviour
    {

        #region Members

        protected CarControlSystem controlSystem;         // the car controller we want to use

        #endregion

        #region Actions

        protected virtual void Awake()
        {
            controlSystem = GetComponent<CarControlSystem>();
        }

        #endregion

        #region Methods

        public virtual void KillCar()
        {
            controlSystem.driving = false;
        }

        #endregion

    }
}