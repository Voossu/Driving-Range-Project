using UnityEngine;
using TMPro;

public class CamerasManager : Singleton<CamerasManager>
{

    #region Members

    [MustBeAssigned] public GameObject[] CameraObjects = new GameObject[0];
    [ReadOnly] [SerializeField] private int activeIndex = 0;
    public GameObject activeCamera => activeIndex < CameraObjects.Length ? CameraObjects[activeIndex] : null;

    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI m_UICameraLabel;

    #endregion

    #region Actions

    private void Reset()
    {
        CameraObjects = GameObject.FindGameObjectsWithTag("MainCamera");
        ResetActives();
    }

    private void OnEnable()
    {
        ResetActives();
    }

    #endregion

    #region Methods

    [Button]
    public void SwitchCamera()
    {
        if (CameraObjects == null || CameraObjects.Length == 0) return;
        activeIndex = activeIndex + 1 < CameraObjects.Length ? activeIndex + 1 : 0;
        ResetActives();
    }

    private void ResetActives()
    {
        for (int i = 0; i < CameraObjects.Length; i++)
        {
            CameraObjects[i].SetActive(i == activeIndex);
        }
        if (m_UICameraLabel) m_UICameraLabel.text = CameraObjects[activeIndex].name;
    }

    #endregion

}