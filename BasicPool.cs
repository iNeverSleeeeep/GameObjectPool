using LuaInterface;
using Nirvana;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    internal interface IPool
    {
        string Name { get; }
        int Count { get; }
        PoolObjectHolder Get();
        void Release(PoolObject toRelease);
        void OnDestroy();
        void Update();
        PoolPreset Preset { get; }
        PoolObject ForceSpawm();
        void ForceRelease();
    }

    internal class BasicPool : IPool
    {
        internal static ObjectPool<PoolObjectHolder> HolderPool = new ObjectPool<PoolObjectHolder>(holder=>holder.OnGet(), holder => holder.OnRelease());
        private Stack<PoolObject> m_CachedObjects = new Stack<PoolObject>();
        private PoolObject m_Original = null;
        private PoolPreset m_Preset = null;

        internal BasicPool(PoolObject original, PoolPreset preset, string name)
        {
            Debug.Assert(original != null);
            Name = name;
            m_Original = original;
            m_Preset = preset;
            original.Pool = this;
        }

        public int Count
        {
            get { return m_CachedObjects.Count; }
        }

        public PoolPreset Preset
        {
            get
            {
                return m_Preset;
            }
        }

        public string Name { get; private set; }

        public virtual PoolObjectHolder Get()
        {
            var holder = HolderPool.Get();
            PoolObject target = null;
            if (m_CachedObjects.Count > 0)
            {
                target = m_CachedObjects.Pop();
                target.Reset();
                target.Pool = this;
            }
            else
                target = ForceSpawm();
            target.Holder = holder;
            return holder;
        }

        public void Release(PoolObject toRelease)
        {
            if (toRelease != null)
                m_CachedObjects.Push(toRelease);
        }

        public void OnDestroy()
        {
            while (m_CachedObjects.Count > 0)
                m_CachedObjects.Pop().gameObject.SafeDestroySelf();
        }

        public void Update()
        {
            
        }

        public PoolObject ForceSpawm()
        {
            var poolObject = GameObject.Instantiate(m_Original.gameObject, PoolManager.Root).GetComponent<PoolObject>();
            poolObject.Pool = this;
            return poolObject;
        }

        public void ForceRelease()
        {
            if (m_CachedObjects.Count > 0)
                m_CachedObjects.Pop().gameObject.SafeDestroySelf();
        }
    }
}

