using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
namespace Puppet3D
{
	[ExecuteInEditMode]
	[CustomEditor(typeof(IKControl))]
	public class IKHandleEditor : Editor
	{
		public string[] IkType = { "Basic 3 Bone", "Multi Bone" };


		public void OnEnable()
		{
			IKControl myTarget = (IKControl)target;

			myTarget.endTransform = myTarget.bottomJointTransform;



		}

		

		public void resetIK(IKControl myTarget)
		{
			myTarget.enabled = false;
			//myTarget.transform.localPosition = Vector3.zero;
			for (int i = 0; i < 100; i++)
			{
				for (int j = 0; j < myTarget.bindBones.Count; j++)
				{
					myTarget.bindBones[j].localRotation = Quaternion.Euler(myTarget.bindPose[j]);

				}
			}
			myTarget.enabled = true;

		}
		public void setEndBone(IKControl myTarget)
		{
			myTarget.angleLimits.Clear();
			myTarget.angleLimitTransform.Clear();



			if (myTarget.numberOfBones < 2)
				myTarget.numberOfBones = 2;

			GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<GlobalControl>();


			myTarget.endTransform = myTarget.bottomJointTransform;

			myTarget.startTransform = myTarget.endTransform;

			bool unlockedBone = true;

			for (int i = 0; i < myTarget.numberOfBones - 1; i++)
			{


				if (myTarget.startTransform.parent != null)
				{
					for (int j = 0; j < globalCtrlScripts.Length; j++)
					{
						if (myTarget.startTransform.parent.GetComponent<GlobalControl>())
						{
							myTarget.numberOfBones = i + 1;
							unlockedBone = false;
						}

						foreach (ParentControl ParentControl in globalCtrlScripts[j]._ParentControls)
						{
							if (ParentControl.bone.transform == myTarget.startTransform.parent)
							{
								myTarget.numberOfBones = i + 1;
								unlockedBone = false;
							}
						}
						foreach (SplineControl splineCtrl in globalCtrlScripts[j]._SplineControls)
						{
							foreach (GameObject bone in splineCtrl.bones)
							{
								if (bone.transform == myTarget.startTransform.parent)
								{
									myTarget.numberOfBones = i + 1;
									unlockedBone = false;

								}
							}
						}
					}
					if (unlockedBone)
					{


						if (myTarget.startTransform != myTarget.endTransform && myTarget.limitBones)
						{
							Vector2 limit = new Vector2();
							Transform limitTransform = myTarget.startTransform;

							Vector3 newEulerAngle = new Vector3(limitTransform.localEulerAngles.x % 360,
																 limitTransform.localEulerAngles.y % 360,
																 limitTransform.localEulerAngles.z % 360);

							if (newEulerAngle.x < 0)
								newEulerAngle.x += 360;
							if (newEulerAngle.y < 0)
								newEulerAngle.y += 360;
							if (newEulerAngle.z < 0)
								newEulerAngle.z += 360;
							myTarget.startTransform.localEulerAngles = newEulerAngle;

							float rangedVal = limitTransform.localEulerAngles.z % 360;
							if (rangedVal > 0 && rangedVal < 180)
							{
								limit = new Vector2(0, 180);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}
							else if (rangedVal > 180 && rangedVal < 360)
							{
								limit = new Vector2(180, 360);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}
							else if (rangedVal > -180 && rangedVal < 0)
							{

								limit = new Vector2(-180, 0);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}
							else if (rangedVal > -360 && rangedVal < -180)
							{
								limit = new Vector2(-360, -180);
								myTarget.angleLimits.Add(limit);
								myTarget.angleLimitTransform.Add(limitTransform);

							}



						}
						myTarget.startTransform = myTarget.startTransform.parent;
					}


				}
				else
					myTarget.numberOfBones = i + 1;



			}

		}
		

	}
}
