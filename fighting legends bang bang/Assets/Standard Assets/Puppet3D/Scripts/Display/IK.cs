using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	public class IK : Control
	{
		public float HandleSize = 1f;
		private void OnDrawGizmos()
		{
			if (Active)
			{
				Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
				Gizmos.matrix = rotationMatrix;
				Gizmos.color = Color.green;

				Gizmos.DrawWireCube(Vector3.zero, transform.localScale * HandleSize);
			}
		}
	}
}
