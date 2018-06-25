using System.Collections;
using System.Collections.Generic;
using Code.Towers.Components;
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
        public ComponentArray<NavMeshAgent> NavMeshAgents;
    }
    
    [Inject] AgentComponent AgentComponents;
    
    protected override void OnUpdate()
    {
        for (int i = 0; i < AgentComponents.Length; i++)
        {
            AgentComponents.NavMeshAgents[i].destination = OwnedComponents.Owneds[0].gameObject.transform.position;
        }
    }
}
