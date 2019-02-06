using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicles.Utility
{

    [ExecuteInEditMode]
    public class CarRaycastSystem : MonoBehaviour
    {
        #region Members

        [SerializeField] private LayerMask sensorMask; // Defines the layer
        public float lengthOfSensors = 10;
        public float normilize = 12;

        public int numberOfSensor => _numberOfMainSensor + _numberOfBackSensor;
        [SerializeField] protected int _numberOfMainSensor = 5;
        public int numberOfMainSensor
        {
            get => _numberOfMainSensor;
            set
            {
                _numberOfMainSensor = value;
                InitRaycast();
            }
        }
        [SerializeField] protected int _numberOfBackSensor = 1;
        public int numberOfBackSensor
        {
            get => _numberOfBackSensor;
            set
            {
                _numberOfBackSensor = value;
                InitRaycast();
            }
        }

        private float[] _sensorAngels = new float[0];    // List of sensor angles
        public float[] sensorAngels => _sensorAngels;
        [SerializeField] private float[] _sensorValues = new float[0];
        public float[] sensorValues => _sensorValues;


        [Header("Display settings")]
        [SerializeField] Color raycastBaseColor = Color.green;
        [SerializeField] Color raycastHitsColor = Color.red;

        #endregion


        #region Actions

        private void Reset()
        {
            sensorMask = LayerMask.GetMask(new string[] { "Default" });
        }

        private void Awake()
        {
            InitRaycast();
        }

        private void InitRaycast()
        {
            _sensorAngels = new float[numberOfSensor];
            _sensorValues = new float[numberOfSensor];

            for (int i = 0; i < _numberOfMainSensor; i++)
            {
                _sensorAngels[i] = 180.0f / (_numberOfMainSensor - 1) * i - 90;
            }

            for (int i = 0; i < _numberOfBackSensor; i++)
            {
                _sensorAngels[_numberOfMainSensor + i] = 180.0f / (_numberOfBackSensor + 1) * (i + 1) - 90 + (1 - 2 * (_numberOfBackSensor / 2 - i) % 2) * 180;
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < numberOfSensor; i++)
            {
                var direction = Quaternion.Euler(0, _sensorAngels[i], 0) * transform.forward;
                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, lengthOfSensors, sensorMask, QueryTriggerInteraction.Ignore))
                {
                    _sensorValues[i] = hit.distance / lengthOfSensors * normilize;
                }
                else
                {
                    _sensorValues[i] = normilize;
                }
            }
        }

        private void OnDrawGizmos()
        {
            var color = Gizmos.color;
            for (int i = 0; i < numberOfSensor; i++)
            {
                Gizmos.color = Color.Lerp(raycastHitsColor, raycastBaseColor, _sensorValues[i] / normilize - 0.1f);
                var direction = Quaternion.Euler(0, _sensorAngels[i], 0) * transform.forward;
                Gizmos.DrawLine(transform.position, transform.position + direction * _sensorValues[i] / normilize * lengthOfSensors);
            }
            Gizmos.color = color;
        }

        #endregion
    }

}