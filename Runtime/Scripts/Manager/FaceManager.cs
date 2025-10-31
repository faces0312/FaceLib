using UnityEngine;

namespace FaceLib
{
    /// <summary>
    /// Central manager for FaceLib. Provides lifecycle control and a singleton entry point.
    /// </summary>
    public sealed class FaceManager : MonoBehaviour
    {
        private static FaceManager _instance;

        public static FaceManager Instance => _instance;

        public static bool IsInitialized { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoBootstrap()
        {
            EnsureExists();
        }

        public static void EnsureExists()
        {
            if (_instance != null) return;

            var existing = FindObjectOfType<FaceManager>();
            if (existing != null)
            {
                _instance = existing;
            }
            else
            {
                var go = new GameObject("FaceManager");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<FaceManager>();
            }
        }

        public static void Initialize()
        {
            EnsureExists();
            if (IsInitialized) return;
            IsInitialized = true;
        }

        public static void Shutdown()
        {
            if (!IsInitialized) return;
            IsInitialized = false;
        }

#if UNITY_EDITOR
        // Test helper: resets static state to ensure isolated tests
        public static void EditorForceResetForTests()
        {
            IsInitialized = false;
            _instance = null;
            var found = Object.FindObjectsOfType<FaceManager>();
            foreach (var m in found)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(m.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(m.gameObject);
                }
            }
        }
#endif
    }
}


