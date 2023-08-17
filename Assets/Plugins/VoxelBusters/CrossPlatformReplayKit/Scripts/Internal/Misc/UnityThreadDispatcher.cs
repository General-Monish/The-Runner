using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.ReplayKit.Common.DesignPatterns;

namespace VoxelBusters.ReplayKit.Internal
{
	public class UnityThreadDispatcher : SingletonPattern<UnityThreadDispatcher>
    {
        #region Fields

        private Queue<Action> m_queue = new Queue<Action>();

        #endregion

        #region Static Block

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            // Initialise
            UnityThreadDispatcher   dispatcher  = Instance;
        }

        #endregion

        #region Unity methods

        private void Update()
        {
            while (m_queue.Count > 0)
            {
                Action action = m_queue.Dequeue();
                action();
            }
        }

        #endregion

        #region Public static methods

        public static void Enqueue(Action action)
        {
            UnityThreadDispatcher   dispatcher  = Instance;
            dispatcher.m_queue.Enqueue(action);
        }

        #endregion

    }
}
