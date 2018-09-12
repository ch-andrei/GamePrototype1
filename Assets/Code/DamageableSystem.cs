using System.Collections.Generic;
using Code;
using Code.Components;
using Code.Tools;
using Unity.Entities;
using UnityEngine;

namespace Code
{
    public struct DamageData
    {
        public Vector3 HitPosition;
        public float DamageAmount;
        public float DamageRange;
        public AnimationCurve DamageFalloff;
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
        public const bool FriendlyFireEnabled = false;

        private static Queue<DamageData> DamageQueue;

        public static void ApplyDamage(Damageable damageable, float damage)
        {
            damageable.HealthPoints -= damage;
        }
        
        public static void ApplyDamage(GameObject hitObject, float damage)
        {
            var damageable = hitObject.GetComponent<Damageable>();
            if (damageable is null)
            {
                Debug.Log("Tried to damage a gameobject with no damageable component.");
            }
            else
            {
                ApplyDamage(damageable, damage);
            }
        }
        
        public static void EnqueueApplyDamage(DamageData data)
        {
            DamageQueue.Enqueue(data);
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            DamageQueue = new Queue<DamageData>();
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            while (DamageQueue.Count != 0)
            {
                var damageData = DamageQueue.Dequeue();

                for (int i = 0; i < DamageableComponents.Length; i++)
                {
                    Damageable damageable = DamageableComponents.Damageables[i];

                    float distance = Vector3.Distance(damageable.transform.position, damageData.HitPosition);
                    if (distance < damageData.DamageRange)
                    {
                        float lerp = 1f - Mathf.Clamp(distance / damageData.DamageRange, 0f, 1f);
                        float damage = damageData.DamageAmount * damageData.DamageFalloff.Evaluate(lerp);

                        ApplyDamage(damageable, damage);
                    }
                }
            }
            
            // apply color changes and despawn if needed
            for (int i = 0; i < DamageableComponents.Length; i++)
            {
                Transform transform = DamageableComponents.Transforms[i];
                Damageable damageable = DamageableComponents.Damageables[i];
                Despawnable despawnable = DamageableComponents.Despawnables[i];

                // check health points value
                if (damageable.HealthPoints <= 0)
                {
                    despawnable.EnqueueDespawn = true;
                }
                else
                {
                    // animate color of the material (ex: green to red, as health goes down)
                    var meshRenderers = damageable.Renderers;

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

