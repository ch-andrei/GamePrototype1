using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    [System.Serializable] public class Animated
    {
        [Range(0.01f, 100f)] public float AnimationTime;
        
        public float Start;
        public float End;

        public AnimationCurve Curve;

        public bool Enabled;
    
        [HideInInspector] public float AnimationTimeStart;

        public Animated()
        {
            AnimationTime = 1;
            Curve = AnimationCurve.Linear(0, 0, 1, 1);
            Enabled = true;
        }
    }
    
    [System.Serializable] public class DespawnableAnimated : Animated
    {
        public DespawnableAnimated() : base()
        {
            AnimationTime = 1;
            Curve = AnimationCurve.Linear(0, 0, 1, 1);
            Enabled = false;
        }
    }
}
