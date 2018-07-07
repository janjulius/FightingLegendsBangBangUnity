using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class Bone : MonoBehaviour {
		private Mesh _mesh;
		public float Radius = .5f;
		public Color Color = Color.white;
		// Use this for initialization
		void Start() {
			string path = "bone";
			_mesh = (Mesh)Resources.Load(path, typeof(Mesh));
		}

		// Update is called once per frame
		void Update() {

		}
		
		private void OnDrawGizmos()
		{
			if (this.enabled)
			{
				if (_mesh == null)
				{
					string path = "bone";
					_mesh = (Mesh)Resources.Load(path, typeof(Mesh));
				}
				Gizmos.color = Color;
				Gizmos.DrawWireSphere(transform.position, Radius);
				foreach (Transform child in transform)
				{
					Vector3 nudge = (child.position - transform.position).normalized * Radius;

					float scaler = Vector3.Distance(child.position - nudge, transform.position + nudge);
					//Gizmos.DrawWireMesh(transform.position, new Vector3(scaler, scaler, scaler)) ;
					Vector3 look = child.position - transform.position;
					Quaternion rot = Quaternion.identity;
					if (look != Vector3.zero)
						rot = Quaternion.LookRotation(look);
					Gizmos.DrawWireMesh(_mesh, 0, transform.position + nudge, rot, new Vector3(Radius, Radius, scaler));

				}
			}
		}
		private void OnHideGizmos()
		{

		}
	}

}
