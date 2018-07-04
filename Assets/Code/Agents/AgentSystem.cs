using System.Collections;
using System.Collections.Generic;
using Code;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class AgentSystem : ComponentSystem {
    
    struct OwnedComponent
    {
        public int Length;
        public ComponentArray<Owned> Owneds;
    }
    
    [Inject] OwnedComponent OwnedComponents;
    
    struct AgentComponent
    {
        public int Length;
        public ComponentArray<Transform> Transforms;
        public ComponentArray<Agent> Agents;
        public ComponentArray<NavMeshAgent> NavMeshAgents;
        public ComponentArray<Despawnable> Despawnables;
    }
    
    [Inject] AgentComponent AgentComponents;
    
    protected override void OnUpdate()
    {
        float time = Time.time;
        
        for (int i = 0; i < AgentComponents.Length; i++)
        {
            var agent = AgentComponents.Agents[i];
            var navMeshAgent = AgentComponents.NavMeshAgents[i];
            var despawnable = AgentComponents.Despawnables[i];

            if (!navMeshAgent.isOnNavMesh)
            {
                despawnable.ForceDespawn = true;
            }
            else
            {
                if (agent.HasGoal)
                {
                    float distanceToGoal = Vector3.Distance(navMeshAgent.destination,
                        AgentComponents.Transforms[i].position);

                    if (distanceToGoal < 2f && !despawnable.StartDespawn)
                    {
                        despawnable.EnqueueDespawn = true;
                    }
                }
                else
                {
                    // TODO: find closest Owned with other faction
                    navMeshAgent.destination = OwnedComponents.Owneds[0].gameObject.transform.position;
                    agent.HasGoal = true;
                }
            }
        }
    }
}
