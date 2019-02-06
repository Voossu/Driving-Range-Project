using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicles.Car;
using Vehicles.Utility;

namespace Vehicles.Utility
{
    public abstract class AbstractCarSpawner<S, C> : Singleton<S>
        where S : AbstractCarSpawner<S, C>
        where C : CarAbstractControl
    {
        #region Members

        [MustBeAssigned] [SerializeField] protected C carPrefab;
        [MustBeAssigned] public Transform population;

        protected C[] spawned = new C[0];

        [ReadOnly] [SerializeField] private int _livedCarCounts = 0;
        public int livedCarCounts => _livedCarCounts;

        #endregion

        #region Actions

        #endregion

        #region Methods

        public virtual void SpawnStart()
        {
            _livedCarCounts = spawned.Length;
            for (int i = 0; i < spawned.Length; i++)
            {
                spawned[i] = GenerateCar(i);
            }
        }

        protected virtual C GenerateCar(int i = -1)
        {
            GeometryOptions geometry = GeometryManager.instance.options;
            C spawnCar = Instantiate(carPrefab, geometry.respawn.position, geometry.respawn.rotation, population);
            spawnCar.name = carPrefab.name + (i != -1 ? i.ToString(" 000") : "");
            spawnCar.GetComponent<CheckpointAgent>().Circuit = geometry.checkpointCircuit;
            return spawnCar;
        }

        protected virtual void DestroyCars()
        {
            for (int i = 0; i < spawned.Length; i++)
            {
                Destroy(spawned[i].gameObject);
            }
        }

        public virtual void SpawnStop()
        {
            DestroyCars();
            spawned = new C[0];
            _livedCarCounts = 0;
        }

        public virtual void OnCarDie(C car)
        {
            if (car.gameObject.activeSelf && car.GetComponent<CarControlSystem>().driving && Array.IndexOf(spawned, car) > -1)
            {
                _livedCarCounts--;
            }
        }

        #endregion

    }
}