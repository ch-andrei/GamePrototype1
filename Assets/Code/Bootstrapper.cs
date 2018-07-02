using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Code
{
    public sealed class Bootstrapper
    {
        public static PrefabManager PrefabManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeWithScene()
        {
            var PrefabManagerGo = GameObject.FindGameObjectWithTag("Prefab Manager");
            PrefabManager = PrefabManagerGo?.GetComponent<PrefabManager>();
            if (!PrefabManager)
                return;
        }
    }
}
