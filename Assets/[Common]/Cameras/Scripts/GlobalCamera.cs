using System;
using UnityEngine;

public class GlobalCamera : MonoBehaviour
{
    #region Members

    public float moveSpeed = 1.0f;         // How fast the rig will move to keep up with the target's position.

    #endregion

    #region Actions

    private void Update()
    {
        FollowTarget(Time.deltaTime * moveSpeed);
    }

    #endregion

    #region Methods

    protected virtual void FollowTarget(float delta)
    {
        var target = GeometryManager.instance.options?.camHome;
        if (target) transform.position = Vector3.Lerp(transform.position, target.position, delta);
    }

    #endregion
}