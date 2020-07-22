using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    internal static class PoolReleaser
    {
        internal static void Release(PoolObjectHolder toRelease)
        {
            if (toRelease.valid)
            {
                var list = ListPool<PoolObject>.Get();
                toRelease.target.GetComponentsInChildren(true, list);
                // GetComponentsInChildren是有顺序的，所以只要从后向前循环一遍即可回收所有
                for (var i = list.Count - 1; i >= 0; --i)
                    list[i].Release();
                ListPool<PoolObject>.Release(list);
            }
            BasicPool.HolderPool.Release(toRelease);
        }
    }
}

