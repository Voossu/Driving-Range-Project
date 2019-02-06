using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vehicles.Utility;

public class GeometryOptions : MonoBehaviour
{

    #region Members

    [MustBeAssigned] [SerializeField] private WaypointCircuit _waypointCircuit;
    public WaypointCircuit waypointCircuit => _waypointCircuit;

    [MustBeAssigned] [SerializeField] private CheckpointCircuit _checkpointCircuit;
    public CheckpointCircuit checkpointCircuit => _checkpointCircuit;

    [MustBeAssigned] [SerializeField] private Transform _respawn;
    public Transform respawn => _respawn;

    [MustBeAssigned] [SerializeField] private Transform _camHome;
    public Transform camHome => _camHome;

    #endregion

    #region Actions

    private void Reset()
    {
        _waypointCircuit = transform.GetComponentInChildren<WaypointCircuit>();
        _checkpointCircuit = transform.GetComponentInChildren<CheckpointCircuit>();
        _respawn = transform.Find("Respawn");
        _camHome = transform.Find("CamHome");
    }

    #endregion

}
