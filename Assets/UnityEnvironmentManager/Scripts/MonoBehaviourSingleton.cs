
using UnityEngine;
using UnitySingleton.Common;

namespace UnitySingleton
{
    public class MonoBehaviourSingleton<T> : SingletonCommon where T : class
    {
        public static T _Instance { get; private set; }

        protected virtual void Awake()
        {
            _Instance = InitSingleton(_Instance);
        }
    }

    public class MonoBehaviourProtectedSingleton<T> : SingletonCommon where T : class
    {
        protected static T _Instance = null;

        protected virtual void Awake()
        {
            _Instance = InitSingleton(_Instance);
        }
    }

    public class MonoBehaviourPrivateSingleton<T> : SingletonCommon where T : class
    {
        private static T _Instance = null;

        protected virtual void Awake()
        {
            _Instance = InitSingleton(_Instance);
        }
    }
}

namespace UnitySingleton.Common
{
    public class SingletonCommon : MonoBehaviour
    {
        protected SingletonCommon() { }

        protected T InitSingleton<T>(T instance) where T : class
        {
            if (instance == null)
            {
                instance = GetComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != GetComponent<T>())
                Destroy(gameObject);
            return instance;
        }
    }
}