using System.Collections;
using System.Collections.Generic;
using Code.Tools;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

namespace Code
{
    public class Selectable : MonoBehaviour
    {
        public bool Enabled = true;

        [Range(0.01f, 1f)] public float MaxOpacity = 0.15f;
        [Range(0.01f, 100f)] public float DistanceToFullFade = 1f;
        
        [HideInInspector] public List<MeshRenderer> MeshRenderers;

        public void Start()
        {
            MeshRenderers = Utilities.GetComponentsInHierarchy<MeshRenderer>(this.transform);
        }
    }
}