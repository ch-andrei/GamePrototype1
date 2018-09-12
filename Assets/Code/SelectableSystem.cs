using Code.Tools;
using Unity.Entities;
using UnityEngine;

namespace Code
{
    public class SelectableSystem : ComponentSystem
    {
        struct DamageableComponent
        {
            public int Length;
            public ComponentArray<Transform> Transforms;
            public ComponentArray<Selectable> Selectables;
        }

        [Inject] DamageableComponent SelectableComponents;

        public const float SelectableInactiveEpsilon = 1e-3f;

        protected override void OnUpdate()
        {
            // raycast from mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out var hitInfo);
            
            for (int i = 0; i < SelectableComponents.Length; i++)
            {
                Selectable selectable = SelectableComponents.Selectables[i];
                Transform transform = SelectableComponents.Transforms[i];

                if (selectable != null && transform != null)
                {
                    if (selectable.Enabled)
                    {
                        float distance;
                        if (hit)
                        {
                            Vector3 pos = transform.position;
                            Vector3 hitPos = hitInfo.point;
                            distance = Vector3.Distance(pos, hitPos);
                        }
                        else
                        {
                            distance = float.MaxValue;
                        }
                        
                        foreach (var selectableObject in selectable.SelectionIndicators)
                        {
                            float alpha;
                            if (hit)
                            {
                                float lerp = 1f - Mathf.Clamp(
                                                 Mathf.Max(distance - selectableObject.DistanceBeforeFade, 0f) 
                                                 / selectableObject.DistanceToFullFade, 
                                                 0f, 1f);
                                alpha = selectableObject.MaxOpacity * Easing.Smoothstep2(lerp);
                                alpha = alpha <= SelectableInactiveEpsilon ? 0 : alpha;
                            }
                            else
                            {
                                alpha = 0f;
                            }
                            
                            var renderers = selectableObject.SelectionRenderers;
                            for (int j = 0; j < renderers.Count; j++)
                            {
                                var renderer = renderers[j];
                            
                                Color color = renderer.material.color;
                                color.a = alpha;
                                renderer.material.color = color;
                            }
                        }
                    }
                    else
                    {
                        // nothing to do
                    }
                }
            }
        }
    }
}