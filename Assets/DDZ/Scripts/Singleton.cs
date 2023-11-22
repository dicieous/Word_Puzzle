using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T Instance;
        public static T instance => Instance;
   
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) 
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }

    public abstract class SingletonInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T Instance;
        public static T instance => Instance;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
        
        }

    }

