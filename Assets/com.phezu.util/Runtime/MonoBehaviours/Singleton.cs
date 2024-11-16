using UnityEngine;

namespace Phezu.Util
{

    /// <summary>
    /// Inherit from this base class to create a singleton. Note: This object should never be inactive
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new();
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (Application.isPlaying && m_ShuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = (T)FindObjectOfType(typeof(T));

                        if (m_Instance == null && Application.isPlaying)
                        {
                            Debug.Log("Could not find singleton instance, typeof: " + typeof(T) + " , returning null");
                        }
                    }

                    return m_Instance;
                }
            }
        }

        protected virtual void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }

        protected virtual void OnDestroy()
        {
            m_ShuttingDown = true;
        }
    }
}