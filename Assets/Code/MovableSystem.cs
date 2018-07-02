using Unity.Entities;
using UnityEngine;

namespace Code
{
    // PLACEHOLDER TO MOVE TARGETABLES
    public class MovableSystem : ComponentSystem
    {
        struct TowerTarget
        {
            public int Length;
            public ComponentArray<Transform> transforms;
            public ComponentArray<PlaceholderMovable> movable;
        }
    
        [Inject] TowerTarget movableEntities;
    
        protected override void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
    
            for (int i = 0; i < movableEntities.Length; i++)
            {
                PlaceholderMovable movable = movableEntities.movable[i];
                Transform transform = movableEntities.transforms[i];
                
                transform.position += deltaTime * movable.MoveDirection * movable.DirectionMultiplier;
                if (Vector3.Distance(transform.position, movable.PosAtMoveStart) > movable.DistanceToMove)
                {
                    movable.DirectionMultiplier = -movable.DirectionMultiplier; // invert movement direction
                    movable.PosAtMoveStart = transform.position; // record current position
                }
            }
        }
    }
}

