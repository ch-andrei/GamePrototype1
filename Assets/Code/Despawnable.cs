using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Code.Tools;

namespace Code
{
    public class Despawnable : MonoBehaviour
    {
        public bool AllowDespawn = false;
        public bool ForceDespawn = false;
        
        public float TimeToLive = 0;

        [HideInInspector] public float TimeAtSpawn;
        [HideInInspector] public float TimeAtDespawnAnimationStart
        {
            get => Animated.AnimationTimeStart;
            set => Animated.AnimationTimeStart = value;
        }
        
        [HideInInspector] public List<MeshRenderer> MeshRenderers;

        public Animated Animated;

        public void Start()
        {
            TimeAtSpawn = Time.time;
            MeshRenderers = Utilities.GetComponentsInChildren<MeshRenderer>(this.transform);
        }
    }
}
