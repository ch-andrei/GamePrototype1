using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Code
{
    public sealed class Bootstrapper
    {
        public static PrefabManager PrefabManager;

        public static ObjectPooler ObjectPooler;

        public static string PrefabManagerObjectTag = "Prefab Manager";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeWithScene()
        {
            var go = GameObject.FindGameObjectWithTag(PrefabManagerObjectTag);
            
            PrefabManager = go?.GetComponent<PrefabManager>();
            if (!PrefabManager)
                Debug.Log("PrefabManager was not initialized");
            
            ObjectPooler = go?.GetComponent<ObjectPooler>();
            if (!ObjectPooler)
                Debug.Log("ObjectPooler was not initialized");
        }
    }
}
