using Unity.Entities;
using UnityEngine;

namespace Code.Components
{
    [RequireComponent(typeof(GameObjectEntity))]
    public class Owned : MonoBehaviour
    {
        public int PlayerId;
    }
}
