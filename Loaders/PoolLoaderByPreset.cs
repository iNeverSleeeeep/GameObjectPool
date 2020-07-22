using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    public static class PoolLoaderByPreset
    {
        public static void CreatePools(PoolObjectPreset preset)
        {
            if (preset.assets == null)
                return;
            for (var i = 0; i < preset.assets.Length; ++i)
            {
                var asset = preset.assets[i];
                var obj = Resources.Load<GameObject>(asset);
                PoolFactory.Create(preset.name + "." + asset, obj, preset.preset);
            }
        }
    }
}

