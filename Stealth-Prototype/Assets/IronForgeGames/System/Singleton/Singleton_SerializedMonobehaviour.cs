using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class Singleton_SerializedMonobehaviour<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
{
    private static T instance;

    private static object _lock = new object();

    public static T Instance {
        get {
            lock (_lock)
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("(Singleton) " + typeof(T) + " Has been destroyed with onapplication quit and will not be recreated");
                }

                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("More than one objects were found in the scene.");

                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                }

                return instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);

        Enable();
    }

    private void OnDisable()
    {
        Disable();
    }

    private void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    // this method is meant to be overridden
    protected virtual void Enable()
    {
    }

    // this method is meant to be overridden
    protected virtual void Disable()
    {
    }
}
