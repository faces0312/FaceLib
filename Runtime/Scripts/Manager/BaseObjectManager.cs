using UnityEngine;

namespace FaceLib
{
    /// <summary>
    /// 오브젝트 매니저 공통 베이스. Mono 싱글톤 + 라이프사이클 + 스폰/정리 헬퍼 제공.
    /// </summary>
    public abstract class BaseObjectManager<TSelf> : SingletonWithMono<TSelf>, IBaseManager where TSelf : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (IsInitialized) return;
            OnInitialize();
            IsInitialized = true;
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;
            OnShutdown();
            IsInitialized = false;
        }

        protected abstract void OnInitialize();
        protected abstract void OnShutdown();

        // 프리팹 해석(비풀링 경로). 필요 시 파생 클래스에서 경로/리소스 정책 커스터마이즈
        protected virtual GameObject ResolvePrefab(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return Resources.Load<GameObject>(key);
        }

        // 풀 스폰 래퍼
        protected GameObject InternalSpawnByKey(string key, Vector3 position, Quaternion rotation, Transform parent, bool worldSpace)
        {
            PoolManager.Instance.Initialize();
            var go = PoolManager.Instance.Spawn(key, position, rotation, parent);
            if (go == null) return null;
            if (!worldSpace && parent != null)
            {
                go.transform.localPosition = position;
                go.transform.localRotation = rotation;
            }
            return go;
        }

        protected void InternalDespawn(GameObject instance)
        {
            PoolManager.Instance.Initialize();
            PoolManager.Instance.Despawn(instance);
        }

        public void DespawnChildrenByPrefix(Transform parent, string prefix)
        {
            if (parent == null || string.IsNullOrEmpty(prefix)) return;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                if (child != null && child.name.StartsWith(prefix))
                {
                    InternalDespawn(child.gameObject);
                }
            }
        }
    }
}


