using UnityEngine;

using System.Collections;

namespace Puppet3D
{
	[ExecuteInEditMode]
	public class Guide : MonoBehaviour {
		public float Radius = .1f;
		public Vector3 HandlePos;
		public bool SnapToCentre = true;		
		public bool Mirror = true;
		public Guide MirroredGuide;
		

		[HideInInspector]
		public Color _Color;
	
		void OnDrawGizmosSelected()
		{			
			_Color = 2f * Color.green;
			if (Mirror && MirroredGuide != null )
			{
				MirroredGuide._Color = _Color;
				MirroredGuide.HandlePos = HandlePos;
				MirroredGuide.gameObject.transform.position = new Vector3(-1f * transform.position.x, transform.position.y, transform.position.z);
			}
			Gizmos.color = _Color;
			Gizmos.DrawSphere(HandlePos, Radius);
		}
		private void OnDrawGizmos()
		{
			Gizmos.color = _Color;
			_Color = 2f * new Color(1f, .5f, 0.75f);
			if (SnapToCentre)
			{
				Ray worldRay = new Ray(transform.position + Vector3.forward * 1000f, Vector3.back);
				Ray worldRay2 = new Ray(transform.position + Vector3.back * 1000f, Vector3.forward);

				RaycastHit hit, hit2;

				if (Physics.Raycast(worldRay, out hit))
				{
					if (Physics.Raycast(worldRay2, out hit2))
					{
						HandlePos = (hit.point + hit2.point) * .5f;
					}
					else
						HandlePos = hit.point;
				}
				else if (Physics.Raycast(worldRay2, out hit2))
					HandlePos = hit2.point;
				else
					HandlePos = transform.position;
			}
			else
				HandlePos = transform.position;

			Gizmos.DrawSphere(HandlePos, Radius);

		}
		private void OnEnable()
		{
			Refresh();
		}
		public void Refresh()
		{
			_Color = 2f * Color.green;
			if (Mirror && MirroredGuide != null)
			{
				MirroredGuide._Color = _Color;
				MirroredGuide.HandlePos = HandlePos;
				MirroredGuide.gameObject.transform.position = new Vector3(-1f * transform.position.x, transform.position.y, transform.position.z);
			}
		}
	}

}