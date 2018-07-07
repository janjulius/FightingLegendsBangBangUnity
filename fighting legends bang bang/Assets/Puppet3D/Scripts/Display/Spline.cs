using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class Spline : Control {
		public Vector3 HandleSize =  Vector3.one;
		// Use this for initialization
		void Start() {
		
		}

		// Update is called once per frame
		void Update() {

		}

		private void OnDrawGizmos()
		{
			if (Active && this.enabled)
			{
				Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
				Gizmos.matrix = rotationMatrix;

				Gizmos.color = Color.magenta;

				Gizmos.DrawWireCube(Vector3.zero, new Vector3(transform.localScale.x * HandleSize.x, transform.localScale.y * HandleSize.y, transform.localScale.z * HandleSize.z));

			}

		}
	}

}