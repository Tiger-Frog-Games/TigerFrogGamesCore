using UnityEngine;

namespace TigerFrogGames.Core
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject(typeof(T).Name + "Auto-Generated");
                        instance = gameObject.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if(!Application.isPlaying) return;

            instance = this as T;
        }
    }
}