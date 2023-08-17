using UnityEngine;
using System.Collections;
using VoxelBusters.ReplayKit.Common.Utility;

namespace VoxelBusters.ReplayKit.Internal

{
	public class NotifySettingsOnValueChangeAttribute : ExecuteOnValueChangeAttribute 
	{
		#region Constructors

        public  NotifySettingsOnValueChangeAttribute () : base ("OnPropertyModified")
		{}

		#endregion
	}
}