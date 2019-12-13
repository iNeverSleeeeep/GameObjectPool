using Assets.Scripts.Common;
using Nirvana;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    public class PoolFactory
    {
        public static void Create(string poolName, GameObject original, PoolPreset preset = null)
        {
            if (preset == null)
                preset = PoolPreset.Default;

            var pool = new BasicPool(original.GetOrAddComponent<PoolObject>(), preset, poolName);

            IPool decorator = new SetParentDecorator(pool);
            if (preset.preloadCount > 0)
                decorator = new PreloadDecorator(decorator);

            if (preset.autoRelease > 0)
                decorator = new AutoReleaseDecorator(decorator);

            if (preset.maxActiveObjects > 0)
                decorator = new MaxActiveDecorator(decorator);

            PoolManager.Instance().Add(poolName, decorator);
        }

        public static void Destroy(string poolName)
        {
            var pool = PoolManager.Instance().Remove(poolName);
            pool.OnDestroy();
        }
    }
}
