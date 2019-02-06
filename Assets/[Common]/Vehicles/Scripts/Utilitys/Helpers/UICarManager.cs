using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vehicles.Car;
using Vehicles.Utility;

public class UICarManager : MonoBehaviour
{

    #region Members

    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hCarName;
    [MustBeAssigned] [SerializeField] private TextMeshProUGUI hFitness;
    [MustBeAssigned] [SerializeField] private NNView hNNViev;
    [MustBeAssigned] [SerializeField] private Button saveButton;
    [MustBeAssigned] [SerializeField] private Button dropButton;
    [MustBeAssigned] [SerializeField] private Button bestButton;

    [SerializeField] private CarControlSystem _selectCar = null;
    public CarControlSystem selectCar { get => _selectCar; set => SelectCar(value); }

    #endregion

    #region Actions

    private void OnEnable()
    {
        UpdateUI();
    }

    public void FixedUpdate()
    {
        if (_selectCar != null)
        {
            hFitness.text = _selectCar.fitness.ToString("0.00");
        }
    }

    #endregion

    #region Methods

    private void UpdateUI()
    {
        if (_selectCar == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        hCarName.text = _selectCar.name;
        hFitness.text = _selectCar.fitness.ToString("0.00");

        hNNViev.gameObject.SetActive(false);

        hNNViev.Agent = _selectCar.GetComponent<CarNeuralControl>()?.agent;
        saveButton.gameObject.SetActive(hNNViev.Agent != null);

        bestButton.gameObject.SetActive(NeuralCarSpawner.instance.generation > -1);
        dropButton.gameObject.SetActive(NeuralCarStore.saves.Count > 0);
    }

    public void SelectCar(CarControlSystem car)
    {
        _selectCar = car;
        UpdateUI();
    }

    public void BestNeuralCar()
    {
        NeuralCarSpawner.instance.SelectBest();
    }

    public void SaveNeuralCar()
    {
        saveButton.gameObject.SetActive(false);
        var select = selectCar.GetComponent<CarNeuralControl>();
        if (select != null) NeuralCarStore.saves.Add(select.GetSave());
        dropButton.gameObject.SetActive(true);
        Debug.Log(select);
    }

    public void DropNeuralCars()
    {
        dropButton.gameObject.SetActive(false);
        NeuralCarStore.saves.Clear();
    }

    #endregion


}
