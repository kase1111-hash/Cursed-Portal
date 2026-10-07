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
        // DontDestroyOnLoad only works on root objects, so a manager on a child persists its whole root;
        // keep persistent managers on a root that holds nothing scene-specific (see SceneSetup)
        GameObject persistentRoot = transform.root.gameObject;

        if (Instance == null)
        {
            Instance = this as T;
            if (transform.parent != null)
            {
                Debug.LogWarning($"[{typeof(T).Name}] is not on a root object; its whole hierarchy '{persistentRoot.name}' will persist across scenes.");
            }
            DontDestroyOnLoad(persistentRoot);
        }
        else
        {
            // Remove the same unit that would have persisted (the reloaded copy of that root)
            Destroy(persistentRoot);
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
