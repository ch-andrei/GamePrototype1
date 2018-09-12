using System.Collections;
using System.Collections.Generic;
using Code.Tools;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

namespace Code
{    
    [RequireComponent(typeof(GameObjectEntity))]
    public class Selectable : MonoBehaviour
    {
        public bool Enabled = true;
        
        [HideInInspector] public List<SelectableObject> SelectionIndicators;

        public void Start()
        {
            SelectionIndicators = Utilities.GetComponentsInHierarchy<SelectableObject>(this.transform);
            
            if (SelectionIndicators.Count == 0)
                Debug.Log("Got zero selectables.");
            
//            Debug.Log("GOT MY SELECTABLES");
        }
    }
}
