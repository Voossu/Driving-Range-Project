using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{

    #region Members

    /// <summary>
    /// The instance.
    /// </summary>
    private static T _іnstance;
    public static T instance
    {
        get
        {
            if (_іnstance == null)
            {
                _іnstance = FindObjectOfType<T>();
                if (_іnstance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _іnstance = obj.AddComponent<T>();
                }
            }
            return _іnstance;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected virtual void Awake()
    {
        if (_іnstance == null)
        {
            _іnstance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

}