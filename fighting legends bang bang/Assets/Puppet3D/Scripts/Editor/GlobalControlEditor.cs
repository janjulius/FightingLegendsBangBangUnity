using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet3D
{
	[CustomEditor(typeof(GlobalControl))]
	public class GlobalControlEditor : Editor
	{

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if (GUILayout.Button("Refresh Global Control"))
			{
				(target as GlobalControl).Refresh();
			}

		}


	}
}