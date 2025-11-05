using System.Collections.Generic;
using UnityEngine;

namespace FaceLib
{
    /// <summary>
    /// Use this in scenes to add and initialize managers.
    /// </summary>
    public abstract class BaseGameManager : MonoBehaviour
    {
        protected readonly List<IBaseManager> _preLoadManagerList = new List<IBaseManager>();
        protected readonly List<IBaseManager> _managerList = new List<IBaseManager>();

        protected virtual void Awake()
        {
            FaceManager.EnsureExists();

            AddPreLoadManagers();
            InitializeManagers(_preLoadManagerList);
            OnPreLoad();

            AddManagers();
            InitializeManagers(_managerList);
            OnInit();
        }

        protected virtual void OnDestroy()
        {
            ShutdownManagers(_managerList);
            ShutdownManagers(_preLoadManagerList);
        }

        protected virtual void AddPreLoadManagers() { }
        protected virtual void AddManagers() { }
        protected virtual void OnPreLoad() { }
        protected virtual void OnInit() { }

        private static void InitializeManagers(List<IBaseManager> list)
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                var m = list[i];
                if (m == null) continue;
                if (!m.IsInitialized) m.Initialize();
            }
        }

        private static void ShutdownManagers(List<IBaseManager> list)
        {
            if (list == null) return;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var m = list[i];
                if (m == null) continue;
                if (m.IsInitialized) m.Shutdown();
            }
            list.Clear();
        }
    }
}


