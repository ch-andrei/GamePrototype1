using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Code.Agents
{
    public class Damageable : MonoBehaviour
    {
        public float MaxHealthPoints;
        [HideInInspector] public float HealthPoints;

        public AnimatedColor Animated;

        public void Start()
        {
            HealthPoints = MaxHealthPoints;
        }
    }

    public struct DamageData
    {
        public float DamageAmount;
        public int FactionId;
    }
    
    

    public class DamageableSystem : ComponentSystem
    {
        struct DamageableComponent
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Damageable> Damageables;
            public ComponentArray<Despawnable> Despawnables;
        }

        [Inject] DamageableComponent DamageableComponents;

        public const float ForceDespawnEpsilon = 1e-3f;

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            for (int i = 0; i < DamageableComponents.Length; i++)
            {
                Damageable damageable = DamageableComponents.Damageables[i];
                Despawnable despawnable = DamageableComponents.Despawnables[i];

                if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                    damageable.HealthPoints -= 1;
                
                if (damageable.HealthPoints <= 0)
                {
                    despawnable.EnqueueDespawn = true;
                }
                else
                {
                    var meshRenderers = despawnable.MeshRenderers;

                    float lerp = 1f - Mathf.Clamp(damageable.HealthPoints / damageable.MaxHealthPoints, 0f, 1f);
                    Color color = Color.Lerp(damageable.Animated.Start, damageable.Animated.End, 
                        damageable.Animated.Curve.Evaluate(lerp));

                    for (int j = 0; j < meshRenderers.Count; j++)
                    {
                        var meshRenderer = meshRenderers[j];

                        // maintain alpha from material but not the color
                        color.a = meshRenderer.material.color.a;
                        meshRenderer.material.color = color;
                    }
                }
            }
        }
        
        
    }
}