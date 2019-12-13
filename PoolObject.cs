using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    public class PoolObject : MonoBehaviour
    {
        private IPool m_Pool = null;

        internal IPool Pool
        {
            get { return m_Pool; }
            set { m_Pool = value; }
        }

        [SerializeField] private List<Transform> m_Transforms = new List<Transform>();
        [SerializeField] private List<ParticleSystem> m_ParticleSystems = new List<ParticleSystem>();
        [SerializeField] private List<TrailRenderer> m_TrailRenderers = new List<TrailRenderer>();

        private PoolObjectHolder m_Holder = null;
        public PoolObjectHolder Holder
        {
            get
            {
                return m_Holder;
            }
            internal set
            {
                if (m_Holder == value)
                    return;
                ReleaseCurrentHolder();
                m_Holder = value;
                if (m_Holder != null)
                    m_Holder.target = this;
            }
        }

        internal void Release()
        {
            if (PoolManager.onPoolObjectReleaseCallback != null)
                PoolManager.onPoolObjectReleaseCallback(m_Pool.Name, this);
            m_Pool.Release(this);
            Holder = null;
        }

        internal void Reset()
        {
            foreach (var t in m_Transforms)
                Reset(t);
            foreach (var ps in m_ParticleSystems)
                Reset(ps);
            foreach (var ps in m_TrailRenderers)
                Reset(ps);
        }

        private void ReleaseCurrentHolder()
        {
            if (m_Holder != null)
            {
                m_Holder.target = null;
                m_Holder = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CacheComponents();
        }
#endif

        private void CacheComponents()
        {
            m_Transforms.Clear();
            m_ParticleSystems.Clear();
            m_TrailRenderers.Clear();
            GetComponentsInChildren(true, m_Transforms);
            GetComponentsInChildren(true, m_ParticleSystems);
            GetComponentsInChildren(true, m_TrailRenderers);
        }

        private void Reset(Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        private void Reset(ParticleSystem ps)
        {
            ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void Reset(TrailRenderer tr)
        {
            tr.Clear();
        }
    }
}

