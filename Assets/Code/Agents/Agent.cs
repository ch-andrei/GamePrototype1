using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.AI;

public class Agent : MonoBehaviour 
{
    public GameObject Goal;

    public bool HasGoal;

    public void Start()
    {
        HasGoal = false;
    }
}
