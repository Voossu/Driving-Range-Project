using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GeometryManager : Singleton<GeometryManager>
{

    #region Members

    [MustBeAssigned] public GeometryOptions[] trackPrefabs;

    [MustBeAssigned] public Transform geometry;
    [MustBeAssigned] [SerializeField] private GeometryOptions _activeTrack;
    public GeometryOptions options => _activeTrack;

    [ReadOnly] [SerializeField] private int activeIndex = 0;

    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI m_UITrackLabel;

    #endregion

    #region Actions

    private void Reset()
    {
        geometry = GameObject.FindWithTag("Geometry")?.transform;
        _activeTrack = GameObject.FindWithTag("Track")?.GetComponent<GeometryOptions>();
    }

    private void OnEnable()
    {
        if (m_UITrackLabel) m_UITrackLabel.text = _activeTrack.name;
    }

    #endregion

    #region Methods

    [Button]
    public void SwitchTrack()
    {
        if (trackPrefabs == null || trackPrefabs.Length == 0) return;
        activeIndex = activeIndex + 1 < trackPrefabs.Length ? activeIndex + 1 : 0;
        Destroy(_activeTrack.gameObject);
        _activeTrack = Instantiate(trackPrefabs[activeIndex], geometry, false)?.GetComponent<GeometryOptions>();
        _activeTrack.name = trackPrefabs[activeIndex].name;
        if (m_UITrackLabel) m_UITrackLabel.text = _activeTrack.name;
    }

    #endregion

}
