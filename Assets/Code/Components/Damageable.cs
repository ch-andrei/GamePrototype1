using System.Collections.Generic;
using Code.Tools;
using UnityEngine;

namespace Code.Components
{
    public class Damageable : MonoBehaviour
    {
        public float MaxHealthPoints;
        [HideInInspector] public float HealthPoints;

        public AnimatedColor Animated;

        public List<MeshRenderer> Renderers;

        public void Start()
        {
            Renderers = Utilities.GetComponentsInHierarchy<MeshRenderer>(transform);
            HealthPoints = MaxHealthPoints;
        }
    }
}