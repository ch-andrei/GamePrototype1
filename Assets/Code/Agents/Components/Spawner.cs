using UnityEngine;

namespace Code.Agents.Components
{
    public class Spawner : MonoBehaviour 
    {
        public GameObject SpawnedObject;

        [HideInInspector] public float TimeAtLastSpawn;

        [Range(1, 20)] public int NumObjectsSpawned;
    
        [Range(0.1f, 60f)] public float SecondsBetweenSpawns;

        [Range(0f, 5f)] public float SpawnDistance = 2f;

        public void Start()
        {
            this.TimeAtLastSpawn = -float.MaxValue;
        }
    }
}
