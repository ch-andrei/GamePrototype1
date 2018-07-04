using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Code
{
    public class Scalable : MonoBehaviour
    {
        public TimeAnimatedFloat TimeAnimatedFloat;

        public void Start()
        {
            this.TimeAnimatedFloat.AnimationTimeStart = Time.time;
        }
    }

    public class ScalableSystem : ComponentSystem
    {
        struct ScalableComponents
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Scalable> Scalables;
        }

        [Inject] ScalableComponents Scalables;

        protected override void OnUpdate()
        {
            var time = Time.time;

            for (int i = 0; i < Scalables.Length; i++)
            {
                Transform transform = Scalables.Transforms[i];
                Scalable scalable = Scalables.Scalables[i];
                
                TimeAnimatedFloat timeAnimatedFloat = scalable.TimeAnimatedFloat;
                if (timeAnimatedFloat.Enabled)
                {
                    float lerp = Mathf.Clamp((time - timeAnimatedFloat.AnimationTimeStart) / timeAnimatedFloat.AnimationTime, 0f, 1f);
                    float scale = Mathf.Lerp(timeAnimatedFloat.Start, timeAnimatedFloat.End, timeAnimatedFloat.Curve.Evaluate(lerp));

                    transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }
}

