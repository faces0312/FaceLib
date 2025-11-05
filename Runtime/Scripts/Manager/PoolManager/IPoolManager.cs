using UnityEngine;

namespace FaceLib
{
    public interface IPoolManager : IBaseManager
    {
        void Register(string key, GameObject prefab);
        void Unregister(string key);
        void Prewarm(string key, int count, Transform parent = null);
        GameObject Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null);
        void Despawn(GameObject instance);
    }
}


