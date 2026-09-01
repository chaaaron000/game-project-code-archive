using System;
using UnityEngine;

namespace Nostal.Util
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_instance != null)
                {
                    return s_instance;
                }
                
                s_instance = FindObjectOfType<T>();

                if (s_instance != null)
                {
                    return s_instance;
                }
                
                Debug.LogError($"{typeof(T).Name} 싱글톤이 씬에 없습니다.");
                return null;
            }
        }

        protected virtual void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            s_instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnApplicationQuit()
        {
            s_instance = null;
        }
    }
}