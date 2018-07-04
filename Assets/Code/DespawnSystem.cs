using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Code
{
    public class DespawnSystem : ComponentSystem {
    
        struct DespawnableComponent
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Despawnable> Despawnables;
        }
    
        [Inject] DespawnableComponent DespawnableComponents;

        public const float ForceDespawnEpsilon = 1e-3f;

        protected override void OnUpdate()
        {
            float time = Time.time;

            var toDestroy = new List<GameObject>(); // despawnable objects that must be destroyed
            
            for (int i = 0; i < DespawnableComponents.Length; i++)
            {
                Despawnable despawnable = DespawnableComponents.Despawnables[i];

                if (despawnable != null && despawnable.AllowDespawn)
                {
                    bool destroy = false;
                    bool startedOrEnqueued = despawnable.StartDespawn || despawnable.EnqueueDespawn;
                    if (despawnable.TimeBeforeDespawnStart >= 0 || startedOrEnqueued)
                    { 
                        float timeAlive = time - despawnable.TimeAtSpawn;

                        // animate despawn
                        if (startedOrEnqueued || timeAlive >= despawnable.TimeBeforeDespawnStart)
                        {
                            // initialize despawn animation
                            if (!despawnable.StartDespawn)
                            {
                                despawnable.StartDespawn = true;
                                despawnable.TimeAnimatedFloat.AnimationTimeStart = time;
                            }
                            
                            TimeAnimatedFloat timeAnimatedFloat = despawnable.TimeAnimatedFloat;
                            if (timeAnimatedFloat.Enabled)
                            {
                                if (timeAnimatedFloat.AnimationTime < ForceDespawnEpsilon)
                                {
                                    destroy = true;
                                }
                                else
                                {
                                    // FADE effect
                                    var meshRenderers = despawnable.MeshRenderers;

                                    float lerp = Mathf.Clamp((time - timeAnimatedFloat.AnimationTimeStartDelayed) / 
                                                             timeAnimatedFloat.AnimationTime,
                                        0f, 1f);
                                    float alpha = Mathf.Lerp(timeAnimatedFloat.Start, timeAnimatedFloat.End, 
                                        timeAnimatedFloat.Curve.Evaluate(lerp));

                                    for (int j = 0; j < meshRenderers.Count; j++)
                                    {
                                        var meshRenderer = meshRenderers[j];

                                        Color color = meshRenderer.material.color;
                                        color.a = alpha;
                                        meshRenderer.material.color = color;
                                    }

                                    if (alpha < ForceDespawnEpsilon)
                                    {
                                        destroy = true;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (despawnable.ForceDespawn || destroy)
                    {
                        toDestroy.Add(DespawnableComponents.Transforms[i].gameObject);
                    }
                }
            }

            // destroy loop
            foreach (var go in toDestroy)
            {
                GameObject.Destroy(go);
            }
        }
    }
}

