using System.Collections;
using System.Collections.Generic;
using Code.Towers;
using UnityEngine;
using Unity.Entities;

namespace Code
{
    [UpdateAfter(typeof(SelectableSystem))]
    [UpdateAfter(typeof(TowerAimingSystem))]
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
                Transform transform = DespawnableComponents.Transforms[i];
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
                                    float lerp = Mathf.Clamp((time - timeAnimatedFloat.AnimationTimeStartDelayed) / 
                                                             timeAnimatedFloat.AnimationTime, 0f, 1f);
                                    float alpha = Mathf.Lerp(timeAnimatedFloat.Start, timeAnimatedFloat.End, 
                                        timeAnimatedFloat.Curve.Evaluate(lerp));

                                    RendererSetMatAlpha(despawnable.Renderers, alpha);

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
                        if (despawnable.IsPooledObject)
                        {
                            RendererSetMatAlpha(despawnable.Renderers, 1f);
                            
                            // reset despawnable
                            despawnable.TimeAtSpawn = time;
                            despawnable.EnqueueDespawn = false;
                            despawnable.StartDespawn = false;
                            despawnable.ForceDespawn = false;

                            // TODO: fix this!!!
                            // TODO: do not destroy a pooled object find a way to disactivate it instead
                            
                            var entity = despawnable.gameObject.GetComponent<GameObjectEntity>().Entity;
                            EntityManager.DestroyEntity(entity);
                            transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            toDestroy.Add(DespawnableComponents.Transforms[i].gameObject);
                        }
                    }
                }
            }

            // destroy loop
            foreach (var go in toDestroy)
            {
                GameObject.Destroy(go);
            }
        }

        private static void RendererSetMatAlpha(List<MeshRenderer> meshRenderers, float alpha)
        {
            for (int j = 0; j < meshRenderers.Count; j++)
            {
                var meshRenderer = meshRenderers[j];

                Color color = meshRenderer.material.color;
                color.a = alpha;
                meshRenderer.material.color = color;
            }
        }
    }
}

