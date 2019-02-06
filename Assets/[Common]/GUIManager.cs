using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicles.Utility;

using UnityEngine.UI;
using TMPro;
using System;
using Vehicles.Car;

public class GUIManager : Singleton<GUIManager>
{

    #region Headers

    #region GUI Panels

    [Header("GUI Panels")]
    [SerializeField] private RectTransform spawningTypePanel;
    [SerializeField] private RectTransform mainStandartPanel;
    [SerializeField] private RectTransform mainLearningPanel;
    [SerializeField] private RectTransform gameStandartPanel;
    [SerializeField] private RectTransform gameLearningPanel;
    [SerializeField] private RectTransform selectionCarPanel;

    #endregion


    #region Standart mode - input

    [Header("Standart mode - input")]
    [SerializeField] private Toggle iInputsCars;
    [SerializeField] private TMP_InputField iFollowCars;
    [SerializeField] private Toggle iNeuralCars;

    #endregion

    #region Standart mode - helps

    [Header("Standart mode - helps")]
    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hInputsCars;
    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hFollowCars;
    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hNeuralCars;

    #endregion

    #region Learning mode - input

    [Header("Learning mode - input")]
    [MustBeAssigned] [SerializeField] private TMP_InputField iMainSensors;
    [MustBeAssigned] [SerializeField] private TMP_InputField iBackSensors;
    [MustBeAssigned] [SerializeField] private TMP_InputField iHLayers;
    [MustBeAssigned] [SerializeField] private Toggle iBiasNetwork;

    [MustBeAssigned] [SerializeField] private TMP_InputField iElitism;
    [MustBeAssigned] [SerializeField] private TMP_InputField iMutations;
    [MustBeAssigned] [SerializeField] private TMP_InputField iCrossover;
    [MustBeAssigned] [SerializeField] private TMP_InputField iRandomize;

    [MustBeAssigned] [SerializeField] private TMP_InputField iMutationNeuronProb;
    [MustBeAssigned] [SerializeField] private TMP_InputField iMutationWeightProb;
    [MustBeAssigned] [SerializeField] private TMP_InputField iCrossoverUniformRate;

    #endregion

    #region Learning mode - helps

    [Header("Learning mode - helps")]
    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hNetwork;
    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hPopulation;
    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hGeneration;
    [MustBeAssigned] [SerializeField] private UIChart hLearnChart;


    #endregion

    #endregion

    #region Car Information

    [Header("Car Information")]
    [MustBeAssigned] [SerializeField] private UICarManager hCarManager;

    #endregion

    #region Actions

    private void Update()
    {
        UpdateUI();
    }

    #endregion

    #region Methods

    public void SpawnStandart()
    {
        hInputsCars.text = "off";
        hFollowCars.text = "off";
        hNeuralCars.text = "off";

        int follow;
        if (!Int32.TryParse(iFollowCars.text, out follow) || follow < 0)
        {
            follow = 0;
        }

        if (iInputsCars.isOn)
        {
            hInputsCars.text = "on";
            InputsCarSpawner.instance.SpawnStart();
        }

        hFollowCars.text = follow.ToString();
        FollowCarSpawner.instance.number = follow;
        FollowCarSpawner.instance.SpawnStart();

        if (iNeuralCars.isOn)
        {
            hNeuralCars.text = "on";
            NeuralCarSpawner.instance.respawnType = NeuralCarSpawner.RespawnMode.Standart;
            NeuralCarSpawner.instance.SpawnStart();
        }

    }

    public void SpawnLearning()
    {
        hLearnChart.Clear();

        #region sensors

        int mainSensors;
        if (!Int32.TryParse(iMainSensors.text, out mainSensors) || mainSensors < 0)
        {
            mainSensors = 5;
        }

        int backSensors;
        if (!Int32.TryParse(iBackSensors.text, out backSensors) || backSensors < 0)
        {
            backSensors = 1;
        }

        NeuralCarSpawner.instance.mainSensors = mainSensors;
        NeuralCarSpawner.instance.backSensors = backSensors;

        #endregion

        #region network

        bool bias = iBiasNetwork.isOn;

        hNetwork.text = "{" + mainSensors + "+" + backSensors;
        String[] sLayers = iHLayers.text.Replace(" ", "").Split(',');
        int[] layers = new int[sLayers.Length + 2];
        layers[0] = mainSensors + backSensors;
        for (int i = 0; i < sLayers.Length; i++)
        {
            int layerValue;
            if (!Int32.TryParse(sLayers[i], out layerValue) || layerValue < 0)
            {
                layerValue = 6;
            }
            layers[i + 1] = layerValue;
            hNetwork.text += "," + layerValue;
        }
        hNetwork.text += ",2}" + (bias ? "+b" : "");
        layers[layers.Length - 1] = 2;

        NeuralCarSpawner.instance.bias = bias;
        NeuralCarSpawner.instance.layers = layers;

        #endregion

        #region population

        int elitism;
        if (!int.TryParse(iElitism.text, out elitism) || elitism < 0)
        {
            elitism = 15;
        }
        int mutations;
        if (!int.TryParse(iMutations.text, out mutations) || mutations < 0)
        {
            mutations = 10;
        }
        int crossover;
        if (!int.TryParse(iCrossover.text, out crossover) || crossover < 0)
        {
            crossover = 10;
        }
        int randomize;
        if (!int.TryParse(iRandomize.text, out randomize) || randomize < 0)
        {
            randomize = 15;
        }

        int population = elitism + mutations + crossover + randomize;

        NeuralCarSpawner.instance.elitism = elitism;
        NeuralCarSpawner.instance.mutation = mutations;
        NeuralCarSpawner.instance.crossover = crossover;
        NeuralCarSpawner.instance.random = randomize;

        hPopulation.text = "P:" + population.ToString("00") + "=" + "e:" + elitism.ToString("00") + "+m:" + mutations.ToString("00") + "+c:" + crossover.ToString("00") + "+n:" + randomize.ToString("00");

        #endregion

        #region options

        float mutationNeuronProb;
        if (!float.TryParse(iMutationNeuronProb.text, out mutationNeuronProb) || mutationNeuronProb < 0 || mutationNeuronProb > 1)
        {
            mutationNeuronProb = 1.0f;
        }

        float mutationWeightProb;
        if (!float.TryParse(iMutationWeightProb.text, out mutationWeightProb) || mutationWeightProb < 0 || mutationWeightProb > 1)
        {
            mutationWeightProb = 0.08f;
        }

        float uniformRate;
        if (!float.TryParse(iCrossoverUniformRate.text, out uniformRate) || uniformRate < 0 || uniformRate > 1)
        {
            uniformRate = 0.02f;
        }

        NeuralCarSpawner.instance.mutationNeuronProb = mutationNeuronProb;
        NeuralCarSpawner.instance.mutationWeightProb = mutationWeightProb;
        NeuralCarSpawner.instance.uniformRate = uniformRate;

        #endregion

        NeuralCarSpawner.instance.respawnType = NeuralCarSpawner.RespawnMode.Training;
        NeuralCarSpawner.instance.SpawnStart();
    }

    public void SpawnStop()
    {
        InputsCarSpawner.instance.SpawnStop();
        FollowCarSpawner.instance.SpawnStop();
        NeuralCarSpawner.instance.SpawnStop();
        hCarManager.SelectCar(null);
    }

    public void UpdateUI()
    {
        if (NeuralCarSpawner.instance.generation > -1)
        {
            hGeneration.text = "Gen:" + NeuralCarSpawner.instance.generation + " Live:" + NeuralCarSpawner.instance.livedCarCounts;
        }
    }

    public void AddPointsToChart(float[] points)
    {
        hLearnChart.AddPoints(points);
    }

    public void SelectCar(CarControlSystem car)
    {
        hCarManager.selectCar = car;
    }

    #endregion

}
