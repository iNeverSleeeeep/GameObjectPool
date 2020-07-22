using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fusion
{
    public class MonoSingleton<T> : MonoBehaviour
    {
        private static T m_Instance;

        public static T Instance()
        {
            if (m_Instance == null)
            {

            }
            return m_Instance;
        }
    }
}
