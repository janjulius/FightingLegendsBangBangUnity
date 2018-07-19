using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet3D
{
	public class BoneCreation : Editor {


		public static void BoneFinishCreation()
		{

			Puppet3DEditor.BoneCreation = false;
			EditorPrefs.SetBool("Puppet3D_BoneCreation", false);

			GameObject globalCtrlNew = CreateControls.CreateGlobalControl();


			Bone[] bones = GameObject.FindObjectsOfType<Bone>();
			foreach (Bone bone in bones)
			{
				if(bone.transform.parent == null )
					bone.transform.parent = globalCtrlNew.transform;


			}


		}
		[MenuItem("GameObject/Puppet3D/Skeleton/Create Bone Tool")]
		public static void CreateBoneTool()
		{
			Puppet3DEditor.BoneCreation = true;
			EditorPrefs.SetBool("Puppet3D_BoneCreation", true);

		}


		public static void sortOutBoneHierachy(GameObject changedBone, bool move = false)
		{


			SpriteRenderer spriteRenderer = changedBone.GetComponent<SpriteRenderer>();
			if (spriteRenderer)
				if (spriteRenderer.sprite)
					if (!spriteRenderer.sprite.name.Contains("Bone"))
						return;

			// UNPARENT CHILDREN
			List<Transform> children = new List<Transform>();
						
			foreach (Transform child in children)
			{
				if (!move)
					child.transform.parent = null;
			}
			Transform changedBonesParent = null;
			Transform changedBonesParentsParent = null;
			if (changedBone.transform.parent)
			{
				changedBonesParent = changedBone.transform.parent.transform;
				Undo.RecordObject(changedBonesParent, "bone parent");

				if (changedBone.transform.parent.transform.parent)
				{
					changedBonesParentsParent = changedBone.transform.parent.transform.parent.transform;

					changedBone.transform.parent.transform.parent = null;
				}

			}
			if (!move)
				changedBone.transform.parent = null;

			List<Transform> parentsChildren = new List<Transform>();

			// ORIENT & SCALE PARENT

			if (changedBonesParent)
			{
				
				foreach (Transform child in parentsChildren)
				{
					Undo.RecordObject(child, "parents child");
					child.transform.parent = null;
				}
				SpriteRenderer sprParent = changedBonesParent.GetComponent<SpriteRenderer>();
				if (sprParent)
					if (sprParent.sprite)
						if (sprParent.sprite.name.Contains("Bone"))
						{
							float dist = Vector3.Distance(changedBonesParent.position, changedBone.transform.position);
							if (dist > 0)
								changedBonesParent.rotation = Quaternion.LookRotation(changedBone.transform.position - changedBonesParent.position, Vector3.back) * Quaternion.AngleAxis(90, Vector3.right);
							float length = (changedBonesParent.position - changedBone.transform.position).magnitude;

							changedBonesParent.localScale = new Vector3(length, length, length);
						}


			}
			if (!move)
				changedBone.transform.localScale = Vector3.one;

			// REPARENT CHILDREN

			if (children.Count > 0)
			{
				foreach (Transform child in children)
				{
					SpriteRenderer spr = child.GetComponent<SpriteRenderer>();
					if (spr)
						if (spr.sprite)
							if (spr.sprite.name.Contains("Bone"))
							{
								Undo.RecordObject(child, "parents child");
								child.transform.parent = changedBone.transform;
							}
				}
			}
			else
			{
				Undo.RecordObject(spriteRenderer, "sprite change");
			}

			if (changedBonesParent)
			{
				changedBone.transform.parent = changedBonesParent;
				if (changedBonesParentsParent)
					changedBone.transform.parent.transform.parent = changedBonesParentsParent;

				foreach (Transform child in parentsChildren)
				{
					Undo.RecordObject(child, "parents child");
					child.transform.parent = changedBonesParent;
				}
				SpriteRenderer spr = changedBonesParent.GetComponent<SpriteRenderer>();
				if (spr)
				{
					if (spr.sprite)
					{
						if (spr.sprite.name.Contains("Bone"))
						{
							Undo.RecordObject(spr, "sprite change");
							spr.sprite = Puppet3DEditor.boneSprite;
						}
					}
				}



			}

			// SET CORRECT SPRITE
			if (!move)
			{
				if (children.Count > 0)
					changedBone.GetComponent<SpriteRenderer>().sprite = Puppet3DEditor.boneSprite;
				else
					changedBone.GetComponent<SpriteRenderer>().sprite = Puppet3DEditor.boneNoJointSprite;

			}

			children.Clear();
			parentsChildren.Clear();

		}
		public static float lastZ =10f;
		public static float lastBoneRadius = .5f;

		public static GameObject BoneCreationMode(Vector3 mousPos, Ray ray1 = new Ray(), bool isAutorRig = false)
		{
			RaycastHit hit;

			GameObject bone = new GameObject();
			Undo.RegisterCreatedObjectUndo(bone, "Created Bone");
			bone.name = GetUniqueBoneName("bone");
			Bone boneComp = bone.AddComponent<Bone>();

			if (isAutorRig)
			{
				bone.transform.position = mousPos;
			}
			else
			{
				if (Physics.Raycast(ray1, out hit))
				{
					if(hit.transform.GetComponent<Renderer>()!=null)
						lastBoneRadius = hit.transform.GetComponent<Renderer>().bounds.size.y/100f;
					RaycastHit newHit;
					if (Physics.Raycast(hit.point + 10f * ray1.direction, -1f * ray1.direction, out newHit))
						bone.transform.position = (hit.point + newHit.point) / 2f;
					else
						bone.transform.position = (hit.point);
					lastZ = Vector3.Distance(ray1.origin,bone.transform.position);
				}
				else
					bone.transform.position = ray1.GetPoint(lastZ);
			}

			if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Bone>())
			{
				if(Selection.activeGameObject.transform.childCount==0)
					Selection.activeGameObject.transform.LookAt(bone.transform);
				bone.transform.parent = Selection.activeGameObject.transform;
			}
			boneComp.Radius = lastBoneRadius;

			Selection.activeGameObject = bone;

				return bone;
			
		}
		public static void BoneMoveMode(Vector3 mousePos,Ray ray1 = new Ray())
		{
			GameObject bone = Selection.activeGameObject;

			if (bone && bone.GetComponent<Bone>())
			{
				RaycastHit hit;
				Undo.RecordObject (bone.transform, "Transform Position");

				if (Physics.Raycast(ray1, out hit))
				{
					RaycastHit newHit;
					if (Physics.Raycast(hit.point + 10f * ray1.direction, -1f * ray1.direction, out newHit))
						bone.transform.position = (hit.point + newHit.point) / 2f;
					else
						bone.transform.position = (hit.point);
					lastZ = Vector3.Distance(ray1.origin,bone.transform.position);

				}
				else
					bone.transform.position = ray1.GetPoint(lastZ);


				
				//selectedGO.transform.position = new Vector3(Selection.activeGameObject.transform.position.x, Selection.activeGameObject.transform.position.y, 0);

				//if (!isParentSplineBone)
				//	sortOutBoneHierachy(selectedGO, true);

			}

		}
		public static void BoneMoveIndividualMode(Vector3 mousePos,Ray ray1 = new Ray())
		{
			GameObject bone = Selection.activeGameObject;
			
			if (bone && bone.GetComponent<Bone>())
			{
				List<Vector3> childPos = new List<Vector3>();
				foreach(Transform child in bone.transform)
				{
					childPos.Add(child.position);
				}
				RaycastHit hit;
				Undo.RecordObject (bone.transform, "Transform Position");

				if (Physics.Raycast(ray1, out hit))
				{
					RaycastHit newHit;
					if (Physics.Raycast(hit.point + 10f * ray1.direction, -1f * ray1.direction, out newHit))
						bone.transform.position = (hit.point + newHit.point) / 2f;

					else
						bone.transform.position = (hit.point);
					lastZ = Vector3.Distance(ray1.origin,bone.transform.position);
					
				}
				else
					bone.transform.position = ray1.GetPoint(lastZ);
				
				int index=0;
				foreach(Transform child in bone.transform)
				{
					Undo.RecordObject (child, "Transform Position");

					child.position = childPos[index];
					index++;
				}
				//selectedGO.transform.position = new Vector3(Selection.activeGameObject.transform.position.x, Selection.activeGameObject.transform.position.y, 0);
				
				//if (!isParentSplineBone)
				//	sortOutBoneHierachy(selectedGO, true);
				
			}
			

		}
		public static void BoneDeleteMode()
		{
			GameObject selectedGO = Selection.activeGameObject;
			if (selectedGO)
			{
				
				if (selectedGO.GetComponent<SpriteRenderer>())
				{
					if (selectedGO.GetComponent<SpriteRenderer>().sprite)
					{
						if (selectedGO.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
						{
							// MAKE SURE SELECTION IS NOT AN IK OR PARENT

							GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<GlobalControl>();
							for (int i = 0; i < globalCtrlScripts.Length; i++)
							{
								foreach (IKControl Ik in globalCtrlScripts[i]._Ikhandles)
								{
									if ((Ik.topJointTransformIK == selectedGO.transform) || (Ik.bottomJointTransformIK == selectedGO.transform) || (Ik.middleJointTransformIK == selectedGO.transform))
									{
										Debug.LogWarning("Cannot move bone, as this one has an IK handle");
										return;
									}
								}
								foreach (SplineControl splineCtrl in globalCtrlScripts[i]._SplineControls)
								{

									foreach (GameObject bone in splineCtrl.bones)
									{
										if (bone.transform == selectedGO.transform)
										{
											Debug.LogWarning("Cannot delete Spline Bones Individually");
											return;
										}

									}
								}
							}
						}
						else
							return;
					}
					else
						return;
				}
				else
					return;

				if (selectedGO.transform.parent)
				{
					GameObject parentGO = selectedGO.transform.parent.gameObject;
					DestroyImmediate(selectedGO);
					sortOutBoneHierachy(parentGO);
					Selection.activeGameObject = parentGO;
					
				}
				else
				{
					DestroyImmediate(selectedGO);
				}

			}

		}
		public static void BoneAddMode(Vector3 mousePos)
		{
			GameObject selectedGO = Selection.activeGameObject;

			if (selectedGO)
			{
				
				if (selectedGO.GetComponent<SpriteRenderer>())
				{
					if (selectedGO.GetComponent<SpriteRenderer>().sprite)
					{
						if (selectedGO.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
						{
							// MAKE SURE SELECTION IS NOT AN IK OR PARENT

							GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<GlobalControl>();
							for (int i = 0; i < globalCtrlScripts.Length; i++)
							{
								foreach (IKControl Ik in globalCtrlScripts[i]._Ikhandles)
								{
									if ((Ik.topJointTransformIK == selectedGO.transform) || (Ik.bottomJointTransformIK == selectedGO.transform) || (Ik.middleJointTransformIK == selectedGO.transform))
									{
										Debug.LogWarning("Cannot add bone, as this one has an IK handle");
										return;
									}
								}
								foreach (SplineControl splineCtrl in globalCtrlScripts[i]._SplineControls)
								{

									foreach (GameObject bone in splineCtrl.bones)
									{
										if (bone.transform == selectedGO.transform)
										{
											Debug.LogWarning("Cannot add to Spline Bones");
											return;
										}

									}
								}
							}
						}
						else
							return;
					}
					else
						return;
				}
				else
					return;


				List<Transform> children = new List<Transform>();
				foreach (Transform child in selectedGO.transform)
					children.Add(child);
				foreach (Transform child in children)
					child.parent = null;

				GameObject newBone = BoneCreationMode(mousePos);

				foreach (Transform child in children)
				{

					child.parent = newBone.transform;
					

				}
				Selection.activeGameObject = newBone;
				children.Clear();

			}

		}

		public static string GetUniqueBoneName(string name)
		{
			string nameToAdd = name;
			int nameToAddLength = nameToAdd.Length + 1;
			int index = 0;
			foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if (go.name.StartsWith(nameToAdd))
				{
					string endOfName = go.name.Substring(nameToAddLength, go.name.Length - nameToAddLength);

					int indexTest = 0;
					if (int.TryParse(endOfName, out indexTest))
					{
						if (int.Parse(endOfName) > index)
						{
							index = int.Parse(endOfName);
						}
					}


				}
			}
			index++;
			return (name + "_" + index);

		}
	}
}
