using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Puppet3D
{
	[CustomEditor(typeof(BlendShape))]
	public class BlendShapeEditor : Editor
	{

		public override void OnInspectorGUI()
		{
			
			if (GUILayout.Button("Set Blend Shape"))
			{
				(target as BlendShape).SetBlendShape();
			}
			DrawDefaultInspector();

		}
		
	}
}