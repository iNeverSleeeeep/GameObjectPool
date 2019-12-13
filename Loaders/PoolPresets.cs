using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    [CreateAssetMenu(fileName = "PoolObjectPresets", menuName = "Fusion/PoolPresets", order = -2)]
    public class PoolPresets : ScriptableObject
    {
        public PoolObjectPreset[] poolPresets;

        public void Load(Func<float> processCallback)
        {

        }
    }
}

