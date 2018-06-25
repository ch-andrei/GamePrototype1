using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEditor.Experimental.UIElements;

public class Targetable : MonoBehaviour
{
    public Vector3 MoveDirection;
    public float DistanceToMove;
    
    [HideInInspector]
    public Vector3 previousPos;
        
    [HideInInspector]
    public int c;
    
    // Use this for initialization
    void Start ()
    {
        this.previousPos = this.transform.position;
        this.c = 1;
    }
}

public class TargetableSystem : ComponentSystem
{
    struct TowerTarget
    {
        public int Length;
        public ComponentArray<Transform> transforms;
        public ComponentArray<Targetable> targetables;
    }

    [Inject] TowerTarget targetEntities;

    protected override void OnUpdate()
    {
        var deltaTime = Time.deltaTime;

        for (int i = 0; i < targetEntities.Length; i++)
        {
            Targetable t = targetEntities.targetables[i];
            t.transform.position += deltaTime * t.MoveDirection * t.c;
            if (Vector3.Distance(t.transform.position, t.previousPos) > t.DistanceToMove)
            {
                t.c = -t.c;
                t.previousPos = t.transform.position;
            }
        }
    }
}