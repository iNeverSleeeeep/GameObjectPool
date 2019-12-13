using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    public class PoolObjectHolder
    {

        public PoolManager.PoolObjectResetDelegate onReleaseCallback;

        private PoolObject m_Target = null;
        public PoolObject target
        {
            get
            {
                return m_Target;
            }
            internal set
            {
                Debug.Assert(m_Target == null || value == null);
                if (m_Target == value)
                    return;
                if (m_Target != null)
                {
                    if (onReleaseCallback != null)
                    {
                        onReleaseCallback(m_Target.Pool.Name, m_Target);
                        onReleaseCallback = null;
                    }
                }
                m_Target = value;
            }
        }

        public Transform transform
        {
            get
            {
                if (valid == false)
                    return null;
                return m_Target.transform;
            }
        }

        public GameObject gameObject
        {
            get
            {
                if (valid == false)
                    return null;
                return m_Target.gameObject;
            }
        }

        public bool valid
        {
            get { return m_Target != null; }
        }

        internal void OnGet()
        {
            target = null;
        }
        internal void OnRelease()
        {
            target = null;
        }
    }

}
