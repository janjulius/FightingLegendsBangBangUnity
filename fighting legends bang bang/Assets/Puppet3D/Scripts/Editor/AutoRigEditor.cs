using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
namespace Puppet3D
{
	public class AutoRigEditor : Editor
	{

		static private string _puppet3DPath;

		[MenuItem("GameObject/Puppet3D/MakeGuides")]
		public static bool MakeGuides(bool rigFingers = false)
		{
			GameObject[] gos = Selection.gameObjects;
			if (gos.Length == 0)
				return false;
			if(FindObjectOfType<Guides>() !=null)
				return false;

			GameObject go = gos[0];


			Bounds bounds = new Bounds();

			foreach (GameObject goer in gos)
			{
				if (goer.GetComponent<MeshRenderer>() == null && goer.GetComponent<SkinnedMeshRenderer>() == null)
					return false;
				if (goer.GetComponent<MeshCollider>() == null)
					goer.AddComponent<MeshCollider>();
				if (goer.GetComponent<Renderer>() != null)
					bounds.Encapsulate(goer.GetComponent<Renderer>().bounds);
			}

			RecursivelyFindFolderPath("Assets");
			string path = (_puppet3DPath + "/Prefabs/Guides.prefab");
			GameObject guides = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			GameObject guideGO = Instantiate(guides) as GameObject;
			Undo.RegisterCreatedObjectUndo(guideGO, "Create guide object");
			guideGO.name = (go.name + " _GUIDES");
			Guides guideComp = guideGO.AddComponent<Guides>();

			guideComp.Biped = gos;
			guideComp.Bounds = bounds;
			guideGO.transform.localScale = new Vector3(bounds.size.y / 200f, bounds.size.y / 200f, bounds.size.y / 200f);

			guideGO.transform.position = new Vector3((bounds.max.x + bounds.min.x) / 2f, bounds.min.y, 0f);

			Guide[] allGuides = guideGO.transform.GetComponentsInChildren<Guide>();
			foreach (Guide guide in allGuides)
			{
				guide.Radius = bounds.size.y / 100f;
				if(guide.name.Contains("head") || guide.name.Contains("index") || guide.name.Contains("middle") || guide.name.Contains("ring") || guide.name.Contains("little") || guide.name.Contains("thumb"))
					guide.Radius = bounds.size.y / 200f;

			}
			if (!rigFingers)
			{
				DestroyImmediate(GameObject.Find("indexL1_guide"));
				DestroyImmediate(GameObject.Find("indexR1_guide"));
				DestroyImmediate(GameObject.Find("middleL1_guide"));
				DestroyImmediate(GameObject.Find("middleR1_guide"));
				DestroyImmediate(GameObject.Find("ringL1_guide"));
				DestroyImmediate(GameObject.Find("ringR1_guide"));
				DestroyImmediate(GameObject.Find("littleL1_guide"));
				DestroyImmediate(GameObject.Find("littleR1_guide"));
				DestroyImmediate(GameObject.Find("thumbL1_guide"));
				DestroyImmediate(GameObject.Find("thumbR1_guide"));

			}
			return true;
		}
		public static Bounds GetBounds(GameObject go)
		{
			Sprite spr = go.GetComponent<SpriteRenderer>().sprite;
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spr)) as TextureImporter;
			SpriteMetaData[] smdArray = textureImporter.spritesheet;
			Bounds newBounds = go.GetComponent<SpriteRenderer>().bounds;
			for (int k = 0; k < smdArray.Length; k++)
			{
				if (smdArray[k].name == spr.name)
				{

					float XProp = (smdArray[k].rect.center.x / spr.texture.width) - .5f;
					float YProp = (smdArray[k].rect.center.y / spr.texture.height) - .5f;
					float XScaleProp = smdArray[k].rect.width / spr.texture.width;
					float YScaleProp = smdArray[k].rect.height / spr.texture.height;
					Vector3 newSize = new Vector3(newBounds.size.x / XScaleProp, newBounds.size.y / YScaleProp, newBounds.size.z);
					newBounds.size = newSize;
					newBounds.center -= new Vector3(XProp * newSize.x, YProp * newSize.y, 0f);

				}
			}
			//        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//        cube.transform.position = newBounds.min;
			//        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//        cube.transform.position = newBounds.min + newBounds.size;
			return newBounds;
		}
		private static void RecursivelyFindFolderPath(string dir)
		{
			string[] paths = Directory.GetDirectories(dir);
			foreach (string s in paths)
			{
				if (s.Contains("Puppet3D"))
				{
					_puppet3DPath = s;
					break;
				}
				else
				{
					RecursivelyFindFolderPath(s);
				}
			}
		}
		public static void CreateModRig(bool rigFingers = false)
		{
			Animator anim = null;
			if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Animator>())
			{
				anim = Selection.activeGameObject.GetComponent<Animator>();
				if (anim != null && anim.runtimeAnimatorController == null)
					anim.runtimeAnimatorController = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(Puppet3DEditor._puppet3DPath + "/Animation/AutoRig/P3D_AnimatorController.controller", typeof(UnityEditor.Animations.AnimatorController));

			}
			else
			{
				Debug.LogWarning("Needs to have an animator with an Avatar with bones defined");
				return;
			}
			RecursivelyFindFolderPath("Assets");
			string path = (_puppet3DPath + "/Prefabs/Guides.prefab");
			GameObject guidesGO = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			GameObject guideGO = Instantiate(guidesGO) as GameObject;
			Undo.RegisterCreatedObjectUndo(guideGO, "Create guide object");
			Guides guideComp = guideGO.AddComponent<Guides>();
			Bounds bounds = new Bounds();
			SkinnedMeshRenderer[] smrs = anim.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
			guideComp.Biped = new GameObject[1];
			foreach (SkinnedMeshRenderer smr in smrs)
			{
				guideComp.Biped[0] = smr.gameObject;
				bounds.Encapsulate(smr.bounds);
			}
			guideComp.Bounds = bounds;

			if (!rigFingers)
			{
				DestroyImmediate(GameObject.Find("indexL1_guide"));
				DestroyImmediate(GameObject.Find("indexR1_guide"));
				DestroyImmediate(GameObject.Find("middleL1_guide"));
				DestroyImmediate(GameObject.Find("middleR1_guide"));
				DestroyImmediate(GameObject.Find("ringL1_guide"));
				DestroyImmediate(GameObject.Find("ringR1_guide"));
				DestroyImmediate(GameObject.Find("littleL1_guide"));
				DestroyImmediate(GameObject.Find("littleR1_guide"));
				DestroyImmediate(GameObject.Find("thumbL1_guide"));
				DestroyImmediate(GameObject.Find("thumbR1_guide"));

			}

			Transform hips = anim.GetBoneTransform(HumanBodyBones.Hips);
			if (hips == null) { Debug.LogWarning("Needs an Avatar with Hips bone"); return; }
			Transform Chest = anim.GetBoneTransform(HumanBodyBones.Chest);
			if (Chest == null) { Debug.LogWarning("Needs an Avatar with Chest bone"); return; }
			Transform Spine = anim.GetBoneTransform(HumanBodyBones.Spine);
			if (Spine == null) { Debug.LogWarning("Needs an Avatar with Spine bone"); return; }
			Transform ClavL = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
			if (ClavL == null) { Debug.LogWarning("Needs an Avatar with ClavL bone");  }
			Transform ClavR = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
			if (ClavR == null) { Debug.LogWarning("Needs an Avatar with ClavR bone");  }
			Transform LeftUpperLeg = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			if (LeftUpperLeg == null) { Debug.LogWarning("Needs an Avatar with LeftUpperLeg bone"); return; }
			Transform RightUpperLeg = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			if (RightUpperLeg == null) { Debug.LogWarning("Needs an Avatar with RightUpperLeg bone"); return; }
			Transform LeftUpperArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			if (LeftUpperArm == null) { Debug.LogWarning("Needs an Avatar with LeftUpperArm bone"); return; }
			Transform RightUpperArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
			if (RightUpperArm == null) { Debug.LogWarning("Needs an Avatar with RightUpperArm bone"); return; }
			Transform LeftHand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
			if (LeftHand == null) { Debug.LogWarning("Needs an Avatar with LeftHand bone"); return; }
			Transform RightHand = anim.GetBoneTransform(HumanBodyBones.RightHand);
			if (RightHand == null) { Debug.LogWarning("Needs an Avatar with RightHand bone"); return; }
			Transform LeftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
			if (LeftFoot == null) { Debug.LogWarning("Needs an Avatar with LeftFoot bone"); return; }
			Transform RightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
			if (RightFoot == null) { Debug.LogWarning("Needs an Avatar with RightFoot bone"); return; }
			Transform Head = anim.GetBoneTransform(HumanBodyBones.Head);
			if (Head == null) { Debug.LogWarning("Needs an Avatar with Head bone"); return; }
			Transform Neck = anim.GetBoneTransform(HumanBodyBones.Neck);
			if (Neck == null) { Debug.LogWarning("Needs an Avatar with Neck bone"); return; }
			Transform LeftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
			if (LeftLowerArm == null) { Debug.LogWarning("Needs an Avatar with LeftLowerArm bone"); return; }
			Transform RightLowerArm = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
			if (RightLowerArm == null) { Debug.LogWarning("Needs an Avatar with RightLowerArm bone"); return; }
			Transform LeftLowerLeg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			if (LeftLowerLeg == null) { Debug.LogWarning("Needs an Avatar with LeftLowerLeg bone"); return; }
			Transform RightLowerLeg = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
			if (RightLowerLeg == null) { Debug.LogWarning("Needs an Avatar with RightLowerLeg bone"); return; }

			Transform LeftIndexDistal = anim.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
			Transform LeftIndexIntermediate = anim.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
			Transform LeftIndexProximal = anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
			Transform LeftMiddleDistal = anim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
			Transform LeftMiddleIntermediate = anim.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
			Transform LeftMiddleProximal = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
			Transform LeftRingDistal = anim.GetBoneTransform(HumanBodyBones.LeftRingDistal);
			Transform LeftRingIntermediate = anim.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
			Transform LeftRingProximal = anim.GetBoneTransform(HumanBodyBones.LeftRingProximal);
			Transform LeftLittleDistal = anim.GetBoneTransform(HumanBodyBones.LeftLittleDistal);
			Transform LeftLittleIntermediate = anim.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
			Transform LeftLittleProximal = anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
			Transform LeftThumbDistal = anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
			Transform LeftThumbIntermediate = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
			Transform LeftThumbProximal = anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);


			Transform RightIndexDistal = anim.GetBoneTransform(HumanBodyBones.RightIndexDistal);
			Transform RightIndexIntermediate = anim.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
			Transform RightIndexProximal = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
			Transform RightMiddleDistal = anim.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
			Transform RightMiddleIntermediate = anim.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
			Transform RightMiddleProximal = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
			Transform RightRingDistal = anim.GetBoneTransform(HumanBodyBones.RightRingDistal);
			Transform RightRingIntermediate = anim.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
			Transform RightRingProximal = anim.GetBoneTransform(HumanBodyBones.RightRingProximal);
			Transform RightLittleDistal = anim.GetBoneTransform(HumanBodyBones.RightLittleDistal);
			Transform RightLittleIntermediate = anim.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
			Transform RightLittleProximal = anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
			Transform RightThumbDistal = anim.GetBoneTransform(HumanBodyBones.RightThumbDistal);
			Transform RightThumbIntermediate = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
			Transform RightThumbProximal = anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);


			guideGO.transform.Find("hip_guide").GetComponent<Guide>().HandlePos = hips.position;
			guideGO.transform.Find("chest_guide").GetComponent<Guide>().HandlePos = Chest.position;
			guideGO.transform.Find("thighL_guide").GetComponent<Guide>().HandlePos = LeftUpperLeg.position;
			guideGO.transform.Find("thighR_guide").GetComponent<Guide>().HandlePos = RightUpperLeg.position;
			guideGO.transform.Find("armL_guide").GetComponent<Guide>().HandlePos = LeftUpperArm.position;
			guideGO.transform.Find("armR_guide").GetComponent<Guide>().HandlePos = RightUpperArm.position;
			guideGO.transform.Find("handL_guide").GetComponent<Guide>().HandlePos = LeftHand.position;
			guideGO.transform.Find("handR_guide").GetComponent<Guide>().HandlePos = RightHand.position;
			guideGO.transform.Find("footL_guide").GetComponent<Guide>().HandlePos = LeftFoot.position;
			guideGO.transform.Find("footR_guide").GetComponent<Guide>().HandlePos = RightFoot.position;
			guideGO.transform.Find("neck_guide").GetComponent<Guide>().HandlePos = Neck.position;
			guideGO.transform.Find("neck_guide/head_guide").GetComponent<Guide>().HandlePos = Head.position;
			guideGO.transform.Find("elbowL_guide").GetComponent<Guide>().HandlePos = LeftLowerArm.position;
			guideGO.transform.Find("elbowR_guide").GetComponent<Guide>().HandlePos = RightLowerArm.position;
			guideGO.transform.Find("kneeL_guide").GetComponent<Guide>().HandlePos = LeftLowerLeg.position;
			guideGO.transform.Find("kneeR_guide").GetComponent<Guide>().HandlePos = RightLowerLeg.position;
			if (rigFingers)
			{
				if (LeftIndexProximal)
					guideGO.transform.Find("handL_guide/indexL1_guide").GetComponent<Guide>().HandlePos = LeftIndexProximal.position;
				if (LeftIndexIntermediate)
					guideGO.transform.Find("handL_guide/indexL1_guide/indexL2_guide").GetComponent<Guide>().HandlePos = LeftIndexIntermediate.position;
				if (LeftIndexDistal)
					guideGO.transform.Find("handL_guide/indexL1_guide/indexL2_guide/indexL3_guide").GetComponent<Guide>().HandlePos = LeftIndexDistal.position;
				if (LeftMiddleProximal)
					guideGO.transform.Find("handL_guide/middleL1_guide").GetComponent<Guide>().HandlePos = LeftMiddleProximal.position;
				if (LeftMiddleIntermediate)
					guideGO.transform.Find("handL_guide/middleL1_guide/middleL2_guide").GetComponent<Guide>().HandlePos = LeftMiddleIntermediate.position;
				if (LeftMiddleDistal)
					guideGO.transform.Find("handL_guide/middleL1_guide/middleL2_guide/middleL3_guide").GetComponent<Guide>().HandlePos = LeftMiddleDistal.position;

				if (LeftRingProximal)
					guideGO.transform.Find("handL_guide/ringL1_guide").GetComponent<Guide>().HandlePos = LeftRingProximal.position;
				if (LeftRingIntermediate)
					guideGO.transform.Find("handL_guide/ringL1_guide/ringL2_guide").GetComponent<Guide>().HandlePos = LeftRingIntermediate.position;
				if (LeftRingDistal)
					guideGO.transform.Find("handL_guide/ringL1_guide/ringL2_guide/ringL3_guide").GetComponent<Guide>().HandlePos = LeftRingDistal.position;

				if (LeftLittleProximal)
					guideGO.transform.Find("handL_guide/littleL1_guide").GetComponent<Guide>().HandlePos = LeftLittleProximal.position;
				if (LeftLittleIntermediate)
					guideGO.transform.Find("handL_guide/littleL1_guide/littleL2_guide").GetComponent<Guide>().HandlePos = LeftLittleIntermediate.position;
				if (LeftLittleDistal)
					guideGO.transform.Find("handL_guide/littleL1_guide/littleL2_guide/littleL3_guide").GetComponent<Guide>().HandlePos = LeftLittleDistal.position;

				if (LeftThumbProximal)
					guideGO.transform.Find("handL_guide/thumbL1_guide").GetComponent<Guide>().HandlePos = LeftThumbProximal.position;
				if (LeftThumbIntermediate)
					guideGO.transform.Find("handL_guide/thumbL1_guide/thumbL2_guide").GetComponent<Guide>().HandlePos = LeftThumbIntermediate.position;
				if (LeftThumbDistal)
					guideGO.transform.Find("handL_guide/thumbL1_guide/thumbL2_guide/thumbL3_guide").GetComponent<Guide>().HandlePos = LeftThumbDistal.position;


				if (RightIndexProximal)
					guideGO.transform.Find("handR_guide/indexR1_guide").GetComponent<Guide>().HandlePos = RightIndexProximal.position;
				if (RightIndexIntermediate)
					guideGO.transform.Find("handR_guide/indexR1_guide/indexR2_guide").GetComponent<Guide>().HandlePos = RightIndexIntermediate.position;
				if (RightIndexDistal)
					guideGO.transform.Find("handR_guide/indexR1_guide/indexR2_guide/indexR3_guide").GetComponent<Guide>().HandlePos = RightIndexDistal.position;

				if (RightMiddleProximal)
					guideGO.transform.Find("handR_guide/middleR1_guide").GetComponent<Guide>().HandlePos = RightMiddleProximal.position;
				if (RightMiddleIntermediate)
					guideGO.transform.Find("handR_guide/middleR1_guide/middleR2_guide").GetComponent<Guide>().HandlePos = RightMiddleIntermediate.position;
				if (RightMiddleDistal)
					guideGO.transform.Find("handR_guide/middleR1_guide/middleR2_guide/middleR3_guide").GetComponent<Guide>().HandlePos = RightMiddleDistal.position;

				if (RightRingProximal)
					guideGO.transform.Find("handR_guide/ringR1_guide").GetComponent<Guide>().HandlePos = RightRingProximal.position;
				if (RightRingIntermediate)
					guideGO.transform.Find("handR_guide/ringR1_guide/ringR2_guide").GetComponent<Guide>().HandlePos = RightRingIntermediate.position;
				if (RightRingDistal)
					guideGO.transform.Find("handR_guide/ringR1_guide/ringR2_guide/ringR3_guide").GetComponent<Guide>().HandlePos = RightRingDistal.position;

				if (RightLittleProximal)
					guideGO.transform.Find("handR_guide/littleR1_guide").GetComponent<Guide>().HandlePos = RightLittleProximal.position;
				if (RightLittleIntermediate)
					guideGO.transform.Find("handR_guide/littleR1_guide/littleR2_guide").GetComponent<Guide>().HandlePos = RightLittleIntermediate.position;
				if (RightLittleDistal)
					guideGO.transform.Find("handR_guide/littleR1_guide/littleR2_guide/littleR3_guide").GetComponent<Guide>().HandlePos = RightLittleDistal.position;

				if (RightThumbProximal)
					guideGO.transform.Find("handR_guide/thumbR1_guide").GetComponent<Guide>().HandlePos = RightThumbProximal.position;
				if (RightThumbIntermediate)
					guideGO.transform.Find("handR_guide/thumbR1_guide/thumbR2_guide").GetComponent<Guide>().HandlePos = RightThumbIntermediate.position;
				if (RightThumbDistal)
					guideGO.transform.Find("handR_guide/thumbR1_guide/thumbR2_guide/thumbR3_guide").GetComponent<Guide>().HandlePos = RightThumbDistal.position;
			}
			GlobalControl globalControl = anim.gameObject.AddComponent<GlobalControl>();
			AutoRig(globalControl);

			string HipName = anim.GetBoneTransform(HumanBodyBones.Hips).name;

			GameObject HipFK = Instantiate(hips.gameObject) as GameObject;
			PrefabUtility.DisconnectPrefabInstance(HipFK);
			HipFK.transform.parent = hips.parent;
			HipFK.transform.position = hips.position;
			HipFK.transform.rotation = hips.rotation;
			HipFK.transform.localScale = hips.localScale;

			var children = hips.GetComponentsInChildren(typeof(Transform));
			var children2 = HipFK.GetComponentsInChildren(typeof(Transform));

			for (int j = 0; j < children2.Length; j++)
			{
				Bone b = children2[j].gameObject.AddComponent<Bone>();
				b.Radius = 0.01f;
				b.Color = Color.black;
			}

			for (int i = 0; i < children.Length; i++)
			{
				children[i].name = (children[i].name + "_JNT");
				IKFKBlend ikfkBlend = children[i].gameObject.AddComponent<IKFKBlend>();
				ikfkBlend.FK = children2[i] as Transform;

				Bone b = children[i].gameObject.AddComponent<Bone>();
				b.Radius = 0.01f;
				b.Color = Color.blue;
			}
			HipFK.name = HipName;


			IKFKBlend ikfk = OffsetIKFK(hips, globalControl, "Spine_01");
			if (ikfk != null)
			{
				ikfk.ContrstrainPosition = true;
				ikfk.ConstrainedControl = GameObject.Find("spline_Ctrl_GRP_1").transform;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(Chest, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04");
			if (ikfk != null)
			{
				ikfk.ContrstrainPosition = true;
				ikfk.ConstrainedControl = GameObject.Find("spline_Ctrl_GRP_2").transform;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(LeftUpperLeg, globalControl, "Spine_01/thighL");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.LegL;
				var ikfkChildren = ikfk.transform.GetComponentsInChildren<IKFKBlend>();
				
				for (int i = 0; i < ikfkChildren.Length; i++)
				{
					ikfkChildren[i].GroupID = IKFKBlend.IKFKType.LegL;
				}
				ikfk.Init();
			}
			ikfk = OffsetIKFK(RightUpperLeg, globalControl, "Spine_01/thighR");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.LegR;
				var ikfkChildren = ikfk.transform.GetComponentsInChildren<IKFKBlend>();
				for (int i = 0; i < ikfkChildren.Length; i++)
				{
					ikfkChildren[i].GroupID = IKFKBlend.IKFKType.LegR;
				}
				ikfk.Init();
			}
			ikfk = OffsetIKFK(LeftUpperArm, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavL/shoulderL");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.ArmL;
				var ikfkChildren = ikfk.transform.GetComponentsInChildren<IKFKBlend>();
				for (int i = 0; i < ikfkChildren.Length; i++)
				{
					ikfkChildren[i].GroupID = IKFKBlend.IKFKType.ArmL;
				}
				ikfk.Init();
			}
			ikfk = OffsetIKFK(RightUpperArm, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavR/shoulderR");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.ArmR;
				var ikfkChildren = ikfk.transform.GetComponentsInChildren<IKFKBlend>();
				for (int i = 0; i < ikfkChildren.Length; i++)
				{
					ikfkChildren[i].GroupID = IKFKBlend.IKFKType.ArmR;
				}
				ikfk.Init();
			}
			ikfk = OffsetIKFK(LeftHand, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavL/shoulderL/elbowL/handL");
			if (ikfk != null)
			{
				ikfk.Handle = GameObject.Find("handL_CTRL").transform;
				ikfk.GroupID = IKFKBlend.IKFKType.ArmL;				
				ikfk.Init();
			}
			ikfk = OffsetIKFK(RightHand, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavR/shoulderR/elbowR/handR");
			if (ikfk != null)
			{
				ikfk.Handle = GameObject.Find("handR_CTRL").transform;
				ikfk.GroupID = IKFKBlend.IKFKType.ArmR;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(LeftFoot, globalControl, "Spine_01/thighL/kneeL/footL");
			if (ikfk != null)
			{
				ikfk.Handle = GameObject.Find("footL_CTRL").transform;
				ikfk.GroupID = IKFKBlend.IKFKType.LegL;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(RightFoot, globalControl, "Spine_01/thighR/kneeR/footR");
			if (ikfk != null)
			{
				ikfk.Handle = GameObject.Find("footR_CTRL").transform;
				ikfk.GroupID = IKFKBlend.IKFKType.LegR;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(Neck, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/Neck");
			if (ikfk != null)
			{
				//ikfk.Handle = GameObject.Find("Head_CTRL").transform;
				ikfk.GroupID = IKFKBlend.IKFKType.Body;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(Head, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/Neck/Head");
			if (ikfk != null)
			{
				//ikfk.Handle = GameObject.Find("Head_CTRL").transform;
				ikfk.GroupID = IKFKBlend.IKFKType.Body;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(LeftLowerArm, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavL/shoulderL/elbowL");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.ArmL;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(RightLowerArm, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavR/shoulderR/elbowR");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.ArmR;
				ikfk.Init();
				ikfk = OffsetIKFK(LeftLowerLeg, globalControl, "Spine_01/thighL/kneeL");
				ikfk.GroupID = IKFKBlend.IKFKType.LegL;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(RightLowerLeg, globalControl, "Spine_01/thighR/kneeR");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.LegR;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(Spine, globalControl, "Spine_01/Spine_02");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.Body;				
				ikfk.Init();
			}
			ikfk = OffsetIKFK(ClavL, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavL");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.ArmL;
				ikfk.Init();
			}
			ikfk = OffsetIKFK(ClavR, globalControl, "Spine_01/Spine_02/Spine_03/Spine_04/clavR");
			if (ikfk != null)
			{
				ikfk.GroupID = IKFKBlend.IKFKType.ArmR;
				ikfk.Init();
			}
			if (rigFingers)
			{
				string handLString = "Spine_01/Spine_02/Spine_03/Spine_04/clavL/shoulderL/elbowL/handL";
				string handRString = "Spine_01/Spine_02/Spine_03/Spine_04/clavR/shoulderR/elbowR/handR";

				ikfk = OffsetIKFK(LeftIndexProximal, globalControl, handLString + "/indexL1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftIndexIntermediate, globalControl, handLString + "/indexL1/indexL2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftIndexDistal, globalControl, handLString + "/indexL1/indexL2/indexL3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftMiddleProximal, globalControl, handLString + "/middleL1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftMiddleIntermediate, globalControl, handLString + "/middleL1/middleL2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftMiddleDistal, globalControl, handLString + "/middleL1/middleL2/middleL3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftRingProximal, globalControl, handLString + "/ringL1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftRingIntermediate, globalControl, handLString + "/ringL1/ringL2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftRingDistal, globalControl, handLString + "/ringL1/ringL2/ringL3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftLittleProximal, globalControl, handLString + "/littleL1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftLittleIntermediate, globalControl, handLString + "/littleL1/littleL2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftLittleDistal, globalControl, handLString + "/littleL1/littleL2/littleL3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftThumbProximal, globalControl, handLString + "/thumbL1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftThumbIntermediate, globalControl, handLString + "/thumbL1/thumbL2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(LeftThumbDistal, globalControl, handLString + "/thumbL1/thumbL2/thumbL3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmL; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}

				ikfk = OffsetIKFK(RightIndexProximal, globalControl, handRString + "/indexR1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightIndexIntermediate, globalControl, handRString + "/indexR1/indexR2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightIndexDistal, globalControl, handRString + "/indexR1/indexR2/indexR3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightMiddleProximal, globalControl, handRString + "/middleR1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightMiddleIntermediate, globalControl, handRString + "/middleR1/middleR2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightMiddleDistal, globalControl, handRString + "/middleR1/middleR2/middleR3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightRingProximal, globalControl, handRString + "/ringR1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightRingIntermediate, globalControl, handRString + "/ringR1/ringR2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightRingDistal, globalControl, handRString + "/ringR1/ringR2/ringR3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightLittleProximal, globalControl, handRString + "/littleR1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightLittleIntermediate, globalControl, handRString + "/littleR1/littleR2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightLittleDistal, globalControl, handRString + "/littleR1/littleR2/littleR3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightThumbProximal, globalControl, handRString + "/thumbR1");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightThumbIntermediate, globalControl, handRString + "/thumbR1/thumbR2");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
				ikfk = OffsetIKFK(RightThumbDistal, globalControl, handRString + "/thumbR1/thumbR2/thumbR3");
				if (ikfk != null)
				{
					ikfk.GroupID = IKFKBlend.IKFKType.ArmR; ikfk.ContrstrainPosition = true;
					ikfk.Init();
				}
			}
			globalControl.Refresh();
			/*
			if (hips.parent != null)
			{
				hips.parent.gameObject.AddComponent<IKFKBlend> ();
				hips.parent.gameObject.AddComponent<FABRIKControl> ();
			}
			else
				hips.gameObject.AddComponent<FABRIKControl>();
			*/
		}

		private static IKFKBlend OffsetIKFK(Transform hips, GlobalControl globalControl, string boneName)
		{
			IKFKBlend ikfk = null;
			if (hips != null)
			{
				ikfk = hips.gameObject.GetComponent<IKFKBlend>();
				ikfk.IK = globalControl.transform.Find(boneName).transform;
				ikfk.Run();
				Quaternion before = hips.transform.rotation;
				ikfk.IKFK = 0f;
				ikfk.Run();
				Quaternion after = hips.transform.rotation;
				Quaternion offset = (Quaternion.Inverse(after) * before);
				ikfk.RotationOffset = offset;
			}
			return ikfk;
		}

		[MenuItem("GameObject/Puppet3D/AutoRig")]
		public static void AutoRig(GlobalControl globalControl = null)
		{
			bool rigFingers = false;
			if (GameObject.Find("indexL1_guide"))
				rigFingers = true;

			Guides[] guides = FindObjectsOfType<Guides>();
			foreach (Guides guide in guides)
			{
				Undo.RegisterCompleteObjectUndo(guides, "guides");

				// Set controls
				Vector3 _hipPoint = guide.transform.Find("hip_guide").GetComponent<Guide>().HandlePos;
				Vector3 _chestPoint = guide.transform.Find("chest_guide").GetComponent<Guide>().HandlePos;

				Vector3 _thighLPoint = guide.transform.Find("thighL_guide").GetComponent<Guide>().HandlePos;
				Vector3 _footLPoint = guide.transform.Find("footL_guide").GetComponent<Guide>().HandlePos;

				Vector3 _thighRPoint = guide.transform.Find("thighR_guide").GetComponent<Guide>().HandlePos;
				Vector3 _footRPoint = guide.transform.Find("footR_guide").GetComponent<Guide>().HandlePos;

				Vector3 _armLPoint = guide.transform.Find("armL_guide").GetComponent<Guide>().HandlePos;
				Vector3 _handLPoint = guide.transform.Find("handL_guide").GetComponent<Guide>().HandlePos;

				Vector3 _armRPoint = guide.transform.Find("armR_guide").GetComponent<Guide>().HandlePos;
				Vector3 _handRPoint = guide.transform.Find("handR_guide").GetComponent<Guide>().HandlePos;

				Vector3 _neckPoint = guide.transform.Find("neck_guide").GetComponent<Guide>().HandlePos;
				Vector3 _headPoint = guide.transform.Find("neck_guide/head_guide").GetComponent<Guide>().HandlePos;

				Vector3 _elbowLPoint = guide.transform.Find("elbowL_guide").GetComponent<Guide>().HandlePos - Vector3.forward * (guide.Bounds.size.y / 200f); 
				Vector3 _kneeLPoint = guide.transform.Find("kneeL_guide").GetComponent<Guide>().HandlePos + Vector3.forward * (guide.Bounds.size.y / 200f); ;

				Vector3 _elbowRPoint = guide.transform.Find("elbowR_guide").GetComponent<Guide>().HandlePos - Vector3.forward * (guide.Bounds.size.y / 200f); 
				Vector3 _kneeRPoint = guide.transform.Find("kneeR_guide").GetComponent<Guide>().HandlePos + Vector3.forward * (guide.Bounds.size.y / 200f);

				Vector3 _indexL1Point=Vector3.zero,_indexL2Point=Vector3.zero,_indexL3Point=Vector3.zero,_indexR1Point=Vector3.zero,_indexR2Point=Vector3.zero,_indexR3Point=Vector3.zero,_middleL1Point=Vector3.zero,_middleL2Point=Vector3.zero,_middleL3Point = Vector3.zero,
					_middleR1Point=Vector3.zero,_middleR2Point=Vector3.zero,_middleR3Point=Vector3.zero,_ringL1Point=Vector3.zero,_ringL2Point=Vector3.zero,_ringL3Point=Vector3.zero,_ringR1Point=Vector3.zero,_ringR2Point=Vector3.zero,_ringR3Point = Vector3.zero,
					_littleL1Point=Vector3.zero,_littleL2Point=Vector3.zero,_littleL3Point=Vector3.zero,_littleR1Point=Vector3.zero,_littleR2Point=Vector3.zero,_littleR3Point=Vector3.zero,_thumbL1Point=Vector3.zero,_thumbL2Point=Vector3.zero,_thumbL3Point = Vector3.zero,
					 _thumbR1Point=Vector3.zero,_thumbR2Point=Vector3.zero,_thumbR3Point = Vector3.zero;
				if (rigFingers)
				{
					_indexL1Point = guide.transform.Find("handL_guide/indexL1_guide").GetComponent<Guide>().HandlePos;
					_indexL2Point = guide.transform.Find("handL_guide/indexL1_guide/indexL2_guide").GetComponent<Guide>().HandlePos;
					_indexL3Point = guide.transform.Find("handL_guide/indexL1_guide/indexL2_guide/indexL3_guide").GetComponent<Guide>().HandlePos;

					_indexR1Point = guide.transform.Find("handR_guide/indexR1_guide").GetComponent<Guide>().HandlePos;
					_indexR2Point = guide.transform.Find("handR_guide/indexR1_guide/indexR2_guide").GetComponent<Guide>().HandlePos;
					_indexR3Point = guide.transform.Find("handR_guide/indexR1_guide/indexR2_guide/indexR3_guide").GetComponent<Guide>().HandlePos;

					 _middleL1Point = guide.transform.Find("handL_guide/middleL1_guide").GetComponent<Guide>().HandlePos;
					 _middleL2Point = guide.transform.Find("handL_guide/middleL1_guide/middleL2_guide").GetComponent<Guide>().HandlePos;
					 _middleL3Point = guide.transform.Find("handL_guide/middleL1_guide/middleL2_guide/middleL3_guide").GetComponent<Guide>().HandlePos;

					 _middleR1Point = guide.transform.Find("handR_guide/middleR1_guide").GetComponent<Guide>().HandlePos;
					 _middleR2Point = guide.transform.Find("handR_guide/middleR1_guide/middleR2_guide").GetComponent<Guide>().HandlePos;
					 _middleR3Point = guide.transform.Find("handR_guide/middleR1_guide/middleR2_guide/middleR3_guide").GetComponent<Guide>().HandlePos;

					 _ringL1Point = guide.transform.Find("handL_guide/ringL1_guide").GetComponent<Guide>().HandlePos;
					 _ringL2Point = guide.transform.Find("handL_guide/ringL1_guide/ringL2_guide").GetComponent<Guide>().HandlePos;
					 _ringL3Point = guide.transform.Find("handL_guide/ringL1_guide/ringL2_guide/ringL3_guide").GetComponent<Guide>().HandlePos;

					 _ringR1Point = guide.transform.Find("handR_guide/ringR1_guide").GetComponent<Guide>().HandlePos;
					 _ringR2Point = guide.transform.Find("handR_guide/ringR1_guide/ringR2_guide").GetComponent<Guide>().HandlePos;
					 _ringR3Point = guide.transform.Find("handR_guide/ringR1_guide/ringR2_guide/ringR3_guide").GetComponent<Guide>().HandlePos;

					 _littleL1Point = guide.transform.Find("handL_guide/littleL1_guide").GetComponent<Guide>().HandlePos;
					 _littleL2Point = guide.transform.Find("handL_guide/littleL1_guide/littleL2_guide").GetComponent<Guide>().HandlePos;
					 _littleL3Point = guide.transform.Find("handL_guide/littleL1_guide/littleL2_guide/littleL3_guide").GetComponent<Guide>().HandlePos;

					 _littleR1Point = guide.transform.Find("handR_guide/littleR1_guide").GetComponent<Guide>().HandlePos;
					 _littleR2Point = guide.transform.Find("handR_guide/littleR1_guide/littleR2_guide").GetComponent<Guide>().HandlePos;
					 _littleR3Point = guide.transform.Find("handR_guide/littleR1_guide/littleR2_guide/littleR3_guide").GetComponent<Guide>().HandlePos;

					 _thumbL1Point = guide.transform.Find("handL_guide/thumbL1_guide").GetComponent<Guide>().HandlePos;
					 _thumbL2Point = guide.transform.Find("handL_guide/thumbL1_guide/thumbL2_guide").GetComponent<Guide>().HandlePos;
					 _thumbL3Point = guide.transform.Find("handL_guide/thumbL1_guide/thumbL2_guide/thumbL3_guide").GetComponent<Guide>().HandlePos;

					 _thumbR1Point = guide.transform.Find("handR_guide/thumbR1_guide").GetComponent<Guide>().HandlePos;
					 _thumbR2Point = guide.transform.Find("handR_guide/thumbR1_guide/thumbR2_guide").GetComponent<Guide>().HandlePos;
					 _thumbR3Point = guide.transform.Find("handR_guide/thumbR1_guide/thumbR2_guide/thumbR3_guide").GetComponent<Guide>().HandlePos;
				}
				GlobalControl[] globalControls = FindObjectsOfType<GlobalControl>();

				
					// Hide other global controls

				foreach (GlobalControl gc in globalControls)
				{
					if(gc != globalControl)
						gc.gameObject.SetActive(false);
				}
				

				GameObject[] gos = new GameObject[1];
				gos[0] = guide.gameObject;
				if (guide.Biped != null)
					gos = guide.Biped;

				Bounds[] bounds = new Bounds[gos.Length];
				string[] geoNames = new string[gos.Length];

				GameObject Spine_01 = null;
				GameObject Spine_04 = null;
				MakeSpine(_hipPoint, _chestPoint, _footLPoint, out Spine_01, out Spine_04);
				GameObject legL = null;
				GameObject legL_CTRL = null;
				GameObject legLEnd = null;

				MakeLimb(_thighLPoint, _footLPoint, _kneeLPoint, Spine_01, "legL", "thighL", "kneeL", "footL", out legL, out legL_CTRL, out legLEnd, true);
				GameObject legR = null;
				GameObject legR_CTRL = null;
				GameObject legREnd = null;
				MakeLimb(_thighRPoint, _footRPoint, _kneeRPoint, Spine_01, "legR", "thighR", "kneeR", "footR", out legR, out legR_CTRL, out legREnd, true);

				GameObject clavL = null;
				GameObject handLObject = null;
				GameObject handL_CTRL = null;
				GameObject handLEnd = null;
				
				MakeClav(Spine_04, _armLPoint, "clavL", out clavL);
				MakeLimb(_armLPoint, _handLPoint, _elbowLPoint, clavL, "armL", "shoulderL", "elbowL", "handL", out handLObject, out handL_CTRL, out handLEnd, false);

				GameObject clavR = null;
				GameObject handRObject = null;
				GameObject handR_CTRL = null;
				GameObject handREnd = null;

				MakeClav(Spine_04, _armRPoint, "clavR", out clavR);
				MakeLimb(_armRPoint, _handRPoint, _elbowRPoint, clavR, "armR", "shoulderR", "elbowR", "handR", out handRObject, out handR_CTRL, out handREnd, false);


				Selection.activeGameObject = clavL;
				CreateControls.CreateParentControl();
				ParentControl clavLControl = Selection.activeGameObject.GetComponent<ParentControl>();
				string path = "arrowControl";
				Mesh controlMesh = (Mesh)Resources.Load(path, typeof(Mesh));
				clavLControl.OverrideMesh = controlMesh;
				clavLControl.HandleScale = Vector3.one * (guide.Bounds.size.y/5f);
				clavLControl.OverrideMeshOffset = new Vector3(0, guide.Bounds.size.y / 20f, guide.Bounds.size.y / 10f);

				Selection.activeGameObject = clavR;
				CreateControls.CreateParentControl();
				ParentControl clavRControl = Selection.activeGameObject.GetComponent<ParentControl>();
				clavRControl.OverrideMesh = controlMesh;
				clavRControl.HandleScale = Vector3.one*(guide.Bounds.size.y / 5f);
				clavRControl.OverrideMeshOffset = new Vector3(0, guide.Bounds.size.y / 20f, guide.Bounds.size.y / 10f);
				GameObject.Find("clavL_CTRL_GRP").transform.parent = GameObject.Find("spline_Ctrl_2").transform;
				GameObject.Find("clavR_CTRL_GRP").transform.parent = GameObject.Find("spline_Ctrl_2").transform;

				Vector3 headTop = new Vector3(_headPoint.x, guide.Bounds.max.y * .95f, _headPoint.z);
				MakeHead(_headPoint, _chestPoint, _neckPoint, headTop, Spine_04);

				GameObject[] handL = new GameObject[2] { handLObject, handL_CTRL };
				GameObject[] handR = new GameObject[2] { handRObject, handR_CTRL };

				float fingerScale = guide.Bounds.size.y / 75f;
				if (rigFingers)
				{
					Vector3[] indexFingersL = new Vector3[3];
					indexFingersL[0] = _indexL1Point; indexFingersL[1] = _indexL2Point; indexFingersL[2] = _indexL3Point;
					MakeFingers(indexFingersL, handL, "indexL", fingerScale);
					Vector3[] indexFingersR = new Vector3[3];
					indexFingersR[0] = _indexR1Point; indexFingersR[1] = _indexR2Point; indexFingersR[2] = _indexR3Point;
					MakeFingers(indexFingersR, handR, "indexR", fingerScale);

					Vector3[] middleFingersL = new Vector3[3];
					middleFingersL[0] = _middleL1Point; middleFingersL[1] = _middleL2Point; middleFingersL[2] = _middleL3Point;
					MakeFingers(middleFingersL, handL, "middleL", fingerScale);
					Vector3[] middleFingersR = new Vector3[3];
					middleFingersR[0] = _middleR1Point; middleFingersR[1] = _middleR2Point; middleFingersR[2] = _middleR3Point;
					MakeFingers(middleFingersR, handR, "middleR", fingerScale);

					Vector3[] ringFingersL = new Vector3[3];
					ringFingersL[0] = _ringL1Point; ringFingersL[1] = _ringL2Point; ringFingersL[2] = _ringL3Point;
					MakeFingers(ringFingersL, handL, "ringL", fingerScale);
					Vector3[] ringFingersR = new Vector3[3];
					ringFingersR[0] = _ringR1Point; ringFingersR[1] = _ringR2Point; ringFingersR[2] = _ringR3Point;
					MakeFingers(ringFingersR, handR, "ringR", fingerScale);

					Vector3[] littleFingersL = new Vector3[3];
					littleFingersL[0] = _littleL1Point; littleFingersL[1] = _littleL2Point; littleFingersL[2] = _littleL3Point;
					MakeFingers(littleFingersL, handL, "littleL", fingerScale);
					Vector3[] littleFingersR = new Vector3[3];
					littleFingersR[0] = _littleR1Point; littleFingersR[1] = _littleR2Point; littleFingersR[2] = _littleR3Point;
					MakeFingers(littleFingersR, handR, "littleR", fingerScale);

					Vector3[] thumbFingersL = new Vector3[3];
					thumbFingersL[0] = _thumbL1Point; thumbFingersL[1] = _thumbL2Point; thumbFingersL[2] = _thumbL3Point;
					MakeFingers(thumbFingersL, handL, "thumbL", fingerScale);
					Vector3[] thumbFingersR = new Vector3[3];
					thumbFingersR[0] = _thumbR1Point; thumbFingersR[1] = _thumbR2Point; thumbFingersR[2] = _thumbR3Point;
					MakeFingers(thumbFingersR, handR, "thumbR", fingerScale);

					DestroyImmediate(handLEnd);
					DestroyImmediate(handREnd);


					GameObject[] fingerBones = new GameObject[15];
					fingerBones[0] = GameObject.Find("indexL1_CTRL_GRP");
					fingerBones[1] = GameObject.Find("indexL2_CTRL_GRP");
					fingerBones[2] = GameObject.Find("indexL3_CTRL_GRP");
					fingerBones[3] = GameObject.Find("middleL1_CTRL_GRP");
					fingerBones[4] = GameObject.Find("middleL2_CTRL_GRP");
					fingerBones[5] = GameObject.Find("middleL3_CTRL_GRP");
					fingerBones[6] = GameObject.Find("ringL1_CTRL_GRP");
					fingerBones[7] = GameObject.Find("ringL2_CTRL_GRP");
					fingerBones[8] = GameObject.Find("ringL3_CTRL_GRP");
					fingerBones[9] = GameObject.Find("littleL1_CTRL_GRP");
					fingerBones[10] = GameObject.Find("littleL2_CTRL_GRP");
					fingerBones[11] = GameObject.Find("littleL3_CTRL_GRP");
					fingerBones[12] = GameObject.Find("thumbL1_CTRL_GRP");
					fingerBones[13] = GameObject.Find("thumbL2_CTRL_GRP");
					fingerBones[14] = GameObject.Find("thumbL3_CTRL_GRP");

					MakeHandDriver(handL_CTRL, fingerBones);

					fingerBones[0] = GameObject.Find("indexR1_CTRL_GRP");
					fingerBones[1] = GameObject.Find("indexR2_CTRL_GRP");
					fingerBones[2] = GameObject.Find("indexR3_CTRL_GRP");
					fingerBones[3] = GameObject.Find("middleR1_CTRL_GRP");
					fingerBones[4] = GameObject.Find("middleR2_CTRL_GRP");
					fingerBones[5] = GameObject.Find("middleR3_CTRL_GRP");
					fingerBones[6] = GameObject.Find("ringR1_CTRL_GRP");
					fingerBones[7] = GameObject.Find("ringR2_CTRL_GRP");
					fingerBones[8] = GameObject.Find("ringR3_CTRL_GRP");
					fingerBones[9] = GameObject.Find("littleR1_CTRL_GRP");
					fingerBones[10] = GameObject.Find("littleR2_CTRL_GRP");
					fingerBones[11] = GameObject.Find("littleR3_CTRL_GRP");
					fingerBones[12] = GameObject.Find("thumbR1_CTRL_GRP");
					fingerBones[13] = GameObject.Find("thumbR2_CTRL_GRP");
					fingerBones[14] = GameObject.Find("thumbR3_CTRL_GRP");

					MakeHandDriver(handR_CTRL, fingerBones);

				}

				Bone[] bones = FindObjectsOfType<Bone>();
				List<Object> objList = new List<Object>();

				for (int i = 0; i < bones.Length; i++)
				{
					if (bones[i])
					{
						bones[i].Radius = guide.Bounds.size.y / 100f;
						if(bones[i].name.Contains("index")|| bones[i].name.Contains("middle")|| bones[i].name.Contains("ring")|| bones[i].name.Contains("little")|| bones[i].name.Contains("thumb"))
							bones[i].Radius *= .75f;

						objList.Add(bones[i].gameObject);
					}
				}
				for (int i = 0; i < gos.Length; i++)
				{

					string modelName = gos[i].name;
					bounds[i] = gos[i].GetComponent<Renderer>().bounds;

					Selection.activeGameObject = gos[i];
					bool isSkinned = true;


					if (isSkinned)
					{
						geoNames[i] = modelName;

						GameObject geo = gos[i];
						if (geo == null)
							continue;
						SkinnedMeshRenderer smr = geo.GetComponent<SkinnedMeshRenderer>();
						if (smr == null)
						{
							//Undo.DestroyObjectImmediate(smr);
							//geo.AddComponent<MeshRenderer>();

							if (i == 0)
								objList.Add(geo);
							else
								objList[objList.Count - 1] = geo;

							Selection.objects = objList.ToArray();

							Skinning.BindSmoothSkin(Puppet3DEditor._skinningType);
							Selection.activeGameObject = geo;
						}

					}

				}
				

				globalControl = FindObjectOfType<GlobalControl>();
				if (globalControl.gameObject.GetComponent<Animator>() == null)
				{
					Animator animator = globalControl.gameObject.AddComponent<Animator>();


					if (animator != null && animator.runtimeAnimatorController == null)
						animator.runtimeAnimatorController = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(Puppet3DEditor._puppet3DPath + "/Animation/AutoRig/P3D_AnimatorController.controller", typeof(UnityEditor.Animations.AnimatorController));

					globalControl.Refresh();
				}
				//globalControl.BonesVisiblity = false;
				//globalControl.UpdateVisibility();
				foreach (GlobalControl gc in globalControls)
					gc.gameObject.SetActive(true);


				Undo.DestroyObjectImmediate(guide.gameObject);


			}
		}
		static void MakeFingers(Vector3[] fingers, GameObject[] parentTo, string boneName, float Scaler)
		{
			Selection.activeObject = parentTo[0];

			BoneCreation.CreateBoneTool();

			int index = 1;
			Transform parentControl = parentTo[1].transform;

			foreach (Vector3 finger in fingers)
			{
				BoneCreation.BoneCreationMode(finger, new Ray(), true);

				GameObject bone = GameObject.Find("bone_1");
				bone.name = (boneName + index);
				GameObject boneSelected = Selection.activeGameObject;

				Selection.activeGameObject = boneSelected;

				index++;

			}
			Vector3 endFingerDir = .5f*(fingers[2] - fingers[1]);
			BoneCreation.BoneCreationMode(fingers[2] + endFingerDir, new Ray(), true);

			BoneCreation.BoneFinishCreation();


			index = 1;

			for(int i=0;i<fingers.Length;i++)
			{
				string boneNameCurrent = (boneName + index);
				Selection.activeGameObject = GameObject.Find(boneNameCurrent);
				//GameObject boneSelected = Selection.activeGameObject;
				CreateControls.CreateOrientControl(Scaler);

				Selection.activeGameObject.transform.parent.SetParent(parentControl);
				parentControl = Selection.activeGameObject.transform;

				index++;

			}
			GameObject boneEnd = GameObject.Find("bone_1");
			boneEnd.name = boneName + "End";

			//DestroyImmediate(boneEnd);
		}
		static void MakeHandDriver(GameObject handControl, GameObject[] fingerBones)
		{
			DrivenKey drivenKey = handControl.AddComponent<DrivenKey>();
			drivenKey.DriverColection = new DrivenObject[1];
			drivenKey.DriverColection[0] = new DrivenObject(fingerBones);
			//drivenKey.DriverColection[0].DrivenGOs = fingerBones;
			drivenKey.BlendName = "Open Close";
			for (int i = 0; i < fingerBones.Length; i++)
			{
				if (drivenKey.DriverColection[0].DrivenGOs[i].name.Contains("thumbL"))
					drivenKey.DriverColection[0].EndRotations[i] *= Quaternion.Euler(new Vector3(0f, -30f, 0f));
				else if (drivenKey.DriverColection[0].DrivenGOs[i].name.Contains("thumbR"))
					drivenKey.DriverColection[0].EndRotations[i] *= Quaternion.Euler(new Vector3(0f, 30f, 0f));
				else
					drivenKey.DriverColection[0].EndRotations[i] *= Quaternion.Euler(new Vector3(90f, 0f, 0f));
			}
			drivenKey.DriverEnabled = true;
		}

		static void MakeClav(GameObject parentTo, Vector3 childToParentPos, string boneName, out GameObject clav)
		{
			Selection.activeObject = null;
			BoneCreation.CreateBoneTool();
			Vector3 dir =  (childToParentPos * .1f + parentTo.transform.position * .9f) ;
			BoneCreation.BoneCreationMode(dir, new Ray(), true);
			BoneCreation.BoneFinishCreation();
			clav = GameObject.Find("bone_1");
			clav.name = boneName;
			clav.transform.parent = parentTo.transform;

		}
		static void MakeLimb(Vector3 _thighLPoint, Vector3 _footLPoint, Vector3 _kneePoint, GameObject parentTo, string controlName, string controlName3, string controlName2, string controlName1, out GameObject handObject, out GameObject hand_CTRL, out GameObject handEnd, bool flip = false)
		{
			

			Selection.activeObject = parentTo;

			float limbScale = Vector3.Distance(_footLPoint, _thighLPoint) * .01f;
			Vector3 scaledFootPos = (_footLPoint - _thighLPoint) / limbScale;
			scaledFootPos += _thighLPoint;

			Vector3 scaledKneePos = (_kneePoint - _thighLPoint) / limbScale;
			scaledKneePos += _thighLPoint;

			BoneCreation.CreateBoneTool();
			BoneCreation.BoneCreationMode(_thighLPoint, new Ray(), true);

			BoneCreation.BoneCreationMode(scaledKneePos, new Ray(), true);
			GameObject endLimbGO = BoneCreation.BoneCreationMode(scaledFootPos, new Ray(), true);
			handObject = endLimbGO;

			Vector3 FootEnd = _footLPoint + .5f *(_footLPoint - _kneePoint);
			if (flip)
				FootEnd = _footLPoint + .25f * Vector3.forward * (_footLPoint - _kneePoint).magnitude;
			Vector3 scaledFinalEndLimbGO = (FootEnd - _thighLPoint) / limbScale;
			scaledFinalEndLimbGO += _thighLPoint;

			GameObject finalEndLimbGO = BoneCreation.BoneCreationMode(scaledFinalEndLimbGO, new Ray(), true);
			finalEndLimbGO.name = (controlName1 + "End");
			handEnd = finalEndLimbGO;

			Transform elbow = endLimbGO.transform.parent;
			Transform shoulderBone = endLimbGO.transform.parent.parent;
			endLimbGO.name = controlName1;
			elbow.name = (controlName2);
			shoulderBone.name = (controlName3);

			BoneCreation.BoneFinishCreation();
			
			Selection.activeGameObject = endLimbGO;
					
			CreateControls.IKCreateTool(false, true, limbScale*20f);
			GameObject limbControlParent = GameObject.Find(controlName1 + "_CTRL_GRP");
			hand_CTRL = GameObject.Find(controlName1 + "_CTRL");



			Transform limbParent = limbControlParent.transform.parent;
			limbControlParent.transform.parent = shoulderBone;
			shoulderBone.localScale = shoulderBone.localScale * limbScale;
			GameObject.Find(shoulderBone.name + "_FK").transform.localScale = shoulderBone.localScale;
			GameObject.Find(shoulderBone.name + "_IK").transform.localScale = shoulderBone.localScale;
			limbControlParent.transform.parent = limbParent;
		}


		static void MakeSpine(Vector3 _hipPoint, Vector3 _chestPoint, Vector3 GroundPos, out GameObject Spine_01, out GameObject Spine_04)
		{
			//Spine
			float hips2GroundScale = (_hipPoint.y - GroundPos.y);
			float spineScale = (.01f * hips2GroundScale) / 2.5f;
			Vector3 scaledChestPos = (_chestPoint - _hipPoint) / spineScale;
			scaledChestPos += _hipPoint;
			Puppet3DEditor.numberSplineJoints = 3;
			SplineEditor.CreateSplineTool();
			SplineEditor.SplineCreationMode(_hipPoint, new Ray(), true, spineScale * 100f, spineScale *50f, spineScale * 100f);
			SplineEditor.SplineCreationMode(scaledChestPos, new Ray(), true,  spineScale * 100f, spineScale * 50f, spineScale * 100f);
			SplineEditor.SplineFinishCreation();
			Transform splineRoot = GameObject.Find("spline_GRP_1").transform;
			Transform splineRootChild1 = GameObject.Find("spline_Ctrl_GRP_1").transform;
			Transform splineRootChild2 = GameObject.Find("spline_Ctrl_GRP_2").transform;
			splineRootChild1.parent = null;
			splineRootChild2.parent = null;
			splineRoot.position = _hipPoint;
			splineRootChild1.parent = splineRoot;
			splineRootChild2.parent = splineRoot;
			splineRoot.localScale = new Vector3(spineScale, spineScale, spineScale);
			Undo.RegisterCompleteObjectUndo(splineRoot, "spineScale");
			Spine_01 = GameObject.Find("bone_1");
			Spine_01.name = "Spine_01";
			GameObject.Find("bone_2").name = "Spine_02";
			GameObject.Find("bone_3").name = "Spine_03";
			Spine_04 = GameObject.Find("bone_4");
			Spine_04.name = "Spine_04";
			//Spine_04 = GameObject.Find("bone_3");
			//Spine_04.name = "Spine_03";
		}

		static void MakeHead(Vector3 _headPoint, Vector3 _chestPoint, Vector3 _neckPoint, Vector3 _headTop, GameObject Spine_04)
		{
			float headScale = Vector3.Distance(_chestPoint, _neckPoint) * .01f;


			Selection.activeObject = Spine_04;

			BoneCreation.CreateBoneTool();
			BoneCreation.BoneCreationMode(_neckPoint, new Ray(), true);
			BoneCreation.BoneCreationMode(_headPoint, new Ray(), true);
			BoneCreation.BoneCreationMode(_headTop, new Ray(), true);

			BoneCreation.BoneFinishCreation();

			GameObject neck = GameObject.Find("bone_1");
			neck.name = "Neck";
			GameObject head = GameObject.Find("bone_2");
			head.name = "Head";
			Selection.activeGameObject = neck;

			GameObject headEnd = GameObject.Find("bone_3");
			headEnd.name = "HeadEnd";

			CreateControls.CreateOrientControl(headScale*200f);
			GameObject neckControl = GameObject.Find("Neck_CTRL");
			Selection.activeGameObject = head;
			CreateControls.CreateOrientControl(headScale * 200f);
			GameObject headControl = GameObject.Find("Head_CTRL");

			ParentControl parentControlNeck = neckControl.GetComponent<ParentControl>();
			parentControlNeck.ConstrianedPosition = true;
			parentControlNeck.HandleScale = 150f * headScale * new Vector3(1f, 1f, .1f);

			ParentControl parentControlHead = headControl.GetComponent<ParentControl>();
			parentControlHead.HandleScale = 150f * headScale * new Vector3(1f,  1f, .1f);


			GameObject neckControlParent = GameObject.Find("Neck_CTRL_GRP");
			GameObject headControlParent = GameObject.Find("Head_CTRL_GRP");
			neckControlParent.transform.localScale = Vector3.one * headScale * 2f;
			//GameObject.Find ("bone_22").name = ("a");

			headControlParent.transform.parent = neckControl.transform;

		}
	}
}
