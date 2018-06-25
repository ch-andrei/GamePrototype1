using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Unity.Entities;

using MathFunctions;
using Code.Towers.Components;

public class TowerAimingSystem : ComponentSystem
{
    struct AnimatedTurretsComponent
    {
        public int Length;
        public ComponentArray<Transform> transforms;
        public ComponentArray<Turret> turrets;
        public ComponentArray<AnimatedTurret> animated;
    }
    
    [Inject] AnimatedTurretsComponent animatedTurretsComponent;

    struct TowerTarget
    {
        public int Length;
        public ComponentArray<Transform> transforms;
        public ComponentArray<Targetable> targetables;
    }
    
    [Inject] TowerTarget targetComponents;

    protected override void OnUpdate()
    {
        float newDirectionProbability = 0.005f;
        float targetingAngle = 1f;
        
        var deltaTime = Time.deltaTime;
        
        for (int i = 0; i < animatedTurretsComponent.Length; i++)
        {
            Turret turret = animatedTurretsComponent.turrets[i];
            AnimatedTurret animated = animatedTurretsComponent.animated[i];

            // find closest target for current tower and update
            {
                int closestTarget = -1;
                float closestDistance = float.MaxValue;
                for (int j = 0; j < targetComponents.Length; j++)
                {
                    float distance = Vector3.Distance(targetComponents.transforms[j].position, turret.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = j;
                    }
                }

                if (closestTarget != -1)
                {
                    turret.Target = targetComponents.transforms[closestTarget].gameObject;
                }
            }

            bool targetLock = false;
            bool hasTarget = turret.Target != null;
            bool targetInRange = hasTarget && 
                                 TargetInRange(animated.transform.position, turret.Target.transform.position,
                                     turret.FireRange);

            bool idleAnimation = !targetInRange;

            animated.Target = targetInRange ? turret.Target.transform.position : animated.Target;

            Vector3 toTarget = animated.Target - animated.Gun.transform.position;
            float angleDeltaCrt = Vector3.Angle(toTarget, animated.Gun.transform.forward);
            
            // recalculate animation parameters
            Quaternion toTargetQuaternion = Quaternion.LookRotation(toTarget);
            
            if (angleDeltaCrt < targetingAngle)
            {
                targetLock = true;
            }
            
            // apply rotation
            if (!targetLock)
            {
                float lerpRatio = turret.MaxRotationSpeed / (angleDeltaCrt + 1e-9f) * deltaTime;
                
                // calculate the Quaternion for the rotation
                var rotGun = Quaternion.Slerp(animated.Gun.transform.rotation, toTargetQuaternion, lerpRatio);
                var rotTurret = Quaternion.Slerp(animated.Gun.transform.rotation, toTargetQuaternion, lerpRatio);
                
                // Apply the rotation to turret (vertical)
                animated.transform.rotation = rotTurret;
                animated.transform.eulerAngles = new Vector3(0f, animated.transform.eulerAngles.y, 0f); // only rotate y
                
                // Apply local rotation to gun (horizontal)
                animated.Gun.transform.rotation = rotGun; 
                animated.Gun.transform.localEulerAngles = new Vector3(
                    Mathf.Clamp(animated.Gun.transform.localEulerAngles.x, 
                        -turret.MaxYRotationDelta, turret.MaxYRotationDelta), 
                    0f, 
                    0f); // only rotate x
               
                toTarget = animated.Target - animated.Gun.transform.position;
                
                float angleDeltaAfter = Vector3.Angle(toTarget, animated.Gun.transform.forward);
                Debug.Log("angle diff b/a " + angleDeltaCrt + "/" + angleDeltaAfter + 
                          "; Has Target " + hasTarget + " in range " + targetInRange + 
                          "; targetLock " + targetLock + "; is idle " + idleAnimation 
                          );
            }
            else
            {
                // shoot at target
                
            }
            
            
            if (idleAnimation && targetLock)
            {
                // update rotation direction
                var random = UnityEngine.Random.Range(0f, 1f);
                if (random < newDirectionProbability)
                {
                    float theta = 2 * Mathf.PI * UnityEngine.Random.Range(0f, 1f);                    
                    Vector3 direction = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));
                    
                    // compute a random target within fire range
                    animated.Target = animated.Gun.transform.position + direction;
                }
            }
        }
    }
    
    public static bool TargetInRange(Vector3 position, Vector3 targetPosition, float range)
    {
        return Vector3.Distance(position, targetPosition) < range;
    }

    public static bool ModifyTarget(Vector3 target, AnimatedTurret animatedTurret)
    {
        if (animatedTurret.Target == target)
            return false;
        else
        {
            animatedTurret.Target = target;
            return true;
        }
    }
}
