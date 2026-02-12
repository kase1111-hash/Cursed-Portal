using UnityEngine;

/// <summary>
/// Generic singleton base for MonoBehaviours that persist across scene loads.
/// Subclasses can override Awake() but MUST call base.Awake().
/// </summary>
public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

/// <summary>
/// Generic singleton base for MonoBehaviours scoped to a single scene.
/// Destroyed on scene transition; no DontDestroyOnLoad.
/// </summary>
public abstract class SceneSingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
