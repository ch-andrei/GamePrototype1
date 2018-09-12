using System.Collections;
using System.Collections.Generic;
using Code;
using Code.Agents.Components;
using Code.Components;
using Unity.Entities;
using UnityEngine;

using Code.Tools;

public class SpawnerSystem : ComponentSystem {
    
    struct SpawnerComponent
    {
        public int Length;
        public ComponentArray<Spawner> Spawners;
    }

    [Inject] SpawnerComponent SpawnerComponents;
        
    protected override void OnUpdate()
    {
        float time = Time.time;
        GameObject Agents = GameObject.FindGameObjectWithTag("Agents");
        
        for (int i = 0; i < SpawnerComponents.Length; i++)
        {
            Spawner spawner = SpawnerComponents.Spawners[i];

            if (Mathf.Abs(time - spawner.TimeAtLastSpawn) > spawner.SecondsBetweenSpawns)
            {
                for (int j = 0; j < spawner.NumObjectsSpawned; j++)
                {
                    var spawnedObject = Object.Instantiate(spawner.SpawnedObject, spawner.transform);
                    
                    spawnedObject.transform.parent = Agents.transform;
                    
                    Vector3 pos = WarpSampler.Warp(WarpSampler.EWarpType.EUniformDisk);
                    pos.z = pos.y;
                    pos.y = 0;
                    
                    spawnedObject.transform.position += spawner.SpawnDistance * pos;
                    
                    var despawnables = Utilities.GetComponentsInHierarchy<Despawnable>(spawnedObject.transform);
                    foreach (var despawnable in despawnables)
                    {
                        if (despawnable != null)
                            despawnable.TimeAtSpawn = time;
                    }
                }

                spawner.TimeAtLastSpawn = time;
            }
        }
    }
}