using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        public delegate void PoolObjectResetDelegate(string poolName, PoolObject poolObject);
        public static PoolObjectResetDelegate onPoolObjectReleaseCallback;

        private static Transform m_Root = null;
        public static Transform Root
        {
            get
            {
                if (m_Root == null)
                {
                    m_Root = new GameObject("PoolObjectRoot").transform;
                    m_Root.SetParent(Instance().transform);
                    m_Root.gameObject.SetActive(false);
                }
                return m_Root;
            }
        }

        private void Update()
        {
            foreach (var poolKV in Pools)
                poolKV.Value.Update();
        }

        Dictionary<string, IPool> Pools = new Dictionary<string, IPool>();

        public int PoolCount
        {
            get
            {
                return Pools.Count;
            }
        }

        internal void Add(string poolName, IPool pool)
        {
            Pools.Add(poolName, pool);
        }

        internal IPool Remove(string poolName)
        {
            IPool pool = null;
            if (Pools.TryGetValue(poolName, out pool))
            {
                Pools.Remove(poolName);
            }
            return pool;
        }

        internal IPool GetPool(string poolName)
        {
            IPool pool = null;
            Pools.TryGetValue(poolName, out pool);
            return pool;
        }

        public PoolObjectHolder GetObject(string poolName)
        {
            IPool pool = null;
            if (Pools.TryGetValue(poolName, out pool))
            {
                return pool.Get();
            }
            return null;
        }

        public void ReleaseObject(PoolObjectHolder toRelease)
        {
            PoolReleaser.Release(toRelease);
        }
    }
}

