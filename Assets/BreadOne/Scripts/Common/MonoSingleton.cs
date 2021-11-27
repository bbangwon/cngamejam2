using UnityEngine;

namespace cngamejam
{
    public class MonoSingleton<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        protected static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
                return instance;
            }
        }

    }
}