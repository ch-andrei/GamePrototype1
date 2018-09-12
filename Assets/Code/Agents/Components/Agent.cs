using Unity.Entities;
using UnityEngine;

namespace Code.Agents.Components
{
    [RequireComponent(typeof(GameObjectEntity))]
    public class Agent : MonoBehaviour 
    {
        public GameObject Goal;

        public bool HasGoal;

        public void Start()
        {
            HasGoal = false;
        }
    }
}
