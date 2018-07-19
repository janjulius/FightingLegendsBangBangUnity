using UnityEngine;
using System.Collections;

namespace Puppet3D
{
	
	[ExecuteInEditMode]
	public class DrivenKey : MonoBehaviour
	{

		public DrivenObject[] DriverColection = new DrivenObject[0];
		
		public bool DriverEnabled = false;

		public float Blend = 0;
		public string BlendName ="Blend";

		
		public void Run()
		{
			if (DriverEnabled)
			{
				for (int i = 0; i < DriverColection.Length; i++)
				{
					for (int j = 0; j < DriverColection[i].DrivenGOs.Length; j++)
					{
						DriverColection[i].DrivenGOs[j].transform.localPosition = Vector3.Lerp(DriverColection[i].StartPositions[j], DriverColection[i].EndPositions[j], Blend);
						DriverColection[i].DrivenGOs[j].transform.localRotation = Quaternion.Lerp(DriverColection[i].StartRotations[j], DriverColection[i].EndRotations[j], Blend);
					}
				}
			}
		}
	}
	[System.Serializable]
	public class DrivenObject
	{
		public GameObject[] DrivenGOs = new GameObject[0];

		public Vector3[] StartPositions = new Vector3[0];
		public Vector3[] EndPositions = new Vector3[0];

		public Quaternion[] StartRotations = new Quaternion[0];
		public Quaternion[] EndRotations = new Quaternion[0];

		public float Blend = 0f;

		public string BlendName = "Blend";
		
		public DrivenObject(GameObject[] objs)
		{
			
			DrivenGOs = new GameObject[objs.Length];
			StartPositions = new Vector3[objs.Length];
			StartRotations = new Quaternion[objs.Length];
			EndPositions = new Vector3[objs.Length];
			EndRotations = new Quaternion[objs.Length];



			for (int i = 0; i < objs.Length; i++)
			{
				DrivenGOs[i] = objs[i];
				StartPositions[i] = objs[i].transform.localPosition;
				StartRotations[i] = objs[i].transform.localRotation;

				EndPositions[i] = objs[i].transform.localPosition;
				EndRotations[i] = objs[i].transform.localRotation;

			}

		}
	}
}
