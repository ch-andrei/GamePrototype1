using System.Collections;
using System.Collections.Generic;
using Code;
using Code.Towers;
using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(GameObjectEntity))]
public class Shooter : MonoBehaviour
{
    public EProjectileType ProjectileType;

    [Range(0.1f, 1000f)] public float ProjectileForce;

    [Range(0.0001f, 100)] public float SecondsBetweenShots;
    public float FireRatePerMinute => 60f / SecondsBetweenShots;
    
    [Range(0.0001f, 90f)] public float LockonAngle = 1f;
    
    [Range(1, 25)] public float FireRange = 10f;

    [Range(0.001f, 0.1f)] public float RangeIndicatorThickness;
    [HideInInspector] public GameObject RangeIndicator;
    
    [HideInInspector] public float TimeAtShot;
    
    public void Start()
    {
        RangeIndicator = Instantiate(Bootstrapper.PrefabManager.TowerRangeIndicator, this.transform);
        RangeIndicator.transform.localScale = new Vector3(2 * FireRange, RangeIndicatorThickness, 2 * FireRange);
        RangeIndicator.transform.position = transform.parent.transform.position + 
                                            new Vector3(0f, RangeIndicatorThickness, 0f) ;
    }
}
