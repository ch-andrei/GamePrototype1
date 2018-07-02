using System;
using System.Collections;
using System.Collections.Generic;
using Code.Towers.Components;
using UnityEngine;
using Unity.Entities;

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
    
    public class ProjectileSystem : ComponentSystem
    {
        struct TurretProjectile
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Projectile> Projectiles;
//            public ComponentArray<Collider> colliders;
        }
    
        [Inject] TurretProjectile TurretProjectiles;

        protected override void OnUpdate()
        {
            var deltaTime = Time.deltaTime;

            for (int i = 0; i < TurretProjectiles.Length; i++)
            {
                Transform transform = TurretProjectiles.Transforms[i];
                Projectile projectile = TurretProjectiles.Projectiles[i];
//                Collider collider = turretProjectiles.colliders[i];

                Vector3 posAfterColisionCheck = transform.position;
                if (CheckCollision(transform, projectile, out posAfterColisionCheck))
                {
                    if (projectile.ProjectileType == EProjectileType.DirectImpact)
                    {
                        DirectImpactCollison(posAfterColisionCheck);
                    }
                    else if (projectile.ProjectileType == EProjectileType.Explosive)
                    {
                        ExplosiveCollision(posAfterColisionCheck);
                    }
                    else
                    {
                        // TODO
                    }

                    projectile.PreviousPosition = transform.position;
                }
            }
        }

        private static bool CheckCollision(Transform transform, Projectile projectile, out Vector3 posColision)
        {
            Vector3 posCrt = transform.position;
            Vector3 posPrev = projectile.PreviousPosition;

            posColision = Vector3.zero;
            
            projectile.PreviousPosition = transform.position;
            
            return false;
        }

        private static void DirectImpactCollison(Vector3 posColision)
        {
            
        }
        
        private static void ExplosiveCollision(Vector3 posColision)
        {
            
        }

    }
}