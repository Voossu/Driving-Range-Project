using UnityEngine;
using System.Collections;

public class FlyingCamera : MonoBehaviour
{

    public enum StartType
    {
        Base, // Enable on last position
        Home  // Enable on home position
    }

    [Header("Start position")]
    public StartType startType = StartType.Base;

    [Header("Walls settings")]
    public bool enableWalls = true;
    [ShowIf("enableWalls")] public Vector3 m_MinWalls = new Vector3(-150, 10, -150);
    [ShowIf("enableWalls")] public Vector3 m_MaxWalls = new Vector3(150, 150, 150);

    [Header("Speed settings")]
    public float mouseSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    #region Actions

    private void OnEnable()
    {
        rotationX = transform.rotation.eulerAngles.y;
        rotationY = -transform.rotation.eulerAngles.x;

        if (startType == StartType.Home)
        {
            ToHome();
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        Rotate();
        Moving();

        if (Input.GetKeyDown(KeyCode.End))
        {
            Cursor.lockState = Cursor.lockState != CursorLockMode.Confined ? CursorLockMode.Confined : CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #endregion

    #region Methods

    private void ToHome()
    {
        var homeTransform = GeometryManager.instance.options?.camHome;
        if (homeTransform)
        {
            transform.position = homeTransform.position;
            rotationX = homeTransform.rotation.eulerAngles.y;
            rotationY = -homeTransform.rotation.eulerAngles.x;
        }
    }

    private void Rotate()
    {
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    }

    private void Moving()
    {
        Vector3 position = transform.position;
        float moveFactor = GetSpeedMoveFactor();

        position += transform.forward * (normalMoveSpeed * moveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
        position += transform.right * (normalMoveSpeed * moveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q)) { position += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { position -= transform.up * climbSpeed * Time.deltaTime; }

        if (enableWalls)
        {
            if (position.x < m_MinWalls.x) position.x = m_MinWalls.x;
            if (position.y < m_MinWalls.y) position.y = m_MinWalls.y;
            if (position.z < m_MinWalls.z) position.z = m_MinWalls.z;

            if (position.x > m_MaxWalls.x) position.x = m_MaxWalls.x;
            if (position.y > m_MaxWalls.y) position.y = m_MaxWalls.y;
            if (position.z > m_MaxWalls.z) position.z = m_MaxWalls.z;
        }

        transform.position = position;
    }

    private float GetSpeedMoveFactor()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) return fastMoveFactor;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) return slowMoveFactor;
        return 1;
    }

    private void SwitchCursorStatus()
    {
        Cursor.lockState = Cursor.lockState != CursorLockMode.Confined ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }

    #endregion

}