using UnityEngine;

namespace FaceLib
{
    public interface IResourceManager : IBaseManager
    {
        T Load<T>(string name) where T : Object;
        GameObject LoadPrefab(string name);
    }
}


