using UnityEngine;

namespace FaceLib
{
    /// <summary>
    /// 리소스 매니저의 공통 베이스(CRTP 싱글톤). 경로 기반 로드와 경량 캐시를 제공합니다.
    /// 파생 클래스에서 검색 경로를 구성하고, 필요 시 헬퍼 메서드를 추가해 사용하세요.
    /// </summary>
    public abstract class BaseResourceManager<TSelf> : SingletonWithMono<TSelf>, IResourceManager where TSelf : MonoBehaviour
    {
        // Instance는 SingletonWithMono에서 제공합니다.

        public bool IsInitialized { get; private set; }

        // 검색 경로와 간단한 캐시
        private readonly System.Collections.Generic.List<string> searchPaths = new System.Collections.Generic.List<string>();
        private readonly System.Collections.Generic.Dictionary<string, Object> cache = new System.Collections.Generic.Dictionary<string, Object>();

        public void Initialize()
        {
            if (IsInitialized) return;
            ConfigureSearchPaths();
            OnInitialized();
            IsInitialized = true;
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;
            OnShuttingDown();
            cache.Clear();
            IsInitialized = false;
        }

        /// <summary>
        /// 검색 경로를 설정하세요. 예: AddSearchPaths("Prefabs", "Prefabs/UI");
        /// </summary>
        protected abstract void ConfigureSearchPaths();

        /// <summary>
        /// Optional hook called after NewGameResource.
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// Optional hook called before clearing references.
        /// </summary>
        protected virtual void OnShuttingDown() { }

        // 경로 유틸
        protected void AddSearchPaths(params string[] paths)
        {
            if (paths == null) return;
            for (int i = 0; i < paths.Length; i++)
            {
                var p = paths[i];
                if (string.IsNullOrEmpty(p)) continue;
                p = p.Replace("\\", "/");
                if (p.StartsWith("/")) p = p.Substring(1);
                if (p.EndsWith("/")) p = p.Substring(0, p.Length - 1);
                if (!string.IsNullOrEmpty(p)) searchPaths.Add(p);
            }
        }

        public T Load<T>(string name) where T : Object
        {
            if (string.IsNullOrEmpty(name)) return null;
            // 캐시 키는 타입+이름 조합
            var cacheKey = typeof(T).FullName + ":" + name;
            if (cache.TryGetValue(cacheKey, out var cached) && cached)
            {
                return cached as T;
            }

            // 절대 경로 시도
            if (name.Contains("/"))
            {
                var direct = Resources.Load<T>(name);
                if (direct != null)
                {
                    cache[cacheKey] = direct;
                    return direct;
                }
            }

            // 검색 경로 순회
            for (int i = 0; i < searchPaths.Count; i++)
            {
                var full = string.IsNullOrEmpty(searchPaths[i]) ? name : searchPaths[i] + "/" + name;
                var obj = Resources.Load<T>(full);
                if (obj != null)
                {
                    cache[cacheKey] = obj;
                    return obj;
                }
            }
            return null;
        }

        public GameObject LoadPrefab(string name) => Load<GameObject>(name);
    }
}


