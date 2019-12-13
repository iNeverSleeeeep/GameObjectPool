using Nirvana;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    [Serializable]
    public class PoolPreset
    {
        // 预加载
        public int preloadCount = 0;
        public int preloadPerFrame = 0;

        // 自动回收
        public int autoRelease = 0;

        // 最大数量
        public int maxActiveObjects = 0;

        public static PoolPreset Default = new PoolPreset();
    }

    [CreateAssetMenu(fileName = "PoolObjectPreset", menuName = "Fusion/PoolObjectPreset", order = -1)]
    public class PoolObjectPreset : ScriptableObject
    {
        public PoolPreset preset;
        // 资源引用
        public AssetID[] assets;
    }
}
