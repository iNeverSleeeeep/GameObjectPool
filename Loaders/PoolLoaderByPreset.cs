using Nirvana;
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
                AssetManager.LoadObject(asset, typeof(GameObject), obj =>
                {
                    if (obj == null)
                        return;
                    PoolFactory.Create(preset.name + "." + asset.AssetName, obj as GameObject, preset.preset);
                });
            }
        }

        public static IEnumerator CreatePoolsCoroutine(PoolObjectPreset preset)
        {
            if (preset.assets == null)
                yield break;
            for (var i = 0; i < preset.assets.Length; ++i)
            {
                var asset = preset.assets[i];
                var wait = AssetManager.LoadObject(asset.BundleName, asset.AssetName, typeof(GameObject));
                yield return wait;

                if (!string.IsNullOrEmpty(wait.Error))
                {
                    Debug.LogError(wait.Error);
                    yield break;
                }

                var obj = wait.GetObject();
                if (obj == null)
                    yield break;
                PoolFactory.Create(preset.name + "." + asset.AssetName, obj as GameObject, preset.preset);
            }
            
        }
    }
}

