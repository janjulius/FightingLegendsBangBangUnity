using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


namespace Puppet3D
{
	public class Puppet3DSortingLayer : MonoBehaviour
	{
		[HideInInspector]
		public Vector2 offsetAmount;
		[HideInInspector]
		public Vector2[] uvsDefault;
		[HideInInspector]
		public bool initialized = false;
		[HideInInspector]
		public Bounds bounds;

	}
}
