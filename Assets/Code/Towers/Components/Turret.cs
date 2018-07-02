using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using Code;

[RequireComponent(typeof(GameObjectEntity))]
public class Turret : MonoBehaviour
{
    public GameObject Gun;
    
    // maximum speed of rotation in degrees
    [Range(0.1f, 1000f)] public float MaxRotationSpeed = 25f; 
    public float MaxRotationSpeedRadians => MathFunctions.Angles.Deg2Rad(MaxRotationSpeed);
    
    // maximum X rotation (up/down) angle in relation to horizon 
    [Range(0f, 90f)] public float MaxRotationX = 25f; 
    public float MaxRotationXRadians => MathFunctions.Angles.Deg2Rad(MaxRotationX);

    [Range(0.0001f, 1f)] public float IdleNewDirectionProbability = 0.005f;

    public bool ShowAimAssist;
       
    [HideInInspector]
    public GameObject AimAssist;
    
    [HideInInspector]
    public Vector3 TargetPos;
    
    [HideInInspector]
    public GameObject TargetObject;
        
    public void Start()
    {
        TargetPos = Gun.transform.position + Gun.transform.forward; // initialize to forward
        AimAssist = Instantiate(Bootstrapper.PrefabManager.TowerAimAssistIndicator, Gun.transform);
    }
}
