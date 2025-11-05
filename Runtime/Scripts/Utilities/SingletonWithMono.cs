using UnityEngine;

namespace FaceLib
{
    /// <summary>
    /// 간단한 MonoBehaviour 싱글톤 유틸리티. 존재 여부를 보장하고, 중복 인스턴스를 방지합니다.
    /// </summary>
    public abstract class SingletonWithMono<TSelf> : MonoBehaviour where TSelf : MonoBehaviour
    {
        protected static TSelf _instance;

        public static TSelf Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType<TSelf>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(TSelf).Name);
                        Object.DontDestroyOnLoad(go);
                        _instance = go.AddComponent<TSelf>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as TSelf;
                Object.DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}


