using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet3D
{
	public class SplineEditor : Editor
	{

		public static GameObject SplineCreationGroup;
		public static StoreData splineStoreData;

		public static void SplineFinishCreation()
		{
			Puppet3DEditor.SplineCreation = false;
			if (splineStoreData == null)
				return;
			CreateSpline();
			splineStoreData.Data.Clear();

		}

		static void CreateSpline()
		{
			if (splineStoreData.Data.Count > 2 && splineStoreData.Data[0] && splineStoreData.Data[1] && splineStoreData.Data[2])
			{

				GameObject tangentCtrl = new GameObject(BoneCreation.GetUniqueBoneName("spline_Tangent"));
				Undo.RegisterCreatedObjectUndo(tangentCtrl, "Created splineTangent");
				splineStoreData.Data.Add(tangentCtrl.transform);
				tangentCtrl.transform.parent = splineStoreData.Data[splineStoreData.Data.Count - 2].transform;
				tangentCtrl.transform.localPosition = Vector3.zero;
				

				splineStoreData.Data[1].position += splineStoreData.Data[0].position - splineStoreData.Data[2].position;

				splineStoreData.Data[splineStoreData.Data.Count - 1].position += splineStoreData.Data[splineStoreData.Data.Count - 2].position - splineStoreData.Data[splineStoreData.Data.Count - 3].position;

				Transform splineCtrlSwap = splineStoreData.Data[0];
				splineStoreData.Data[0] = splineStoreData.Data[1];
				splineStoreData.Data[1] = splineCtrlSwap;

				SplineControl spline = SplineCreationGroup.AddComponent<SplineControl>();

				spline._splineCTRLS.AddRange(splineStoreData.Data);
				spline.numberBones = Puppet3DEditor.numberSplineJoints;
				spline.Create();
				
				foreach (Transform ctrl in splineStoreData.Data)
				{
					if (!ctrl.parent.parent)
						ctrl.parent.parent = SplineCreationGroup.transform;
				}
				GameObject globalCtrl = CreateControls.CreateGlobalControl();
				globalCtrl.GetComponent<GlobalControl>()._SplineControls.Add(spline);
				SplineCreationGroup.transform.parent = globalCtrl.transform;

				globalCtrl.GetComponent<GlobalControl>().InitializeArrays();
				globalCtrl.GetComponent<GlobalControl>().Run();

				Undo.DestroyObjectImmediate(splineStoreData);

				splineStoreData.Data.Clear();



				Bone[] bones = GameObject.FindObjectsOfType<Bone>();
				foreach (Bone bone in bones)
				{
					if (bone.transform.parent == null)
						bone.transform.parent = globalCtrl.transform;


				}
			}



		}

		public static void CreateSplineTool()
		{
			Puppet3DEditor.SplineCreation = true;

			SplineCreationGroup = new GameObject(BoneCreation.GetUniqueBoneName("spline_GRP"));
			Undo.RegisterCreatedObjectUndo(SplineCreationGroup, "undo create Spline");
			splineStoreData = SplineCreationGroup.AddComponent<StoreData>();

		}
		public static float lastZ = 10f;

		public static void SplineCreationMode(Vector3 mousePos, Ray ray1 = new Ray(), bool autoRig = false, float DefaultSizeX = 1f, float DefaultSizeY = 1f, float DefaultSizeZ = 1f)
		{

			GameObject newCtrl = new GameObject(BoneCreation.GetUniqueBoneName("spline_Ctrl"));
			Undo.RegisterCreatedObjectUndo(newCtrl, "Created newCtrl");
			GameObject newCtrlGrp = new GameObject(BoneCreation.GetUniqueBoneName("spline_Ctrl_GRP"));
			Undo.RegisterCreatedObjectUndo(newCtrlGrp, "Created newCtrlGrp");
			newCtrl.transform.parent = newCtrlGrp.transform;

			Undo.RecordObject(splineStoreData, "Adding To Spline Control");

			splineStoreData.Data.Add(newCtrl.transform);


			// start and end
			if (splineStoreData.Data.Count == 1)
			{
				GameObject tangentCtrl = new GameObject(BoneCreation.GetUniqueBoneName("spline_Tangent"));
				Undo.RegisterCreatedObjectUndo(tangentCtrl, "Created splineTangent");
				splineStoreData.Data.Add(tangentCtrl.transform);
				tangentCtrl.transform.parent = splineStoreData.Data[0].transform;
			}

			if (autoRig)
			{
				newCtrlGrp.transform.position = mousePos;
			}
			else
			{

				RaycastHit hit;
				/*if (Physics.Raycast(ray1, out hit))
				{
					RaycastHit newHit;
					if (Physics.Raycast(hit.point + 10f * ray1.direction, -1f * ray1.direction, out newHit))
						newCtrlGrp.transform.position = (hit.point + newHit.point) / 2f;
					else
						newCtrlGrp.transform.position = (hit.point);

					lastZ = Vector3.Distance(ray1.origin, newCtrlGrp.transform.position);

				}*/
				if (Physics.Raycast(ray1, out hit))
				{					
					RaycastHit newHit;
					if (Physics.Raycast(hit.point + 10f * ray1.direction, -1f * ray1.direction, out newHit))
						newCtrlGrp.transform.position = (hit.point + newHit.point) / 2f;
					else
						newCtrlGrp.transform.position = (hit.point);
					lastZ = Vector3.Distance(ray1.origin, newCtrlGrp.transform.position);
				}
				else
					newCtrlGrp.transform.position = ray1.GetPoint(lastZ);
			}

			newCtrlGrp.transform.position = new Vector3(newCtrlGrp.transform.position.x, newCtrlGrp.transform.position.y, newCtrlGrp.transform.position.z);

			Spline newControl = newCtrl.AddComponent<Spline>();
			newControl.HandleSize = new Vector3(DefaultSizeX, DefaultSizeY, DefaultSizeZ);

			Selection.activeGameObject = newCtrl;
		}

	}
}