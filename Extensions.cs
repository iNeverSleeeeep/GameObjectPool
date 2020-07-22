using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fusion
{
    internal static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp == null)
                comp = go.AddComponent<T>();
            return comp;
        }

        public static void SafeDestroySelf(this GameObject go)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                GameObject.Destroy(go);
            else
                GameObject.DestroyImmediate(go);
#else
            GameObject.Destroy(go);
#endif
        }
    }
}
