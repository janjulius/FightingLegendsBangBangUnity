using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet3D
{
	public class CreateControls : Editor
	{

		[MenuItem("GameObject/Puppet3D/Rig/Create IK Control")]
		public static void IKCreateToolMenu()
		{
			IKCreateTool();
		}
		public static void IKCreateTool(bool worldSpace = false, bool IKFK = true, float DefaultSize = 1.5f)
		{

			GameObject bone = Selection.activeObject as GameObject;
			if (bone)
			{
				if (!bone.GetComponent<Bone>())
				{
					
					Debug.LogWarning("This is not a Puppet3D Bone");
					return;
					
				}
				
			}
			else
			{
				Debug.LogWarning("This is not a Puppet3D Bone");
				return;
			}
			GameObject globalCtrl = CreateGlobalControl();
			foreach (ParentControl parentControl in globalCtrl.GetComponent<GlobalControl>()._ParentControls)
			{
				if ((parentControl.bone.transform == bone.transform) || (parentControl.bone.transform == bone.transform.parent.transform))
				{
					Debug.LogWarning("Can't create a IK Control on Bone; it alreay has an Parent Control");
					return;
				}
			}
			

			GameObject IKRoot = null;
			if (bone.transform.parent && bone.transform.parent.transform.parent && bone.transform.parent.transform.parent.GetComponent<Bone>())
				IKRoot = bone.transform.parent.transform.parent.gameObject;
			if (IKRoot == null)
			{
				//Debug.LogWarning("You need to select the end of a chain of three bones");
				IKRoot = bone.transform.parent.gameObject;
				//return;
			}
			// CHECK IF TOP BONE HAS AN IK ATTACHED

			GlobalControl[] globalCtrls = GameObject.FindObjectsOfType<GlobalControl>();
			foreach (GlobalControl glblCtrl in globalCtrls)
			{
				
				foreach (SplineControl splineCtrl in glblCtrl._SplineControls)
				{
					foreach (GameObject splineBone in splineCtrl.bones)
					{
						if (splineBone.transform == bone.transform.parent.transform.parent)
						{
							Debug.LogWarning(bone.transform.parent.transform.parent.name + " has a Spline control attached, please make sure there are at least 3 bones after the spline bone");
							return;
						}
					}
				}
			}
			

			GameObject control = new GameObject();
			Undo.RegisterCreatedObjectUndo(control, "Created control");
			control.name = (bone.name + "_CTRL");
			GameObject controlGroup = new GameObject();
			Undo.RegisterCreatedObjectUndo(controlGroup, "new control grp");
			controlGroup.name = (bone.name + "_CTRL_GRP");

			control.transform.parent = controlGroup.transform;
			controlGroup.transform.position = bone.transform.position;
			if (!worldSpace)
				controlGroup.transform.rotation = bone.transform.rotation;

			GameObject poleVector = new GameObject();
			Undo.RegisterCreatedObjectUndo(poleVector, "Created polevector");
			poleVector.name = (bone.name + "_POLE");
			Pole pv = poleVector.AddComponent<Pole>();
			pv.HandleRadius = DefaultSize;

			GameObject poleVectorParent = new GameObject();
			Undo.RegisterCreatedObjectUndo(poleVectorParent, "Created polevector Parent");
			poleVectorParent.name = (bone.name + "_POLE_GRP");
			poleVector.transform.parent = poleVectorParent.transform;


			float prop = Vector3.Distance(IKRoot.transform.position ,bone.transform.parent.transform.position) / (Vector3.Distance(IKRoot.transform.position, bone.transform.parent.transform.position) + Vector3.Distance(bone.transform.parent.transform.position, bone.transform.position));

			Vector3 dir2elbow = (1f - prop) * IKRoot.transform.position + prop*bone.transform.position;
			poleVectorParent.transform.parent = controlGroup.transform;
			Vector3 dirPole = (bone.transform.parent.position - dir2elbow);
			poleVectorParent.transform.position = bone.transform.parent.position +  10f*dirPole;


			IKControl ikHandle = control.AddComponent<IKControl>();
			ikHandle.poleVector = poleVector.transform;
			IK ikDisplay = control.AddComponent<IK>();
			ikDisplay.HandleSize = DefaultSize;
			// Names

			GameObject TopTransform = IKRoot;
			GameObject MiddleTransform = bone.transform.parent.gameObject;
			GameObject BottomTransform = bone;

			GameObject TopTransformIK = null;			
			GameObject MiddleTransformIK = null;			
			GameObject BottomTransformIK = null;			
			// FK 

			GameObject TopTransformFK = null;			
			GameObject MiddleTransformFK = null;			
			GameObject BottomTransformFK = null;			
			// IK 
			if (IKFK)
			{
				TopTransformIK = new GameObject();
				Undo.RegisterCreatedObjectUndo(TopTransformIK, "Created control");

				IKHidden ikTop = TopTransformIK.AddComponent<IKHidden>();
				ikTop.IKHandle = ikHandle;
				TopTransformIK.name = IKRoot.name + "_IK";
				TopTransformIK.transform.parent = TopTransform.transform.parent;
				TopTransformIK.transform.position = TopTransform.transform.position;
				TopTransformIK.transform.rotation = TopTransform.transform.rotation;
				TopTransformIK.transform.localScale = TopTransform.transform.localScale;

				MiddleTransformIK = new GameObject();
				Undo.RegisterCreatedObjectUndo(MiddleTransformIK, "Created control");

				IKHidden ikMid = MiddleTransformIK.AddComponent<IKHidden>();
				ikMid.IKHandle = ikHandle;
				MiddleTransformIK.name = MiddleTransform.name + "_IK";
				MiddleTransformIK.transform.parent = TopTransformIK.transform;
				MiddleTransformIK.transform.position = MiddleTransform.transform.position;
				MiddleTransformIK.transform.rotation = MiddleTransform.transform.rotation;
				MiddleTransformIK.transform.localScale = MiddleTransform.transform.localScale;

				BottomTransformIK = new GameObject();
				Undo.RegisterCreatedObjectUndo(BottomTransformIK, "Created control");

				BottomTransformIK.name = bone.name + "_IK";

				IKHidden ikBot = BottomTransformIK.AddComponent<IKHidden>();
				ikBot.IKHandle = ikHandle;
				BottomTransformIK.transform.parent = MiddleTransformIK.transform;
				BottomTransformIK.transform.position = BottomTransform.transform.position;
				BottomTransformIK.transform.rotation = BottomTransform.transform.rotation;
				BottomTransformIK.transform.localScale = BottomTransform.transform.localScale;

				// FK 

				TopTransformFK = new GameObject();
				Undo.RegisterCreatedObjectUndo(TopTransformFK, "Created control");

				ikHandle.fks[0] = TopTransformFK.AddComponent<FK>();
				ikHandle.fks[0].IKHandle = ikHandle;
				ikHandle.fks[0].HandleSize = DefaultSize*.9f;
				TopTransformFK.name = IKRoot.name + "_FK";
				TopTransformFK.transform.parent = TopTransform.transform.parent;
				TopTransformFK.transform.position = TopTransform.transform.position;
				TopTransformFK.transform.rotation = TopTransform.transform.rotation;
				TopTransformFK.transform.localScale = TopTransform.transform.localScale;

				MiddleTransformFK = new GameObject();
				Undo.RegisterCreatedObjectUndo(MiddleTransformFK, "Created control");

				ikHandle.fks[1] = MiddleTransformFK.AddComponent<FK>();
				ikHandle.fks[1].IKHandle = ikHandle;
				ikHandle.fks[1].HandleSize = DefaultSize * .9f;
				MiddleTransformFK.name = bone.transform.parent.name + "_FK";
				MiddleTransformFK.transform.parent = TopTransformFK.transform;
				MiddleTransformFK.transform.position = MiddleTransform.transform.position;
				MiddleTransformFK.transform.rotation = MiddleTransform.transform.rotation;
				MiddleTransformFK.transform.localScale = MiddleTransform.transform.localScale;

				BottomTransformFK = new GameObject();
				Undo.RegisterCreatedObjectUndo(BottomTransformFK, "Created control");
								
				ikHandle.fks[2] = BottomTransformFK.AddComponent<FK>();
				ikHandle.fks[2].IKHandle = ikHandle;
				ikHandle.fks[2].HandleSize = DefaultSize * .9f;
				BottomTransformFK.name = bone.name + "_FK";
				BottomTransformFK.transform.parent = MiddleTransformFK.transform;
				BottomTransformFK.transform.position = BottomTransform.transform.position;
				BottomTransformFK.transform.rotation = BottomTransform.transform.rotation;
				BottomTransformFK.transform.localScale = BottomTransform.transform.localScale;
			}
			// store middle bone position to check if it needs flipping

			//Vector3 middleBonePos = bone.transform.parent.transform.position;

			Quaternion topRotBefore = IKRoot.transform.rotation;



			ikHandle.IK_CTRL = control.transform;
			if (IKFK)
			{
				ikHandle.topJointTransformIK = TopTransformIK.transform;
				ikHandle.middleJointTransformIK = MiddleTransformIK.transform;
				ikHandle.bottomJointTransformIK = BottomTransformIK.transform;
				ikHandle.topJointTransformFK = TopTransformFK.transform;
				ikHandle.middleJointTransformFK = MiddleTransformFK.transform;
				ikHandle.bottomJointTransformFK = BottomTransformFK.transform;
				ikHandle.topJointTransform = IKRoot.transform;
				ikHandle.middleJointTransform = bone.transform.parent.transform;
				ikHandle.bottomJointTransform = bone.transform;
			}
			else
			{
				ikHandle.topJointTransformIK = TopTransform.transform;
				ikHandle.middleJointTransformIK = MiddleTransform.transform;
				ikHandle.bottomJointTransformIK = BottomTransform.transform;

			}
			

			ikHandle.scaleStart[0] = IKRoot.transform.localScale;
			ikHandle.scaleStart[1] = IKRoot.transform.GetChild(0).localScale;
			ikHandle.OffsetScale = bone.transform.localScale;

			
			if (worldSpace)
				ikHandle.Offset = ikHandle.bottomJointTransform.rotation;

			if (bone.GetComponent<Bone>())
			{
				ikHandle.AimDirection = Vector3.forward;
				ikHandle.UpDirection = Vector3.right;
			}
			else
			{
				Debug.LogWarning("This is not a Puppet3D Bone");
				ikHandle.AimDirection = Vector3.right;
				ikHandle.UpDirection = Vector3.up;
			}
			Quaternion middleRotBefore = MiddleTransform.transform.rotation;
			ikHandle.DisableRotateAround = true;

			ikHandle.CalculateIK();

			Quaternion topRotAfter = TopTransform.transform.rotation;

			Quaternion topOffset = (Quaternion.Inverse(topRotAfter) * topRotBefore);
			
			ikHandle.topJointTransform_OffsetRotation = topOffset;

			ikHandle.DisableRotateAround = false;

			ikHandle.CalculateIK();

			Quaternion topRotAfter2 = TopTransform.transform.rotation;
			Quaternion topOffset2 = (Quaternion.Inverse(topRotAfter2) * topRotBefore);
			ikHandle.topJointTransform_OffsetRotation2 = topOffset2;

			ikHandle.CalculateIK();

			Quaternion middleRotAfter = MiddleTransform.transform.rotation;
			//Debug.Log("rot " + middleRotAfter);

			Quaternion middleOffset = (Quaternion.Inverse(middleRotAfter) * middleRotBefore);

			ikHandle.middleJointTransform_OffsetRotation = middleOffset;


			
			//if (bone.transform.parent.transform.position.x < IKRoot.transform.position.x)

			Selection.activeObject = ikHandle;

			controlGroup.transform.parent = globalCtrl.transform;
			//poleVector.transform.parent = globalCtrl.transform;
			if (globalCtrl.GetComponent<GlobalControl>().AutoRefresh)
				globalCtrl.GetComponent<GlobalControl>().Init();
			else
				globalCtrl.GetComponent<GlobalControl>()._Ikhandles.Add(ikHandle);


			//fix from now on for 180 flip
			globalCtrl.GetComponent<GlobalControl>()._flipCorrection = -1;
			globalCtrl.GetComponent<GlobalControl>().Run();


		}
		[MenuItem("GameObject/Puppet3D/Rig/Create Parent Control")]
		public static void CreateParentControl(float DefaultScale = 1f)
		{
			GameObject bone = Selection.activeObject as GameObject;
			if (bone)
			{
				if (!bone.GetComponent<Bone>() && !bone.GetComponent<IKHidden>() && !bone.GetComponent<FK>())
				{

					Debug.LogWarning("This is not a Puppet3D Bone");
					return;

				}

			}
			else
			{
				Debug.LogWarning("This is not a Puppet3D Bone");
				return;
			}
			GameObject globalCtrl = CreateGlobalControl();
			foreach (IKControl ikhandle in globalCtrl.GetComponent<GlobalControl>()._Ikhandles)
			{
				if ((ikhandle.bottomJointTransform == bone.transform) || (ikhandle.middleJointTransform == bone.transform))
				{
					Debug.LogWarning("Can't create a parent Control on Bone; it alreay has an IK handle");
					return;
				}
			}
			foreach (ParentControl parentControl in globalCtrl.GetComponent<GlobalControl>()._ParentControls)
			{
				if ((parentControl.bone.transform == bone.transform))
				{
					Debug.LogWarning("Can't create a Parent Control on Bone; it alreay has an Parent Control");
					return;
				}
			}
			foreach (SplineControl splineCtrl in globalCtrl.GetComponent<GlobalControl>()._SplineControls)
			{
				foreach (GameObject splineBone in splineCtrl.bones)
				{
					if (splineBone.transform == bone.transform)
					{
						Debug.LogWarning(bone.transform.parent.transform.parent.name + " has a Spline control attached");
						return;
					}
				}
			}
			GameObject control = new GameObject();
			Undo.RegisterCreatedObjectUndo(control, "Created control");
			control.name = (bone.name + "_CTRL");
			GameObject controlGroup = new GameObject();
			Undo.RegisterCreatedObjectUndo(controlGroup, "CreatedControlGrp");
			controlGroup.name = (bone.name + "_CTRL_GRP");
			control.transform.parent = controlGroup.transform;
			controlGroup.transform.position = bone.transform.position;
			controlGroup.transform.rotation = bone.transform.rotation;

			
			ParentControl parentConstraint = control.AddComponent<ParentControl>();

			parentConstraint.Orient = true;
			parentConstraint.Point = true;
			parentConstraint.bone = bone;
			parentConstraint.OffsetScale = bone.transform.localScale;
			parentConstraint.HandleScale = Vector3.one*DefaultScale;

			Selection.activeObject = control;


			controlGroup.transform.parent = globalCtrl.transform;

			if (globalCtrl.GetComponent<GlobalControl>().AutoRefresh)
				globalCtrl.GetComponent<GlobalControl>().Init();
			else
				globalCtrl.GetComponent<GlobalControl>()._ParentControls.Add(parentConstraint);


		}
		public static GameObject CreateGlobalControl()
		{
			GlobalControl globalCtrl = GameObject.FindObjectOfType<GlobalControl>();

			if (globalCtrl)
			{
				return globalCtrl.gameObject;
			}
			else
			{
				GameObject globalCtrlGO = new GameObject("Global_CTRL");
				Undo.RegisterCreatedObjectUndo(globalCtrlGO, "Created globalCTRL");

				globalCtrlGO.AddComponent<GlobalControl>();

				return globalCtrlGO;
			}

		}
		[MenuItem("GameObject/Puppet3D/Rig/Create Orient Control")]
		public static void CreateOrientControl(float DefaultScale = 1f)
		{
			GameObject bone = Selection.activeObject as GameObject;
			if (bone)
			{
				if (!bone.GetComponent<Bone>())
				{

					Debug.LogWarning("This is not a Puppet3D Bone");
					return;

				}

			}
			else
			{
				Debug.LogWarning("This is not a Puppet3D Bone");
				return;
			}
			GameObject globalCtrl = CreateGlobalControl();
			foreach (IKControl ikhandle in globalCtrl.GetComponent<GlobalControl>()._Ikhandles)
			{
				if ((ikhandle.bottomJointTransform == bone.transform) || (ikhandle.middleJointTransform == bone.transform))
				{
					Debug.LogWarning("Can't create a orient Control on Bone; it alreay has an IK handle");
					return;
				}
			}
			foreach (ParentControl parentControl in globalCtrl.GetComponent<GlobalControl>()._ParentControls)
			{
				if ((parentControl.bone.transform == bone.transform))
				{
					Debug.LogWarning("Can't create a Parent Control on Bone; it alreay has an Parent Control");
					return;
				}
			}
			foreach (SplineControl splineCtrl in globalCtrl.GetComponent<GlobalControl>()._SplineControls)
			{
				foreach (GameObject splineBone in splineCtrl.bones)
				{
					if (splineBone.transform == bone.transform)
					{
						Debug.LogWarning(bone.transform.parent.transform.parent.name + " has a Spline control attached");
						return;
					}
				}
			}

			GameObject control = new GameObject();
			Undo.RegisterCreatedObjectUndo(control, "Created control");
			control.name = (bone.name + "_CTRL");
			GameObject controlGroup = new GameObject();
			Undo.RegisterCreatedObjectUndo(controlGroup, "Created controlGroup");
			controlGroup.name = (bone.name + "_CTRL_GRP");
			control.transform.parent = controlGroup.transform;
			controlGroup.transform.position = bone.transform.position;
			controlGroup.transform.rotation = bone.transform.rotation;
			
			ParentControl parentConstraint = control.AddComponent<ParentControl>();
			parentConstraint.Orient = true;
			parentConstraint.Point = false;
			parentConstraint.bone = bone;
			parentConstraint.HandleScale =Vector3.one* DefaultScale;

			Selection.activeObject = control;
			parentConstraint.OffsetScale = bone.transform.localScale;

			controlGroup.transform.parent = globalCtrl.transform;

			if (globalCtrl.GetComponent<GlobalControl>().AutoRefresh)
				globalCtrl.GetComponent<GlobalControl>().Init();
			else
				globalCtrl.GetComponent<GlobalControl>()._ParentControls.Add(parentConstraint);
		}
		public static void CreateAvatar()
		{
			GameObject go = Selection.activeGameObject;

			if (go != null && go.GetComponent("Animator") != null)
			{
				HumanDescription hd = new HumanDescription();

				Dictionary<string, string> boneName = new System.Collections.Generic.Dictionary<string, string>();
				boneName["Chest"] = "spine";
				boneName["Head"] = "head";
				boneName["Hips"] = "hip";
				boneName["LeftFoot"] = "footL";
				boneName["LeftHand"] = "handL";
				boneName["LeftLowerArm"] = "elbowL";
				boneName["LeftLowerLeg"] = "kneeL";
				boneName["LeftShoulder"] = "clavL";
				boneName["LeftUpperArm"] = "armL";
				boneName["LeftUpperLeg"] = "legL";
				boneName["RightFoot"] = "footR";
				boneName["RightHand"] = "handR";
				boneName["RightLowerArm"] = "elbowR";
				boneName["RightLowerLeg"] = "kneeR";
				boneName["RightShoulder"] = "clavR";
				boneName["RightUpperArm"] = "armR";
				boneName["RightUpperLeg"] = "legR";
				boneName["Spine"] = "spine2";
				string[] humanName = HumanTrait.BoneName;
				HumanBone[] humanBones = new HumanBone[boneName.Count];
				int j = 0;
				int i = 0;
				while (i < humanName.Length)
				{
					if (boneName.ContainsKey(humanName[i]))
					{
						HumanBone humanBone = new HumanBone();
						humanBone.humanName = humanName[i];
						humanBone.boneName = boneName[humanName[i]];
						humanBone.limit.useDefaultValues = true;
						humanBones[j++] = humanBone;
					}
					i++;
				}

				hd.human = humanBones;

				//hd.skeleton = new SkeletonBone[18];
				//hd.skeleton[0].name = ("Hips") ;
				Avatar avatar = AvatarBuilder.BuildHumanAvatar(go, hd);

				avatar.name = (go.name + "_Avatar");
				Debug.Log(avatar.isHuman ? "is human" : "is generic");

				Animator animator = go.GetComponent("Animator") as Animator;
				animator.avatar = avatar;

				string path = AssetDatabase.GenerateUniqueAssetPath(Puppet3DEditor._puppet3DPath + "/Animation/" + avatar.name + ".asset");
				AssetDatabase.CreateAsset(avatar, path);

			}

		}
	}
}
