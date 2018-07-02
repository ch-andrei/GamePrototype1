using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderMovable : MonoBehaviour
{
    public Vector3 MoveDirection;
    public float DistanceToMove;

    [HideInInspector] public Vector3 PosAtMoveStart;

    [HideInInspector] public int DirectionMultiplier;
    
    // Use this for initialization
    void Start()
    {
        this.PosAtMoveStart = this.transform.position;
        this.DirectionMultiplier = 1;
    }
}
