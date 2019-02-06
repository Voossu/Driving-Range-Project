using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicles.Car;
using System.Linq;

namespace Vehicles.Utility
{
    public class NeuralCarSpawner : AbstractCarSpawner<NeuralCarSpawner, CarNeuralControl>
    {
        #region Members 

        public RespawnMode respawnType = RespawnMode.Training;


        [Header("Training mode")]
        // [ReadOnly] [SerializeField] private bool _spawning = false;
        // public bool spawning => _spawning;
        [ReadOnly] [SerializeField] private int _generation = -1;
        public int generation => _generation;

        [Header("Topology")]
        public float lenghtOfSensors = 10;
        public float normalizeSensors = 10;
        public int mainSensors = 5;
        public int backSensors = 1;
        public int sensors => mainSensors + backSensors;

        public int[] layers;
        public bool bias = true;


        [Header("Population")]
        public int elitism = 20;
        public int mutation = 20;
        public int crossover = 20;
        public int random = 0;
        public int number => elitism + mutation + crossover + random;


        [Separator]
        public float mutationNeuronProb = 1.00f;
        public float mutationWeightProb = 0.08f;
        public float uniformRate = 0.02f;


        private NNAgent[] agents;

        #endregion

        #region Actions

        private void FixedUpdate()
        {
            if (_generation > -1 && livedCarCounts == 0)
            {
                SpawnStart();
            }
        }

        #endregion

        #region Methods


        #endregion

        #region Inherts

        protected override CarNeuralControl GenerateCar(int i = -1)
        {
            var car = base.GenerateCar(i);
            switch (respawnType)
            {
                case RespawnMode.Standart:
                    var save = NeuralCarStore.saves[i];
                    car.SetSave(save);
                    break;
                case RespawnMode.Training:
                    car.SetSensors(lenghtOfSensors, mainSensors, backSensors, normalizeSensors);
                    car.agent = agents[i];
                    car.name = carPrefab.name + " " + _generation.ToString("000") + "." + i.ToString("000");
                    break;
            }
            return car;
        }

        public override void SpawnStart()
        {
            switch (respawnType)
            {
                case RespawnMode.Standart:
                    int num = NeuralCarStore.saves.Count;
                    spawned = new CarNeuralControl[num];
                    break;
                case RespawnMode.Training:
                    if (_generation < 0)
                    {
                        spawned = new CarNeuralControl[number];
                        GenerateAgents();
                    }
                    else
                    {
                        GenerateAgents();
                        DestroyCars();
                    }
                    break;
            }
            base.SpawnStart();
            GUIManager.instance.SelectCar(spawned.Length > 0 ? spawned[0].GetComponent<CarControlSystem>() : null);
        }

        private void AddFitnessToChart()
        {
            float[] cut = new float[] {
                spawned.Max(car => car.GetComponent<CarControlSystem>().fitness),
                spawned.Average(car => car.GetComponent<CarControlSystem>().fitness),
                spawned.Min(car => car.GetComponent<CarControlSystem>().fitness)
            };

            GUIManager.instance.AddPointsToChart(cut);

            Debug.Log("Generation " + _generation + " - Better: " + cut[0] + " Average: " + cut[1] + " Worst : " + cut[2]);
        }

        private void GenerateAgents()
        {
            _generation++;

            if (_generation == 0)
            {
                agents = new NNAgent[spawned.Length];
                for (int i = 0; i < agents.Length; i++)
                {
                    agents[i] = new NNAgent(layers, bias);
                }
            }
            else
            {
                AddFitnessToChart();

                NNAgent[] elitAgents = spawned.OrderByDescending(car => car.GetComponent<CarControlSystem>().fitness).Take(elitism).Select(car => car.agent).ToArray();

                int newIndex = 0;

                for (int i = 0; i < elitism; newIndex++, i++)
                {
                    agents[newIndex] = new NNAgent(elitAgents[i]);
                }

                for (int i = 0; i < mutation; newIndex++, i++)
                {
                    agents[newIndex] = new NNAgent(elitAgents[i % 5]);
                    agents[newIndex].Mutator(mutationNeuronProb, mutationWeightProb);
                }

                for (int i = 0; i < crossover; newIndex++, i++)
                {
                    agents[newIndex] = NNAgent.Crossover(elitAgents[0], elitAgents[(i + 1) % 2], uniformRate);
                }

                for (int i = 0; i < random; newIndex++, i++)
                {
                    agents[newIndex] = new NNAgent(layers, bias);
                }

            }

        }

        public override void SpawnStop()
        {
            _generation = -1;
            base.SpawnStop();
        }

        public void SelectBest()
        {
            GUIManager.instance.SelectCar(spawned.OrderByDescending(car => car.GetComponent<CarControlSystem>().fitness).FirstOrDefault().GetComponent<CarControlSystem>());
        }


        #endregion

        #region Structs

        public enum RespawnMode
        {
            Standart, // Standart Racings Mode
            Training  // Training NNAgent Mode
        }

        #endregion

    }
}
