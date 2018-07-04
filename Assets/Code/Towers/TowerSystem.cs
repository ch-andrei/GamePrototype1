using Code.Tools;
using UnityEngine;
using Unity.Entities;

using Code.Tools;
using Code.Towers.Components;

namespace Code.Towers
{
    public class TowerAimingSystem : ComponentSystem
    {
        struct AnimatedTurretsComponent
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Turret> Turrets;
            public ComponentArray<Shooter> Shooters;
        }
        
        [Inject] AnimatedTurretsComponent TurretComponents;
    
        struct TowerTarget
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Targetable> Targetables;
        }
        
        [Inject] TowerTarget TargetComponents;
    
        protected override void OnUpdate()
        {
            var time = Time.time;
            var deltaTime = Time.deltaTime;
            
            for (int i = 0; i < TurretComponents.Length; i++)
            {
                Transform transform = TurretComponents.Transforms[i];
                Turret turret = TurretComponents.Turrets[i];
                Shooter shooter = TurretComponents.Shooters[i];
    
                // find closest target for current tower and update
                {
                    int closestTarget = -1;
                    float closestDistance = float.MaxValue;
                    
                    int closestTargetNoCheck = -1;
                    float closestDistanceNoCheck = float.MaxValue;
                    
                    
                    for (int j = 0; j < TargetComponents.Length; j++)
                    {
                        Transform target = TargetComponents.Transforms[j];
                        float distance = Vector3.Distance(target.position, transform.position);
                        
                        if (distance < closestDistance)
                        {
                            if (HasTargetableTarget(turret, shooter, target.transform))
                            {
                                closestTarget = j;
                                closestDistance = distance;
                            }
                        }

                        // without checking targetable
                        // needed for aiming at object if its too close, but out of range
                        if (distance < closestDistanceNoCheck)
                        {
                            closestTargetNoCheck = j;
                            closestDistanceNoCheck = distance;
                        }
                    }
    
                    turret.TargetObject = null;
                    // update target
                    if (closestTarget == -1)
                    {
                        if (closestTargetNoCheck != -1)
                            turret.TargetObject = TargetComponents.Transforms[closestTargetNoCheck].gameObject;
                    }
                    else
                    {
                        turret.TargetObject = TargetComponents.Transforms[closestTarget].gameObject;
                    }
                }
    
                // animation and aiming control flags
                bool hasTargetableTarget = turret.TargetObject != null &&
                                           TargetInRange(turret, shooter, turret.TargetObject.transform);
                bool idleAnimation = !hasTargetableTarget;
                
                turret.AimAssist.SetActive(turret.ShowAimAssist && hasTargetableTarget);
                turret.TargetPos = hasTargetableTarget ? turret.TargetObject.transform.position : turret.TargetPos;
    
                Vector3 toTarget = turret.TargetPos - turret.Gun.transform.position;
                Quaternion toTargetQuaternion = Quaternion.LookRotation(toTarget);
                
                bool targetLock = false;
                float angleDeltaCrt = Vector3.Angle(toTarget, turret.Gun.transform.forward);
                if (angleDeltaCrt < shooter.LockonAngle)
                {
                    targetLock = true;
                }
    
                {
                    // apply rotation
                    
                    float lerpRatio = turret.MaxRotationSpeed / (Mathf.Max(angleDeltaCrt, 0.001f)) * deltaTime;
                    lerpRatio = Mathf.Clamp(lerpRatio, 0f, 1f);
                    
                    // calculate the Quaternion for the rotation
                    var rotGun = Quaternion.Slerp(turret.Gun.transform.rotation, toTargetQuaternion, lerpRatio);
                    var rotTurret = Quaternion.Slerp(turret.Gun.transform.rotation, toTargetQuaternion, lerpRatio);
                    
                    // Apply the rotation to turret (horizontal: left/right)
                    transform.rotation = rotTurret;
                    transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f); // only rotate y
                    
                    // Apply local rotation to gun (vertical: up/down)
                    turret.Gun.transform.rotation = rotGun; 
                    turret.Gun.transform.localEulerAngles = new Vector3(
                        Mathf.Clamp(turret.Gun.transform.localEulerAngles.x, 
                            -turret.MaxRotationXVertical, turret.MaxRotationXVertical), 
                        0f, 
                        0f); // only rotate x
                   
                    toTarget = turret.TargetPos - turret.Gun.transform.position;
                    
    //                float angleDeltaAfter = Vector3.Angle(toTarget, turret.Gun.transform.forward);
    //                Debug.Log("angle diff b/a/del/speed " + angleDeltaCrt + "/" + angleDeltaAfter + 
    //                          "/" + (angleDeltaCrt - angleDeltaAfter) + 
    //                          "/" + (angleDeltaCrt - angleDeltaAfter) / deltaTime +
    //                          "; lerpRatio " + lerpRatio + "; is targetable " + targetIsTargetable + 
    //                          "; targetLock " + targetLock + "; is idle " + idleAnimation 
    //                          );
                }
                
                if (hasTargetableTarget && targetLock)
                {
                    // shoot at target
                    
                    if (time - shooter.TimeAtShot >= shooter.SecondsBetweenShots)
                    {
                        var projectileSpawnData = new ProjectileSpawnData()
                        {
                            Position = turret.Gun.transform.position,
                            Direction = turret.Gun.transform.forward,
                            TimeAtSpawn = time,
                            Type = shooter.ProjectileType,
                            Force = shooter.ProjectileForce,
                        };
                        
                        // TODO; convert to a pool of objects
                        TurretSpawnProjectile(turret, shooter, projectileSpawnData);
                                                
                        shooter.TimeAtShot = time;
                    }
                    
                }
                
                if (idleAnimation && targetLock)
                {
                    // update rotation direction
                    var random = UnityEngine.Random.Range(0f, 1f);
                    if (random < turret.IdleNewDirectionProbability)
                    {
                        // compute a random vector
                        float theta = 2 * Mathf.PI * UnityEngine.Random.Range(0f, 1f); // random angle over circle
    //                    theta = UnityEngine.Random.Range(0f, 1f) > 0.5 ? Mathf.PI - 1e-3f : 2 * Mathf.PI - 1e-3f; // test
                        Vector3 direction = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
                        
                        // compute a random target within fire range
                        turret.TargetPos = transform.parent.position + direction * 10f;
                    }
                }
            }
        }
    
        private static void TurretSpawnProjectile(Turret turret, Shooter shooter, 
            ProjectileSpawnData projectileSpawnData)
        {
            GameObject projectilePrefab = null;
            if (projectileSpawnData.Type == EProjectileType.DirectImpact)
            {
                projectilePrefab = Bootstrapper.PrefabManager.ProjectileDirectImpact;
            }
            else if (projectileSpawnData.Type == EProjectileType.Explosive)
            {
                projectilePrefab = Bootstrapper.PrefabManager.ProjectileExplosive;
            }
            else
            {
                // TODO:
                // handle other projectile types if any
            }

            if (projectilePrefab == null)
                return;

            GameObject shellPrefab = Bootstrapper.PrefabManager.EmptyShell;
            
            // Parent object
            GameObject Projectiles = GameObject.FindGameObjectWithTag("Projectiles");
            Transform parent = Projectiles == null ? null : Projectiles.transform;
            
            GameObject shellObject = GameObject.Instantiate(shellPrefab,  parent);
            GameObject projectileObject = GameObject.Instantiate(projectilePrefab, parent);
            
            Vector3 spawnPos = turret.Gun.transform.position;

            shellObject.transform.position = spawnPos;
            projectileObject.transform.position = spawnPos + turret.Gun.transform.forward * 2;

            projectileObject.transform.rotation = Quaternion.LookRotation(turret.Gun.transform.forward);

            ConfigureShellForce(shellObject);
            ConfigureDespawnable(shellObject, projectileSpawnData);
            ConfigureDespawnable(projectileObject, projectileSpawnData);
            ConfigureProjectile(projectileObject, projectileSpawnData);
        }

        private static void ConfigureShellForce(GameObject gameObject)
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            Vector3 shellDirection = WarpSampler.Warp(WarpSampler.EWarpType.ECosineHemisphere);
            rigidbody.AddForce(shellDirection * 2f, ForceMode.Impulse);
            rigidbody.AddTorque(shellDirection);
        }

        private static void ConfigureDespawnable(GameObject gameObject, ProjectileSpawnData projectileSpawnData)
        {
            // configure despawnable component of the projectile
            Despawnable despawnable = gameObject.GetComponent<Despawnable>();
            despawnable.TimeAtSpawn = projectileSpawnData.TimeAtSpawn;
        }
        
        private static void ConfigureProjectile(GameObject gameObject, ProjectileSpawnData projectileSpawnData)
        {
            // configure projectile component of the projectile
            Projectile projectile = gameObject.gameObject.GetComponent<Projectile>();
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.AddForce(projectileSpawnData.Direction * projectileSpawnData.Force, ForceMode.Impulse);
        }
    
        private static bool HasTargetableTarget(Turret turret, Shooter shooter, Transform target)
        {
            Quaternion toTarget = Quaternion.LookRotation(target.transform.position - 
                                                                       turret.Gun.transform.position);
    
            bool inRange = TargetInRange(turret, shooter, target);
            inRange &= Mathf.Abs(toTarget.eulerAngles.x) <= turret.MaxRotationXVertical;
    
            return inRange;
        }
        
        private static bool TargetInRange(Turret turret, Shooter shooter, Transform target)
        {
            return Vector3.Distance(target.transform.position, turret.transform.position) < shooter.FireRange;
        }
    }
}