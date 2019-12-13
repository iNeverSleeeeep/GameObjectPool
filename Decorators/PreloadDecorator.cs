using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    internal class PreloadDecorator : PoolDecorator
    {
        internal PreloadDecorator(IPool pool) :base (pool) { }

        private int m_Preloaded = 0;

        public override void Update()
        {
            if (m_Preloaded < Preset.preloadCount)
            {
                for (var i = 0; i < Preset.preloadPerFrame && m_Preloaded < Preset.preloadCount; ++i)
                {
                    Release(ForceSpawm());
                    m_Preloaded++;
                }
            }
        }
    }
}

