using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

using MathFunctions;

public class Turret : MonoBehaviour
{
    [Range(1, 100)]
    public int FireRate = 10;
    
    [Range(1, 25)]
    public float FireRange = 10f;
    
    [Range(0f, 1f)]
    public float Damage = 0.1f;
    
    [Range(0.1f, 1000f)]
    public float MaxRotationSpeed = 25f; // maximum speed of rotation in degrees
    public float MaxRotationSpeedRadians => MathFunctions.AngleConversion.Deg2Rad(MaxRotationSpeed);

    [Range(0f, 90f)]
    public float MaxYRotationDelta = 25f; // maximum Y rotation angle in relation to horizon
    public float MaxYRotationDeltaRadians => MathFunctions.AngleConversion.Deg2Rad(MaxYRotationDelta);
    
    public GameObject Target { get; set; }

    public void Start()
    {
        Target = null;
    }
}
