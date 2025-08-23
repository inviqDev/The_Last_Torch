using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance) return _instance;
            
            _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (_instance) return _instance;
            
            var singletonObj = new GameObject(typeof(T).Name);
            _instance = singletonObj.AddComponent<T>();
            
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}