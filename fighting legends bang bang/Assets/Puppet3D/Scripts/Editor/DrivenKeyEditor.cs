using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Puppet3D
{
	[CustomEditor(typeof(DrivenKey))]
	public class DrivenKeyEditor : Editor
	{
		
		public DrivenKey myTarget;

		public void OnEnable()
		{
			myTarget = (DrivenKey)target;
			if (myTarget.DriverColection == null)
				myTarget.DriverColection = new DrivenObject[0];
		}

		public override void OnInspectorGUI()
		{
			//DrawDefaultInspector();

			EditorGUI.BeginChangeCheck();
			float val = EditorGUILayout.Slider(myTarget.BlendName, myTarget.Blend, 0f, 1f);

			if (EditorGUI.EndChangeCheck())
			{
				myTarget.Run();
				Undo.RecordObject(myTarget, "blend");
				myTarget.Blend = val;

			}

			/*
			if (myTarget.DriverColection != null)
			{
				for (int i = 0; i < myTarget.DriverColection.Length; i++)
				{
					EditorGUI.BeginChangeCheck();

					float b = EditorGUILayout.Slider(myTarget.DriverColection[i].BlendName, myTarget.DriverColection[i].Blend, 0f, 1f);

					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(myTarget, "blend");
						myTarget.Run();
						myTarget.DriverColection[i].Blend = b;
						//EditorUtility.SetDirty(myTarget.DriverColection[i]);
					}
				}
			}
			*/
			myTarget.DriverEnabled = EditorGUILayout.Toggle("Enabled" ,myTarget.DriverEnabled);
			/*if (GUILayout.Button("Add Slider"))
			{
				DrivenObject[] oldVals = new DrivenObject[myTarget.DriverColection.Length + 1];
				
				for (int i = 0; i < myTarget.DriverColection.Length; i++)
				{
					if (myTarget.DriverColection[i] == null)
						myTarget.DriverColection[i] = new DrivenObject(null);
					oldVals[i] = myTarget.DriverColection[i];
					
				}
				myTarget.DriverColection = oldVals;
				
			}
			if (GUILayout.Button("Remove Slider"))
			{
				if (myTarget.DriverColection.Length > 0)
				{
					DrivenObject[] oldVals = new DrivenObject[myTarget.DriverColection.Length - 1];
					

					for (int i = 0; i < myTarget.DriverColection.Length - 1; i++)
					{
						oldVals[i] = myTarget.DriverColection[i];
						
					}
					myTarget.DriverColection = oldVals;
				
				}
			}*/
			
		}

	}
}
