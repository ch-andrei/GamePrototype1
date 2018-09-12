using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class PrefabManager : MonoBehaviour
    {
        // Towers
        public GameObject TowerAimAssistIndicator;
        public GameObject TowerRangeIndicator;
        
        // Projectiles
        public GameObject ProjectileExplosive;
        public GameObject ProjectileDirectImpact;
        public GameObject EmptyShell;
        
        // Explosives
        public GameObject Explosion;
        
        // LineDrawer
        public GameObject LineDrawer;
        
        public List<ObjectPool> ObjectPools;
    }
}