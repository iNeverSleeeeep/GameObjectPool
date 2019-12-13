using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    internal class SetParentDecorator : PoolDecorator
    {
        internal SetParentDecorator(IPool pool) :base (pool) { }

        public override void Release(PoolObject toRelease)
        {
            base.Release(toRelease);
            if (toRelease != null)
                toRelease.transform.SetParent(PoolManager.Root);
        }
    }
}

