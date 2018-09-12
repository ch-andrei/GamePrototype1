using System.Collections.Generic;
using Code.Tools;
using UnityEngine;

namespace Code
{
    public class SelectableObject : MonoBehaviour
    {
        [Range(0.01f, 1f)] public float MaxOpacity;
        [Range(0.01f, 100f)] public float DistanceBeforeFade;
        [Range(0.01f, 100f)] public float DistanceToFullFade;
        
        [HideInInspector] public List<Renderer> SelectionRenderers;

        public void Awake()
        {
            SelectionRenderers = Utilities.GetComponentsInHierarchy<Renderer>(this.transform);
        }
    }
}