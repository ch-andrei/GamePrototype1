using System.Collections;
using System.Collections.Generic;
using Code;
using Code.Agents.Components;
using Code.Components;
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

            // draw path
            if (navMeshAgent != null || navMeshAgent.path != null)
            {
                var line = navMeshAgent.gameObject.GetComponentInChildren<LineRenderer>();
                line.material = new Material( Shader.Find( "Sprites/Default" ) ) { color = Color.yellow };
                var path = navMeshAgent.path;
                line.SetVertexCount(path.corners.Length);
                for (int k = 0; k < path.corners.Length; k++)
                {
                    line.SetPosition(k, path.corners[k]);
                }
            }
            
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

                    // despawn if goal is reached
                    if (distanceToGoal < 2f && !despawnable.StartDespawn)
                    {
                        despawnable.EnqueueDespawn = true;
                    }
                }
                else
                {
                    // TODO: find closest Owned with other faction
                    navMeshAgent.destination = OwnedComponents.Owneds[0].gameObject.transform.position;
                    
//                    navMeshAgent.desiredVelocity
                    agent.HasGoal = true;
                }
            }
        }
    }
}
