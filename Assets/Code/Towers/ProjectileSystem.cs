using System;
using System.Collections;
using System.Collections.Generic;
using Code.Towers.Components;
using UnityEngine;
using Unity.Entities;

using Code;

namespace Code.Towers
{
    public enum EProjectileType
    {
        Explosive,
        DirectImpact,
    }
    
    public struct ProjectileSpawnData
    {
        public Vector3 Position;
        public Vector3 Direction;
        public float TimeAtSpawn;
        public EProjectileType Type;
        public float Force;
    }

    public struct ProjectileColisionData
    {
        public bool Hit;
        public Vector3 HitPosition;
        public GameObject HitObject;
    }
    
    public class ProjectileSystem : ComponentSystem
    {
        struct TurretProjectile
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Projectile> Projectiles;
            public ComponentArray<Despawnable> Despawnables;
        }
    
        [Inject] TurretProjectile TurretProjectiles;

        protected override void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            var time = Time.time;

            for (int i = 0; i < TurretProjectiles.Length; i++)
            {
                var transform = TurretProjectiles.Transforms[i];
                var projectile = TurretProjectiles.Projectiles[i];
                var despawnable = TurretProjectiles.Despawnables[i];

//                // draw line for bullet trajectory
//                GameObject linePrefab = Bootstrapper.PrefabManager.LineDrawer;
//                GameObject line = GameObject.Instantiate(linePrefab, transform);
//                var lineRenderer = line.GetComponent<LineRenderer>();
//                lineRenderer.SetPosition(0, transform.position);
//                lineRenderer.SetPosition(1, projectile.PreviousPosition);
                
                ProjectileColisionData colisionData = CheckCollision(transform, projectile);
                if (colisionData.Hit)
                {
                    if (projectile.ProjectileType == EProjectileType.DirectImpact)
                    {
                        DirectImpactColision(colisionData, projectile);
                    }
                    else if (projectile.ProjectileType == EProjectileType.Explosive)
                    {
                        ExplosiveColision(colisionData, projectile);
                    }
                    else
                    {
                        // TODO: handle other projectile type colisions
                        Debug.Log("Unknown projectile type collided with a surface.");
                    }
                    
                    // update
                    projectile.PreviousPosition = colisionData.HitPosition;
                    
                    // force immediate despawn of the projectile
                    despawnable.ForceDespawn = true;
                }
                
                // update position
                projectile.PreviousPosition = transform.position;
            }
        }

        private static ProjectileColisionData CheckCollision(Transform transform, Projectile projectile)
        {
            Vector3 posCrt = transform.position;
            Vector3 posPrev = projectile.PreviousPosition;
            Vector3 toCrt = posCrt - posPrev;

            ProjectileColisionData colisionData = new ProjectileColisionData();
            
            RaycastHit raycastHit;
            // TODO: maybe use Physics.SphereCast instead
            colisionData.Hit = Physics.Raycast(posPrev, toCrt.normalized, out raycastHit, 
                maxDistance: toCrt.magnitude);

            if (colisionData.Hit)
            {
                colisionData.HitPosition = raycastHit.point;
                colisionData.HitObject = raycastHit.collider.gameObject;
            }
            
            return colisionData;
        }

        private static void DirectImpactColision(ProjectileColisionData colisionData, Projectile projectile)
        {            
            // apply damage
            // TODO: add faction
            DamageableSystem.ApplyDamage(colisionData.HitObject, projectile.DamageOnImpact);
        }
        
        private static void ExplosiveColision(ProjectileColisionData colisionData, Projectile projectile)
        {
            // spawn explosion effect
            GameObject explosion = GameObject.Instantiate(Bootstrapper.PrefabManager.Explosion);
            explosion.transform.rotation = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere);
            explosion.transform.position = colisionData.HitPosition;
            
            // define damage
            var explosiveProjectile = projectile.gameObject.GetComponent<ExplosiveProjectile>();
            DamageData damageData = new DamageData()
            {
                DamageAmount = projectile.DamageOnImpact,
                DamageRange = explosiveProjectile.DamageMaxRange,
                DamageFalloff = explosiveProjectile.DamageFalloff,
                HitPosition = colisionData.HitPosition,
                FactionId = -1, // TODO: add faction
            };
            
            // query apply damage
            var damageFalloff = projectile.gameObject.GetComponent<ExplosiveProjectile>().DamageFalloff;
            DamageableSystem.EnqueueApplyDamage(damageData);
        }

    }
}