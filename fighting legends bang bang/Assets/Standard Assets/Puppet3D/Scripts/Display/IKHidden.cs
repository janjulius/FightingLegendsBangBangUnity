using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class IKHidden : MonoBehaviour {

		[HideInInspector]
		public IKControl IKHandle;
		private void Update()
		{
			if (IKHandle != null && !IKHandle.IKVisibility)
				transform.hideFlags = HideFlags.HideInHierarchy;
			else
				transform.hideFlags = HideFlags.None;
		}
	}

}