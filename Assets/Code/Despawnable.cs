using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

using Code.Tools;

namespace Code
{
    [RequireComponent(typeof(GameObjectEntity))]
    public class Despawnable : MonoBehaviour
    {
        public bool AllowDespawn = false; // toggles ability of object to be despawned
        
        // time between the object is spawned and its despawn animation is played
        // undefined if -1 (object will not despawn until manuall triggered)
        public float TimeBeforeDespawnStart = 0;
        
        public TimeAnimatedFloat TimeAnimatedFloat;

        public bool IsPooledObject = false;
        
        [HideInInspector] public bool EnqueueDespawn = false; // true to start despawn animation on
        [HideInInspector] public bool StartDespawn = false; // true if despawn process has started
        [HideInInspector] public bool ForceDespawn = false; // true if despawn is to be immediately applied

        [HideInInspector] public float TimeAtSpawn; // time from which despawn timeout will be computed
        
        [HideInInspector] public List<MeshRenderer> Renderers; // animate fade on these during despawn

        public void Start()
        {
            TimeAtSpawn = Time.time;
            Renderers = Utilities.GetComponentsInHierarchy<MeshRenderer>(this.transform);
        }
    }
}
