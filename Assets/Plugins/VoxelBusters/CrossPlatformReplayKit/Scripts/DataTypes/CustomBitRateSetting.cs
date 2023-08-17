using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.ReplayKit.Internal
{
    [System.Serializable]
    public class CustomBitRateSetting
    {
        [SerializeField]
        private bool m_allowCustomBitrates = false;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float m_bitrateFactor = 0.5f;


        public bool AllowCustomBitrates
        {
            get { return m_allowCustomBitrates; }
            set
            {
                m_allowCustomBitrates = value;
            }
        }

        public float BitrateFactor
        {
            get { return m_bitrateFactor; }
            set
            {
                m_bitrateFactor = value;
            }
        }
    }
}
