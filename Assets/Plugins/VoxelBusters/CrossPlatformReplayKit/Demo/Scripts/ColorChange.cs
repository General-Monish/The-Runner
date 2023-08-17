using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VoxelBusters.Demos.ReplayKit
{
    public class ColorChange : MonoBehaviour
    {
        [SerializeField]
        private Image m_target = null;

        private float m_currentHue = 0.0f;

        private void Update()
        {
            Color color = Color.HSVToRGB(m_currentHue, 1.0f, 1.0f);
            m_target.color = color;

            // Update color
            m_currentHue = Mathf.PingPong(Time.time/10, 1.0f);
        }

    }
}
