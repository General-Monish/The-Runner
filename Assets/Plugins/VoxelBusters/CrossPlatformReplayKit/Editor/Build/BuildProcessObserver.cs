using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace VoxelBusters.ReplayKit.Internal
{
    internal class BuildProcessObserver : IActiveBuildTargetChanged,
#if !UNITY_2018_1_OR_NEWER
    IPreprocessBuild, IPostprocessBuild
#else
    IPreprocessBuildWithReport, IPostprocessBuildWithReport
#endif
    {
#region IActiveBuildTargetChanged implementation

        public int callbackOrder
        {
            get
            {
                return (int.MaxValue - 1);
            }
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            BuildProcessManager.OnActiveBuildTargetChanged(previousTarget, newTarget);
        }

#endregion

#region IPreprocessBuild implementation

#if !UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            BuildProcessManager.OnPreprocessBuild(new BuildInfo() { Target = target, Path = path });
        }
#else
        public void OnPreprocessBuild(BuildReport report)
        {
            BuildProcessManager.OnPreprocessBuild(new BuildInfo() { Target = report.summary.platform, Path = report.summary.outputPath });
        }
#endif

#endregion

#region IPostprocessBuild implementation

#if !UNITY_2018_1_OR_NEWER
        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            BuildProcessManager.OnPostprocessBuild(new BuildInfo() { Target = target, Path = path });
        }
#else
        public void OnPostprocessBuild(BuildReport report)
        {
            BuildProcessManager.OnPostprocessBuild(new BuildInfo() { Target = report.summary.platform, Path = report.summary.outputPath });
        }
#endif

#endregion
    }
}
#endif