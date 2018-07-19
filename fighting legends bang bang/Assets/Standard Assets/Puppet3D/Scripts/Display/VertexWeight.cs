using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class VertexWeight : MonoBehaviour {
		public float HandleRadius = .01f;
		// Use this for initialization
		void Start() {
		
		}

		// Update is called once per frame
		void Update() {

		}
		
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, .1f* HandleRadius);

		}
	}

}