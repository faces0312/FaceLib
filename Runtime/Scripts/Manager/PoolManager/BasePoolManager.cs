using System.Collections.Generic;
using UnityEngine;

namespace FaceLib
{
    /// <summary>
    /// 오브젝트 풀링의 기본 구현을 제공하는 제네릭 베이스 매니저.
    /// MonoBehaviour 싱글톤 기반이며, 구체 타입 TSelf로 상속해 사용합니다.
    /// </summary>
    public abstract class BasePoolManager<TSelf> : SingletonWithMono<TSelf>, IPoolManager where TSelf : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }

        private readonly Dictionary<string, Queue<GameObject>> keyToPool = new Dictionary<string, Queue<GameObject>>();
        private readonly Dictionary<string, GameObject> keyToPrefab = new Dictionary<string, GameObject>();
        private readonly Dictionary<GameObject, string> instanceToKey = new Dictionary<GameObject, string>();

        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;
            // 풀에 남아있는 인스턴스 정리
            foreach (var kv in keyToPool)
            {
                var q = kv.Value;
                while (q.Count > 0)
                {
                    var go = q.Dequeue();
                    if (go != null)
                    {
                        Object.Destroy(go);
                    }
                }
            }
            keyToPool.Clear();
            keyToPrefab.Clear();
            instanceToKey.Clear();
            IsInitialized = false;
        }

        public void Register(string key, GameObject prefab)
        {
            if (string.IsNullOrEmpty(key) || prefab == null) return;
            if (!keyToPrefab.ContainsKey(key))
            {
                keyToPrefab[key] = prefab;
            }
            if (!keyToPool.ContainsKey(key))
            {
                keyToPool[key] = new Queue<GameObject>();
            }
        }

        public void Unregister(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            keyToPrefab.Remove(key);
            if (keyToPool.TryGetValue(key, out var q))
            {
                while (q.Count > 0)
                {
                    var go = q.Dequeue();
                    if (go != null)
                    {
                        Object.Destroy(go);
                    }
                }
                keyToPool.Remove(key);
            }
        }

        public void Prewarm(string key, int count, Transform parent = null)
        {
            if (count <= 0) return;
            if (!keyToPrefab.TryGetValue(key, out var prefab)) return;
            EnsurePool(key);
            for (int i = 0; i < count; i++)
            {
                var go = Object.Instantiate(prefab, parent);
                go.name = prefab.name;
                go.SetActive(false);
                keyToPool[key].Enqueue(go);
                instanceToKey[go] = key;
            }
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!keyToPrefab.TryGetValue(key, out var prefab)) return null;
            EnsurePool(key);
            GameObject go = null;
            while (keyToPool[key].Count > 0 && go == null)
            {
                go = keyToPool[key].Dequeue();
            }
            if (go == null)
            {
                go = Object.Instantiate(prefab);
                instanceToKey[go] = key;
            }
            var t = go.transform;
            t.SetParent(parent, false);
            t.SetPositionAndRotation(position, rotation);
            go.SetActive(true);
            return go;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null) return;
            if (!instanceToKey.TryGetValue(instance, out var key))
            {
                Object.Destroy(instance);
                return;
            }
            instance.SetActive(false);
            instance.transform.SetParent(null, false);
            EnsurePool(key);
            keyToPool[key].Enqueue(instance);
        }

        private void EnsurePool(string key)
        {
            if (!keyToPool.ContainsKey(key))
            {
                keyToPool[key] = new Queue<GameObject>();
            }
        }
    }
}


