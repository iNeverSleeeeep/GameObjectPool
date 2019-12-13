using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    /*
     * 有最大数量限制的对象池
     * 如果申请超过最大数量的对象，则会返回最先申请的对象
     * 
     */
    internal class MaxActiveDecorator : PoolDecorator
    {
        internal MaxActiveDecorator(IPool pool) : base(pool) { }

        private LinkedList<PoolObject> m_ActiveObjects = new LinkedList<PoolObject>();
        
        public override PoolObjectHolder Get()
        {
            // 这里的实现逻辑是 先强制回收第一个 再取
            if (m_ActiveObjects.Count >= Preset.maxActiveObjects)
                m_ActiveObjects.First.Value.Release();
            var holder = base.Get();
            m_ActiveObjects.AddLast(holder.target);
            return holder;
        }

        public override void Release(PoolObject toRelease)
        {
            m_ActiveObjects.Remove(toRelease);
            base.Release(toRelease);
        }
    }
}

