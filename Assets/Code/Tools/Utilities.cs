using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Tools
{
    public static class Utilities
    {
        public static List<T> GetComponentsInChildren<T>(Transform transform)
        {
            var components = new List<T>();

            foreach (Transform child in transform)
            {
                components.Add(child.GetComponent<T>());
                components.AddRange(GetComponentsInChildren<T>(child));
            }

            return components;
        }
    }
}