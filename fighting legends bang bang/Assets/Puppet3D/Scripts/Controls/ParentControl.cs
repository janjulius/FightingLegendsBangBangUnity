using UnityEngine;
using System.Collections;

namespace Puppet3D
{
	public class ParentControl : Control
	{

		public GameObject bone;
		public Vector3 HandleScale = Vector3.one;
		public Mesh OverrideMesh = null;
		public Vector3 OverrideMeshOffset = Vector3.zero;
		//public bool IsEnabled;
		public bool Point;
		public bool Orient;
		public bool Scale;
		public bool ConstrianedPosition;
		public bool ConstrianedOrient;
		public bool MaintainOffset;
		public Vector3 OffsetPos;
		public Vector3 OffsetScale = new Vector3(1, 1, 1);
		public Quaternion OffsetOrient;


		public void ParentControlRun()
		{

			if (Orient)
			{
				if (MaintainOffset)
					bone.transform.rotation = transform.rotation * OffsetOrient;
				else
					bone.transform.rotation = transform.rotation;
			}
			if (Point)
			{
				if (MaintainOffset)
					bone.transform.position = transform.TransformPoint(OffsetPos);
				else
					bone.transform.position = transform.position;

			}
			if (Scale)
				bone.transform.localScale = new Vector3(transform.localScale.x * OffsetScale.x, transform.localScale.y * OffsetScale.y, transform.localScale.z * OffsetScale.z);

			if (ConstrianedPosition)
				if (!Point)
					transform.position = bone.transform.position;
		}
		private void OnDrawGizmos()
		{
			if (Active)
			{
				if (Point)
					Gizmos.color = Color.blue;
				else
					Gizmos.color = Color.cyan;
				if (OverrideMesh)
				{
					Gizmos.DrawWireMesh(OverrideMesh, 0, transform.TransformPoint(OverrideMeshOffset), transform.rotation, HandleScale);

				}
				else
				{
					Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
					Gizmos.matrix = rotationMatrix;					

					Gizmos.DrawWireCube(Vector3.zero, new Vector3(transform.localScale.x * HandleScale.x, transform.localScale.y * HandleScale.y, transform.localScale.z * HandleScale.z) );
				}

			}

		}
	}
}
