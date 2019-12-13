using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    /*
     * 自动释放采取如下策略
     * 如果{AutoReleaseTime}内没有使用这个对象池，则会每帧释放一个对象
     * 直至对象数量与设置的自动释放数量相等为止
     * 
     */
    internal class AutoReleaseDecorator : PoolDecorator
    {
        internal AutoReleaseDecorator(IPool pool) : base(pool) { }

        private static readonly float AutoReleaseTime = 15.0f;
        private float m_LastAutoReleaseTime = Time.unscaledTime;
        
        public override PoolObjectHolder Get()
        {
            m_LastAutoReleaseTime = Time.unscaledTime;
            return base.Get();
        }

        public override void Update()
        {
            base.Update();
            if (Pool.Count > Preset.autoRelease && Time.unscaledTime > m_LastAutoReleaseTime + AutoReleaseTime)
                ForceRelease();
        }
    }
}

