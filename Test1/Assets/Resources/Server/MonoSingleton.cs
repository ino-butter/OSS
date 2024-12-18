 using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>, new()
{
    private static T _instance = null;
    private static Object _lock = new Object();
    public static T instance
    {
        get
        {
            if (_instance == null && Time.timeScale != 0)
            {
                lock (_lock)
                {
                    CreateInstance();
                }
            }
            return _instance;
        }
    }
    private bool initialized;
    public static void CreateInstance()
    {
        _instance = FindObjectOfType<T>();
        if (_instance == null)
        {
            var go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
        }
        else
        {
            DontDestroyOnLoad(_instance);
        }

        if (!_instance.initialized)
        {
            _instance.Initialize();
        }
    }


    protected virtual void Initialize()
    {
        _instance.initialized = true;
    }

    private void OnApplicationQuit()
    {
        Time.timeScale = 0;
    }
}