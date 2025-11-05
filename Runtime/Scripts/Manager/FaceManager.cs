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

        public static ObjectManager Object { get; private set; }

        public static UIManager UI { get; private set; }

        public static PoolManager Pool { get; private set; }

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
            Object ??= new ObjectManager();
            UI ??= new UIManager();
            Pool ??= new PoolManager();

            Object.Initialize();
            UI.Initialize();
            Pool.Initialize();
            IsInitialized = true;
        }

        public static void Shutdown()
        {
            if (!IsInitialized) return;
            UI?.Shutdown();
            Object?.Shutdown();
            Pool?.Shutdown();

            UI = null;
            Object = null;
            Pool = null;
            IsInitialized = false;
        }

        // --- Individual control APIs ---
        public static void EnsureObject()
        {
            Object ??= new ObjectManager();
        }

        public static void EnsureUI()
        {
            UI ??= new UIManager();
        }

        public static void EnsurePool()
        {
            Pool ??= new PoolManager();
        }

        public static void InitializeObject()
        {
            EnsureExists();
            EnsureObject();
            Object.Initialize();
        }

        public static void InitializeUI()
        {
            EnsureExists();
            EnsureUI();
            UI.Initialize();
        }

        public static void InitializePool()
        {
            EnsureExists();
            EnsurePool();
            Pool.Initialize();
        }

        public static void ShutdownObject()
        {
            Object?.Shutdown();
            Object = null;
        }

        public static void ShutdownUI()
        {
            UI?.Shutdown();
            UI = null;
        }

        public static void ShutdownPool()
        {
            Pool?.Shutdown();
            Pool = null;
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


