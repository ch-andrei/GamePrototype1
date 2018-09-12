using UnityEngine;
using Unity.Entities;

namespace Code.Towers.Components
{
    [RequireComponent(typeof(GameObjectEntity))]
    public class Projectile : MonoBehaviour
    {
        public float DamageOnImpact = 1f;
        
        [HideInInspector] public EProjectileType ProjectileType;
        [HideInInspector] public Vector3 PreviousPosition;
    }
}