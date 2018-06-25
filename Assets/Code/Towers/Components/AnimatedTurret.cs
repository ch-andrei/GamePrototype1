using System.Collections.Generic;
using UnityEngine;

namespace Code.Towers.Components
{
    public class AnimatedTurret : MonoBehaviour
    {
        // the object that will be rotated
        public GameObject Gun;

        public Vector3 Target;
        
        public void Start()
        {
            Target = this.Gun.transform.position + this.Gun.transform.forward;
        }
    }
}