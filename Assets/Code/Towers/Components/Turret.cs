using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using Code;
using Code.Tools;

[RequireComponent(typeof(GameObjectEntity))]
public class Turret : MonoBehaviour
{
    public GameObject Gun;
    
    // maximum speed of rotation in degrees
    [Range(0.1f, 1000f)] public float MaxRotationSpeed = 25f; 
    public float MaxRotationSpeedRadians => Angles.Deg2Rad(MaxRotationSpeed);
    
    // maximum X rotation (up/down) angle in relation to horizon 
    [Range(0f, 90f)] public float MaxRotationXVertical = 25f; 
    public float MaxRotationXVerticalRadians => Angles.Deg2Rad(MaxRotationXVertical);

    [Range(0.0001f, 1f)] public float IdleNewDirectionProbability = 0.005f;

    public bool ShowAimAssist;
       
    [HideInInspector] public GameObject AimAssist;
    public float AimAssistMaxLength = 10f;
    public float AimAssistMaxRayCastLength = 100f;
    
    [HideInInspector] public Vector3 TargetPos;
    
    [HideInInspector] public GameObject TargetObject;
        
    public void Start()
    {
        TargetPos = Gun.transform.position + Gun.transform.forward; // initialize to forward
        AimAssist = Instantiate(Bootstrapper.PrefabManager.TowerAimAssistIndicator, Gun.transform);
//        Debug.Log("Instantiated TowerAimAssistIndicator");
    }
}
