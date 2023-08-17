using UnityEngine;
namespace VoxelBusters.ReplayKit.Internal
{
    [System.Serializable]
    public class InitialisationSettings
    {
        [SerializeField]
        private bool m_requestScreenRecordPermissionOnInitialise = false;

        [SerializeField]
        private bool m_requestMicrophonePermissionOnInitialise = false;

        public bool RequestScreenRecordPermissionOnInitialise
        {
            get { return m_requestScreenRecordPermissionOnInitialise; }
            set
            {
                m_requestScreenRecordPermissionOnInitialise = value;
            }
        }

        public bool RequestMicrophonePermissionOnInitialise
        {
            get { return m_requestMicrophonePermissionOnInitialise; }
            set
            {
                m_requestMicrophonePermissionOnInitialise = value;
            }
        }
    }
}