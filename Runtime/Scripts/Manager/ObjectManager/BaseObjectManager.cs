using UnityEngine;

namespace FaceLib
{
	/// <summary>
	/// CRTP singleton base for object managers with common spawn/cleanup helpers.
	/// </summary>
	public abstract class BaseObjectManager<TSelf> : IBaseManager where TSelf : BaseObjectManager<TSelf>, new()
	{
		private static TSelf _instance;

		public static TSelf Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TSelf();
				}
				return _instance;
			}
		}

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

		// --- API surface ---
		protected virtual GameObject ResolvePrefab(string key)
		{
			if (string.IsNullOrEmpty(key)) return null;
			return Resources.Load<GameObject>(key);
		}

		public GameObject Spawn(string key, bool pooling = true) => Spawn(key, Vector3.zero, Quaternion.identity, null, pooling);

		public GameObject Spawn(string key, Vector3 pos, Vector3 dir, string subName = "", bool pooling = false)
		{
			var rot = dir == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(dir);
			return Spawn(key, pos, rot, null, pooling);
		}

		public GameObject Spawn(string key, Vector3 pos, Quaternion rot, string subName = "", bool pooling = false)
		{
			return Spawn(key, pos, rot, null, pooling);
		}

		public GameObject Spawn(string key, Transform parentTran, string subName = "", bool pooling = false)
		{
			return Spawn(key, Vector3.zero, Quaternion.identity, parentTran, pooling);
		}

		public GameObject Spawn(string key, Vector3 pos, Quaternion rot, Transform parentTran, bool pooling)
		{
			if (string.IsNullOrEmpty(key)) return null;
			if (pooling)
			{
				FaceManager.EnsurePool();
				return InternalSpawnByKey(key, pos, rot, parentTran, worldSpace: parentTran == null);
			}

			var prefab = ResolvePrefab(key);
			if (prefab == null) return null;
			var go = Object.Instantiate(prefab);
			go.name = prefab.name;
			if (parentTran != null) go.transform.SetParent(parentTran, false);
			go.transform.SetPositionAndRotation(pos, rot);
			go.transform.localScale = Vector3.one;
			return go;
		}

		public void Despawn(GameObject go)
		{
			if (go == null) return;
			InternalDespawn(go);
		}

		public void Despawn<TComponent>(TComponent component) where TComponent : Component
		{
			if (component == null) return;
			InternalDespawn(component.gameObject);
		}

		protected GameObject InternalSpawnByKey(string key, Vector3 position, Quaternion rotation, Transform parent, bool worldSpace)
		{
			FaceManager.EnsurePool();
			var go = FaceManager.Pool.Spawn(key, position, rotation, parent);
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
			FaceManager.EnsurePool();
			FaceManager.Pool.Despawn(instance);
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


