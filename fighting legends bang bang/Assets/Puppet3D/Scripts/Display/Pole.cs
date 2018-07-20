using UnityEngine;
using System.Collections;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class Pole : Control {
		public float HandleRadius = 1f;
		// Use this for initialization
		void Start() {
		
		}

		// Update is called once per frame
		void Update() {

		}
		
		private void OnDrawGizmos()
		{
			if (Active)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(transform.position, .1f * HandleRadius);
			}

		}
	}

}