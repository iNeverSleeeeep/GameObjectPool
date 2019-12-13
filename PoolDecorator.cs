using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    internal abstract class PoolDecorator : IPool
    {
        private IPool m_Pool = null;

        internal PoolDecorator(IPool pool)
        {
            Debug.Assert(pool != null);
            m_Pool = pool;
        }

        protected IPool Pool
        {
            get { return m_Pool; }
        }

        public int Count
        {
            get { return Pool.Count; }
        }

        public PoolPreset Preset
        {
            get { return Pool.Preset; }
        }

        public string Name
        {
            get { return Pool.Name; }
        }

        public virtual PoolObjectHolder Get()
        {
            var handler = Pool.Get();
            if (handler != null && handler.valid)
                handler.target.Pool = this;
            return handler;
        }

        public virtual void Release(PoolObject toRelease)
        {
            Pool.Release(toRelease);
        }

        public virtual void OnDestroy()
        {
            Pool.OnDestroy();
        }

        public virtual void Update()
        {
            Pool.Update();
        }

        public PoolObject ForceSpawm()
        {
            return Pool.ForceSpawm();
        }

        public void ForceRelease()
        {
            Pool.ForceRelease();
        }
    }
}

