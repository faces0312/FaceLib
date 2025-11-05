using System.Collections.Generic;
using UnityEngine;

namespace FaceLib
{
    public sealed class ObjectManager : BaseObjectManager<ObjectManager>, IObjectManager
    {
        private readonly Dictionary<string, GameObject> keyToPrefab = new Dictionary<string, GameObject>();
        private readonly Dictionary<GameObject, string> prefabToKey = new Dictionary<GameObject, string>();

        protected override void OnInitialize()
        {
        }

        protected override void OnShutdown()
        {
            keyToPrefab.Clear();
            prefabToKey.Clear();
        }

        // Registration
        public void RegisterPrefab(string key, GameObject prefab)
        {
            if (string.IsNullOrEmpty(key) || prefab == null) return;
            keyToPrefab[key] = prefab;
            prefabToKey[prefab] = key;
            PoolManager.Instance.Initialize();
            PoolManager.Instance.Register(key, prefab);
        }

        public void UnregisterPrefab(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (keyToPrefab.TryGetValue(key, out var prefab))
            {
                keyToPrefab.Remove(key);
                if (prefabToKey.ContainsKey(prefab)) prefabToKey.Remove(prefab);
            }
        }

        // Convenience prewarm
        public void Prewarm(string key, int count, Transform parent = null)
        {
            if (count <= 0) return;
            PoolManager.Instance.Initialize();
            PoolManager.Instance.Prewarm(key, count, parent);
        }

        public void Prewarm(GameObject prefab, int count, Transform parent = null)
        {
            if (prefab == null || count <= 0) return;
            var key = GetOrRegisterKeyForPrefab(prefab);
            Prewarm(key, count, parent);
        }

        // Spawn helpers
        public GameObject Spawn(string key, Transform parent = null)
        {
            if (!keyToPrefab.ContainsKey(key)) return null;
            PoolManager.Instance.Initialize();
            PoolManager.Instance.Register(key, keyToPrefab[key]);
            return InternalSpawnByKey(key, Vector3.zero, Quaternion.identity, parent, worldSpace: false);
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null, bool worldSpace = false)
        {
            PoolManager.Instance.Initialize();
            // If not registered in Pool yet but we have a local mapping, register lazily
            if (!keyToPrefab.ContainsKey(key)) return null;
            PoolManager.Instance.Register(key, keyToPrefab[key]);
            return InternalSpawnByKey(key, position, rotation, parent, worldSpace);
        }

        public GameObject Spawn(string key, Vector3 position, Vector3 euler, Transform parent = null, bool worldSpace = false)
        {
            return Spawn(key, position, Quaternion.Euler(euler), parent, worldSpace);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null) return null;
            var key = GetOrRegisterKeyForPrefab(prefab);
            return Spawn(key, position, rotation, parent);
        }

        public GameObject Spawn(GameObject prefab, Transform parent = null)
        {
            if (prefab == null) return null;
            var key = GetOrRegisterKeyForPrefab(prefab);
            return Spawn(key, parent);
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null) return;
            PoolManager.Instance.Initialize();
            PoolManager.Instance.Despawn(instance);
        }

        public string GetOrRegisterKeyForPrefab(GameObject prefab)
        {
            if (prefab == null) return null;
            if (prefabToKey.TryGetValue(prefab, out var existing)) return existing;
            var key = prefab.name;
            RegisterPrefab(key, prefab);
            return key;
        }

        // Cleanup helpers
        public void DespawnObjectsByPrefix(Transform parent, string prefix)
        {
            DespawnChildrenByPrefix(parent, prefix);
        }

        protected override GameObject ResolvePrefab(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            if (keyToPrefab.TryGetValue(key, out var prefab) && prefab != null) return prefab;
            // Fallback to Resources if not registered
            return Resources.Load<GameObject>(key);
        }

        // --- Auto registration from Resources ---
        public int RegisterAllInResourcesPath(string resourcesSubPath = "Prefabs", bool prewarm = false, int prewarmCount = 0, Transform parent = null)
        {
            var loaded = Resources.LoadAll<GameObject>(resourcesSubPath);
            if (loaded == null || loaded.Length == 0) return 0;
            int registered = 0;
            for (int i = 0; i < loaded.Length; i++)
            {
                var prefab = loaded[i];
                if (prefab == null) continue;
                var key = prefab.name;
                RegisterPrefab(key, prefab);
                registered++;
                if (prewarm && prewarmCount > 0)
                {
                    Prewarm(key, prewarmCount, parent);
                }
            }
            return registered;
        }

#if UNITY_EDITOR
        public int EditorRegisterAllPrefabs(string resourcesSubPath = "Prefabs", bool includeSubdirectories = true, bool useRelativePathAsKey = false, bool prewarm = false, int prewarmCount = 0, Transform parent = null)
        {
            // Editor-only exhaustive scan using AssetDatabase to compute resource-relative paths
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources/" + resourcesSubPath });
            int registered = 0;
            for (int i = 0; i < guids.Length; i++)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(path)) continue;
                if (!includeSubdirectories)
                {
                    // Skip if prefab is not directly under the specified folder
                    var folder = "Assets/Resources/" + resourcesSubPath + "/";
                    if (!path.StartsWith(folder)) continue;
                    var rest = path.Substring(folder.Length);
                    if (rest.Contains("/")) continue;
                }
                // Convert to Resources.Load path
                var resPath = path;
                if (resPath.StartsWith("Assets/Resources/")) resPath = resPath.Substring("Assets/Resources/".Length);
                if (resPath.EndsWith(".prefab")) resPath = resPath.Substring(0, resPath.Length - ".prefab".Length);
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                var key = useRelativePathAsKey ? resPath : prefab.name;
                RegisterPrefab(key, prefab);
                if (prewarm && prewarmCount > 0)
                {
                    Prewarm(key, prewarmCount, parent);
                }
                registered++;
            }
            return registered;
        }
#endif
    }
}


