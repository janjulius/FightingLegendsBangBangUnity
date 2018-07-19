using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Puppet3D
{
	[ExecuteInEditMode]
	public class StoreData : MonoBehaviour {

		public List<Transform> Data = new List<Transform>();
		public Vector3 OriginalSpritePosition = Vector3.zero;
		[HideInInspector]
		public bool Editable = true;
		void Update()
		{
			if (Editable)
			{
				for (int i = Data.Count - 1; i >= 0; i--)
				{
					if (Data[i] == null)
						Data.RemoveAt(i);
				}
			}
		}
	}
}
