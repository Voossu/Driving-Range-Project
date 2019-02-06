using System;
using System.Linq;
using UnityEngine;
using Vehicles.Utility;

namespace Vehicles.Car
{
    [RequireComponent(typeof(CarRaycastSystem))]
    public class CarNeuralControl : CarAbstractControl
    {

        #region Members

        private CarRaycastSystem raycastSystem; // the raycas seystems

        [SerializeField] private NNAgent _agent = null;
        public NNAgent agent
        {
            get => _agent;
            set
            {
                if (value == null) _agent = null;
                if (value.layers.First() == raycastSystem.numberOfSensor || value.layers.Last() == 2)
                {
                    _agent = value;
                    _agent.normalize = raycastSystem.normilize;
                }
            }
        }

        #endregion

        #region Actions

        protected override void Awake()
        {
            base.Awake();
            raycastSystem = GetComponent<CarRaycastSystem>();
        }


        private void FixedUpdate()
        {
            if (controlSystem.driving && agent != null)
            {
                float[] sensorValues = raycastSystem.sensorValues;
                float[] output = agent.FeedForward(sensorValues);

                float v = output[0];
                float h = output[1];

                controlSystem.Move(h, v, v, 0f);
            }
        }

        public override void KillCar()
        {
            NeuralCarSpawner.instance.OnCarDie(this);
            base.KillCar();
        }

        public void SetSensors(float length, int main, int back, float normalize)
        {
            raycastSystem.lengthOfSensors = length;
            raycastSystem.numberOfMainSensor = main;
            raycastSystem.numberOfBackSensor = back;
        }

        public NeuralCarSave GetSave()
        {
            NeuralCarSave save = new NeuralCarSave();
            save.lenghtOfSensors = raycastSystem.lengthOfSensors;
            save.mainSensors = raycastSystem.numberOfMainSensor;
            save.backSensors = raycastSystem.numberOfBackSensor;
            save.agent = agent;
            return save;
        }

        public void SetSave(NeuralCarSave save)
        {
            SetSensors(save.lenghtOfSensors, save.mainSensors, save.backSensors, save.normilize);
            agent = save.agent;
        }

        #endregion

        #region Structs

        [Serializable]
        public struct NeuralCarSave
        {
            public float lenghtOfSensors;
            public int mainSensors;
            public int backSensors;
            public float normilize;
            public NNAgent agent;
        }


        #endregion
    }
}
