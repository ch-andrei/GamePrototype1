using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour 
{
    public GameObject SpawnedObject;

    [HideInInspector]
    public float TimeAtLastSpawn;

    [Range(1, 20)]
    public int NumObjectsSpawned;
    
    [Range(0.1f, 60f)]
    public float SecondsBetweenSpawns;

    public void Start()
    {
        this.TimeAtLastSpawn = -float.MaxValue;
    }
}
