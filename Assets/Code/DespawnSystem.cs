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

        public const float ForceDespawnAlphaMin = 1e-3f;

        protected override void OnUpdate()
        {
            float time = Time.time;
            float deltaTime = Time.deltaTime;

            var toDestroy = new List<GameObject>();
            
            for (int i = 0; i < DespawnableComponents.Length; i++)
            {
                Despawnable despawnable = DespawnableComponents.Despawnables[i];
                Transform transform = DespawnableComponents.Transforms[i];
                
                if (despawnable != null)
                {
                    Animated animated = despawnable.Animated;
                    
                    float timeAlive = time - despawnable.TimeAtSpawn;
                    
                    // animate despawn
                    bool destroy = false;
                    if (despawnable.AllowDespawn && timeAlive >= despawnable.TimeToLive)
                    {
                        if (animated.Enabled)
                        {
                            // FADE effect
                            var meshRenderers = despawnable.MeshRenderers;
                            
                            float lerp = Mathf.Clamp((time - animated.AnimationTimeStart) / animated.AnimationTime, 0f, 1f);
                            float alpha = Mathf.Lerp(animated.Start, animated.End, animated.Curve.Evaluate(lerp));
                            
                            for (int j = 0; j < meshRenderers.Count; j++)
                            {
                                var meshRenderer = meshRenderers[j];
                        
                                Color color = meshRenderer.material.color;
                                color.a *= alpha;
                                meshRenderer.material.color = color;
                            }
                            
                            if (alpha < ForceDespawnAlphaMin)
                            {
                                despawnable.ForceDespawn = true;
                            }
                        }
                    
                        if (timeAlive >= despawnable.TimeToLive + animated.AnimationTime)
                        {
                            destroy = true;
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

