using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Remoting.Messaging;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace MathFunctions {
    public static class Easing
    {
        public static float Smoothstart2(float x)
        {
            return x * x;
        }
        
        public static float Smoothstart3(float x)
        {
            return x * x * x;
        }
        
        public static float Smoothstart4(float x)
        {
            return x * x * x * x;
        }
        
        public static float SmoothStop2(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }
        
        public static float SmoothStop3(float x)
        {
            return 1 - (1 - x) * (1 - x) * (1 - x);
        }
        
        public static float SmoothStop4(float x)
        {
            return 1 - (1 - x) * (1 - x) * (1 - x) * (1 - x);
        }

        public static float Smoothstep2(float x)
        {
            return Mathf.Lerp(Smoothstart2(x), SmoothStop2(x), x);
        }
        
        public static float Smoothstep3(float x)
        {
            return Mathf.Lerp(Smoothstart3(x), SmoothStop3(x), x);
        }
        
        public static float Smoothstep4(float x)
        {
            return Mathf.Lerp(Smoothstart4(x), SmoothStop4(x), x);
        }

        public static float Scale(Function<float, float> function, float x)
        {
            return function(x) * x;
        }
        
        public static float Rescale(Function<float, float> function, float x)
        {
            return function(x) * (1 - x);
        }

        public static float Arch(float x)
        {
            return x * (1 - x);
        }

        public static float NormalizedBezier3(float a, float b, float x)
        {
            float s = 1f - x;
            float x2 = x * x;
            float s2 = s * s;
            float x3 = x2 * x;
            return (3f * a * s2 * x) + (3f * b * x2) + x3;
        }
    }

    public static class AngleConversion
    {
        public static float Deg2Rad(float deg)
        {
            return deg * Mathf.PI / 180f;
        }

        public static float Rad2Deg(float rad)
        {
            return rad / Mathf.PI * 180;
        }
    }
}
