using UnityEngine;

namespace Code.Towers.Components
{
    [RequireComponent(typeof(Projectile))]
    public class ExplosiveProjectile : MonoBehaviour
    {
        public AnimationCurve DamageFalloff;
        public float DamageMaxRange = 5f;
        
        [HideInInspector] public Projectile Projectile;

        public void Start()
        {
            Projectile = gameObject.GetComponent<Projectile>();
            Projectile.ProjectileType = EProjectileType.Explosive;
            DamageFalloff = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }
}