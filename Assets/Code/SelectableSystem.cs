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

            Vector3 hitPos = hitInfo.point;
            hitPos.y = 0;
            
            for (int i = 0; i < SelectableComponents.Length; i++)
            {
                Selectable selectable = SelectableComponents.Selectables[i];
                Transform transform = SelectableComponents.Transforms[i];

                if (selectable != null && transform != null)
                {
                    if (hit && selectable.Enabled)
                    {
                        Vector3 pos = transform.position;
                        pos.y = 0;

                        var meshRenderers = selectable.MeshRenderers;

                        float lerp = 1f - Mathf.Clamp(
                                         Vector3.Distance(pos, hitPos) / selectable.DistanceToFullFade,
                                         0f, 1f);
                        float alpha = selectable.MaxOpacity * MathFunctions.Easing.Smoothstep2(lerp);

                        if (alpha <= SelectableInactiveEpsilon)
                            alpha = 0f;
                        
                        for (int j = 0; j < meshRenderers.Count; j++)
                        {
                            var meshRenderer = meshRenderers[j];

                            Color color = meshRenderer.material.color;
                            color.a = alpha;
                            meshRenderer.material.color = color;
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