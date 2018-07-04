using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Code.Towers.Components;
using UnityEngine;
using Unity.Entities;
using Random = System.Random;

namespace Code.Towers
{
    public enum EProjectileType
    {
        Explosive,
        DirectImpact,
        EmptyShell,
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

            for (int i = 0; i < TurretProjectiles.Length; i++)
            {
                Transform transform = TurretProjectiles.Transforms[i];
                Projectile projectile = TurretProjectiles.Projectiles[i];
                Despawnable despawnable = TurretProjectiles.Despawnables[i];

                ProjectileColisionData colisionData = CheckCollision(transform, projectile);
                if (colisionData.Hit)
                {
                    if (projectile.ProjectileType == EProjectileType.DirectImpact)
                    {
                        DirectImpactColision(colisionData);
                    }
                    else if (projectile.ProjectileType == EProjectileType.Explosive)
                    {
                        ExplosiveColision(colisionData);
                    }
                    else
                    {
                        // TODO
                        // handle other projectile type colisions
                    }

                    // update
                    projectile.PreviousPosition = transform.position;
                    
                    // force immediate despawn of the projectile
                    despawnable.ForceDespawn = true;
                }
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
            
            projectile.PreviousPosition = transform.position;

            return colisionData;
        }

        private static void DirectImpactColision(ProjectileColisionData colisionData)
        {
            // do nothing
        }
        
        private static void ExplosiveColision(ProjectileColisionData colisionData)
        {
            GameObject explosion = GameObject.Instantiate(Bootstrapper.PrefabManager.Explosion);
            explosion.transform.rotation = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere);
            explosion.transform.position = colisionData.HitPosition;
        }

    }
}