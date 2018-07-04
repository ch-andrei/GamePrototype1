using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    // TODO: refactor -> add this to other Animated -> inheritance
    // need to adjust all animations to work once refactored
    [System.Serializable] public class SimpleAnimated
    {
        public bool Enabled = true;
        
        public float Start;
        public float End;
        
        public AnimationCurve Curve;

        public SimpleAnimated()
        {
            // default values
            Enabled = true;
            Start = 1f;
            End = 0f;
            Curve = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }
    
    [System.Serializable] public class TimeAnimatedFloat
    {
        public bool Enabled = true;
     
        [Range(0f, 100f)] public float AnimationTime;
        [Range(0f, 100f)] public float AnimationTimeDelay;
        
        public float Start;
        public float End;
        
        public AnimationCurve Curve;

        [HideInInspector] public float AnimationTimeStart;
        [HideInInspector] public float AnimationTimeStartDelayed => AnimationTimeStart + AnimationTimeDelay;

        public TimeAnimatedFloat()
        {
            // default values
            Enabled = true;
            AnimationTime = 1f;
            AnimationTimeDelay = 0f;
            Start = 1f;
            End = 0f;
            Curve = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }
    
    [System.Serializable] public class AnimatedColor
    {
        public Color Start;
        public Color End;
        
        public AnimationCurve Curve;
        
        public AnimatedColor()
        {
            Start = Color.magenta;
            End = Color.magenta;
        }
    }
    
//    [System.Serializable] public class DespawnableAnimated : Animated
//    {
//        public DespawnableAnimated() : base()
//        {
//            AnimationTime = 1;
//            Curve = AnimationCurve.Linear(0, 0, 1, 1);
//            Enabled = false;
//        }
//    }
}
