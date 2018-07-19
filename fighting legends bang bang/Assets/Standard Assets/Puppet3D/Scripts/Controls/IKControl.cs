using UnityEngine;
using System.Collections.Generic;
namespace Puppet3D
{
	public class IKControl: MonoBehaviour
	{
		[Range (0f,1f)]
		public float IKFK;

		public bool FKVisibility = false;
		public bool IKVisibility = false;

		[HideInInspector]
		public Vector3 AimDirection;

		[HideInInspector]
		public Transform poleVector;
		[HideInInspector]
		public Vector3 UpDirection;

		[HideInInspector]
		public Vector3[] scaleStart =new Vector3[2];
		[HideInInspector]
		public Transform topJointTransformIK, middleJointTransformIK, bottomJointTransformIK;

		[HideInInspector]
		public Vector3 OffsetScale = new Vector3(1,1,1);

		[HideInInspector]
		public Transform IK_CTRL;

	    private Vector3 root2IK ;
	    private Vector3 root2IK2MiddleJoint;
		
		[HideInInspector]
		public int numberIkBonesIndex;

		[HideInInspector]
		public int numberOfBones = 4;
		[HideInInspector]
		public int iterations = 10;
		[HideInInspector]
		public float damping = 1;
		[HideInInspector]
		public Transform IKHandle;
		[HideInInspector]
		public Transform endTransform;
		[HideInInspector]
		public Transform startTransform;
		[HideInInspector]
		public List<Vector3> bindPose;
		[HideInInspector]
		public List<Transform> bindBones;
		[HideInInspector]
		public bool limitBones = true;
		[HideInInspector]
		public Quaternion Offset = Quaternion.identity;
		[HideInInspector]
		public Quaternion topJointTransform_OffsetRotation;
		[HideInInspector]
		public Quaternion topJointTransform_OffsetRotation2;
		[HideInInspector]
		public Quaternion middleJointTransform_OffsetRotation;
		[HideInInspector]
		public Vector3 bottomJointTransform_OffsetRotation;
		[HideInInspector]
		public bool bottomJointTransformMatchesIK_CTRLRotation = true;
		[HideInInspector]
		public bool debug;
		float angle;
		float topJointTransform_Length;
		float middleJointTransform_Length;
		float arm_Length;
		float IK_CTRLDistance;
		float adyacent;
		[HideInInspector]
		public bool DisableRotateAround = false;
		[HideInInspector]
		public Transform topJointTransform, middleJointTransform, bottomJointTransform;
		[HideInInspector]
		public Transform topJointTransformFK, middleJointTransformFK, bottomJointTransformFK;		
		[HideInInspector]
		public FK[] fks = new FK[3]; 


		public void CalculateIK()
		{
			

			if (topJointTransformIK != null && middleJointTransformIK != null && bottomJointTransformIK != null && poleVector != null && IK_CTRL != null)
			{
				topJointTransformIK.LookAt(IK_CTRL, poleVector.position - topJointTransformIK.position);

				topJointTransformIK.Rotate(topJointTransform_OffsetRotation.eulerAngles);

				Vector3 cross = Vector3.Cross(poleVector.position - topJointTransformIK.position, middleJointTransformIK.position - topJointTransformIK.position);



				topJointTransform_Length = Vector3.Distance(topJointTransformIK.position, middleJointTransformIK.position);
				middleJointTransform_Length = Vector3.Distance(middleJointTransformIK.position, bottomJointTransformIK.position);
				arm_Length = topJointTransform_Length + middleJointTransform_Length;
				IK_CTRLDistance = Vector3.Distance(topJointTransformIK.position, IK_CTRL.position);
				IK_CTRLDistance = Mathf.Min(IK_CTRLDistance, arm_Length - arm_Length * 0.001f);

				adyacent = ((topJointTransform_Length * topJointTransform_Length) - (middleJointTransform_Length * middleJointTransform_Length) + (IK_CTRLDistance * IK_CTRLDistance)) / (2 * IK_CTRLDistance);

				angle = Mathf.Acos(adyacent / topJointTransform_Length) * Mathf.Rad2Deg;

				if (!DisableRotateAround)
				{
					topJointTransformIK.RotateAround(topJointTransformIK.position, cross, -angle);
					topJointTransformIK.Rotate(topJointTransform_OffsetRotation2.eulerAngles);
				}

				middleJointTransformIK.LookAt(IK_CTRL, cross);


				middleJointTransformIK.Rotate(middleJointTransform_OffsetRotation.eulerAngles);
				//middleJointTransform.rotate(middleJointTransform_OffsetRotation);

				if (bottomJointTransformMatchesIK_CTRLRotation)
				{
					bottomJointTransformIK.rotation = IK_CTRL.rotation;
					bottomJointTransformIK.Rotate(bottomJointTransform_OffsetRotation);
				}

				if (debug)
				{
					if (middleJointTransformIK != null && poleVector != null)
					{
						Debug.DrawLine(middleJointTransformIK.position, poleVector.position, Color.blue);
					}

					if (topJointTransformIK != null && IK_CTRL != null)
					{
						Debug.DrawLine(topJointTransformIK.position, IK_CTRL.position, Color.red);
					}
				}

			}

			if(topJointTransform != null && middleJointTransform !=null && bottomJointTransform !=null && topJointTransformFK != null && middleJointTransformFK !=null && bottomJointTransformFK !=null)
			{
				topJointTransform.rotation = Quaternion.Lerp(topJointTransformIK.rotation, topJointTransformFK.rotation, IKFK);
				middleJointTransform.rotation = Quaternion.Lerp(middleJointTransformIK.rotation, middleJointTransformFK.rotation, IKFK);
				bottomJointTransform.rotation = Quaternion.Lerp(bottomJointTransformIK.rotation, bottomJointTransformFK.rotation, IKFK);

				topJointTransform.position = Vector3.Lerp(topJointTransformIK.position, topJointTransformFK.position, IKFK);
				middleJointTransform.position = Vector3.Lerp(middleJointTransformIK.position, middleJointTransformFK.position, IKFK);
				bottomJointTransform.position = Vector3.Lerp(bottomJointTransformIK.position, bottomJointTransformFK.position, IKFK);
				foreach (FK fk in fks)
					fk.Transparency = IKFK;
			}
			
			
			
		}
		private bool IsNaN(Quaternion q) 
		{
			
			return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
			
		}




		[HideInInspector]
		public List<Transform> angleLimitTransform = new List<Transform>();
		[HideInInspector]
		public List<Vector2> angleLimits = new List<Vector2>();

		void OnValidate()
		{
			// min & max has to be between 0 ... 360
			for (int i = 0; i < angleLimits.Count; i++) 
			{
				angleLimits [i] = new Vector2 (Mathf.Clamp (angleLimits [i] .x, -360, 360), Mathf.Clamp (angleLimits [i] .y, -360, 360));
			}


		}


		public void CalculateMultiIK()
		{

			if (transform == null || endTransform == null)
				return;

			int i = 0;

			while (i < iterations)
			{
				CalculateMultiIK_run ();
				i++;
			}

			endTransform.rotation = transform.rotation;
		}

		void CalculateMultiIK_run()
		{		
			Transform node = endTransform.parent;

			while (true)
			{
				RotateTowardsIK_CTRL (node);

				if (node == startTransform)
					break;

				node = node.parent;
			}
		}

		void RotateTowardsIK_CTRL(Transform startTransform)
		{		
			if(startTransform ==null)return;

			Vector2 toIK_CTRL = transform.position - startTransform.position;
			Vector2 toEnd = endTransform.position - startTransform.position;

			// Calculate how much we should rotate to get to the IK_CTRL
			float angle = SignedAngle(toEnd, toIK_CTRL);

			// Flip sign if character is turned around

			//angle *= Mathf.Sign(startTransform.root.localScale.x);
			if (startTransform.eulerAngles.y%360>90 && startTransform.eulerAngles.y%360<275 )
				angle *= -1;

			// "Slows" down the IK solving
			angle *= damping; 

			// Wanted angle for rotation
			angle = -(angle - startTransform.localEulerAngles.z);


			if (angleLimits != null && angleLimitTransform.Contains(startTransform))
			{
				//Debug.Log (startTransform);
				// Clamp angle in local space
				Vector2 limit =angleLimits[angleLimitTransform.IndexOf(startTransform)];
				angle = ClampAngle (angle, limit.x, limit.y);
			}
			startTransform.localRotation = Quaternion.Euler(0, 0, angle) ;

		}

		public static float SignedAngle (Vector3 a, Vector3 b)
		{
			float angle = Vector3.Angle (a, b);
			float sign = Mathf.Sign (Vector3.Dot (Vector3.back, Vector3.Cross (a, b)));

			return angle * sign;
		}

		float ClampAngle (float angle, float min, float max)
		{
			//angle = angle % 360;

			return Mathf.Clamp(angle, min, max);
		}
		/*private void OnDrawGizmos()
		{
			if (Active)
			{
				Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
				Gizmos.matrix = rotationMatrix;
				Gizmos.color = Color.green;

				Gizmos.DrawWireCube(Vector3.zero, transform.localScale * HandleSize);
			}
		}*/


	}
}