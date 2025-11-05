using UnityEngine;

namespace FaceLib
{
    public interface IObjectManager : IBaseManager
    {
        void RegisterPrefab(string key, GameObject prefab);
        void UnregisterPrefab(string key);

        void Prewarm(string key, int count, Transform parent = null);
        void Prewarm(GameObject prefab, int count, Transform parent = null);

        GameObject Spawn(string key, Transform parent = null);
        GameObject Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null, bool worldSpace = false);
        GameObject Spawn(string key, Vector3 position, Vector3 euler, Transform parent = null, bool worldSpace = false);
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);
        GameObject Spawn(GameObject prefab, Transform parent = null);

        void Despawn(GameObject instance);

        string GetOrRegisterKeyForPrefab(GameObject prefab);

        void DespawnObjectsByPrefix(Transform parent, string prefix);

        int RegisterAllInResourcesPath(string resourcesSubPath = "Prefabs", bool prewarm = false, int prewarmCount = 0, Transform parent = null);

#if UNITY_EDITOR
        int EditorRegisterAllPrefabs(string resourcesSubPath = "Prefabs", bool includeSubdirectories = true, bool useRelativePathAsKey = false, bool prewarm = false, int prewarmCount = 0, Transform parent = null);
#endif
    }
}


