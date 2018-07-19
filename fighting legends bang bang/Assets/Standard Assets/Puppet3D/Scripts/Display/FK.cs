using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class FK : Control {

		public float Transparency = 0f;
		public float HandleSize = 2f;
		[HideInInspector]
		public IKControl IKHandle;
		// Use this for initialization
		void Start() {
		
		}

		// Update is called once per frame
		void Update() {
			if (IKHandle != null && !IKHandle.FKVisibility)
			{
					transform.hideFlags = HideFlags.HideInHierarchy;

			}
			else
				transform.hideFlags = HideFlags.None;

		}
		private void OnDrawGizmosSelected()
		{
#if UNITY_EDITOR

			if (Active)
			{
				if (UnityEditor.Selection.activeGameObject == this && Transparency == 0f)
					UnityEditor.Selection.activeGameObject = IKHandle.gameObject;
			}
#endif

		}
		private void OnDrawGizmos()
		{
			if (Active)
			{
				Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
				Gizmos.matrix = rotationMatrix;
				Gizmos.color = Color.Lerp(new Color(1, 0, 0, 0), Color.red, Transparency);
				if (Transparency > 0f)
					Gizmos.DrawWireCube(Vector3.zero, transform.localScale * HandleSize);
			}


		}
	}

}