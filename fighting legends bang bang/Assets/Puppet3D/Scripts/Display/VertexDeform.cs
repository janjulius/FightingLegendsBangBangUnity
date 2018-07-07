#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Puppet3D
{
	[ExecuteInEditMode]
	public class VertexDeform : MonoBehaviour
	{

		private static float CURRENT_SIZE = 0.02f;
		private static float CURRENT_FALLOFF = 1f;

		public float _size = CURRENT_SIZE;
		public float _falloff = CURRENT_FALLOFF;

		public BlendShape _parent;

		private float _lastKnownSize = CURRENT_SIZE;
		private float _lastKnownFalloff = CURRENT_FALLOFF;


		public Vector3 IntialPos;
		public bool Active = true;
		public bool Editted = false;
		public bool Selected = false;
		public Vector3 CurrentPos;

		public List<int> ConnectedIndexes = new List<int>();
		public Color ColorVal;
		public int CurrentIndex;
		private GameObject _currentSel = null;
		private void OnEnable()
		{
			CurrentPos = transform.position;
			ColorVal = Color.white;
		}
		void Update()
		{
			
			//ColorVal = Color.white;
			// Change the size if the user requests it
			if (_lastKnownSize != _size)
			{
				_lastKnownSize = _size;
				CURRENT_SIZE = _size;
			}

			// Ensure the rest of the gizmos know the size has changed...
			if (CURRENT_SIZE != _lastKnownSize)
			{
				_lastKnownSize = CURRENT_SIZE;
				_size = _lastKnownSize;
			}

			

			// FALL OFF WIP
			

			if (_lastKnownFalloff != _falloff)
			{
				_lastKnownFalloff = _falloff;
				CURRENT_FALLOFF = _falloff;
			}

			
			if (CURRENT_FALLOFF != _lastKnownFalloff)
			{
				_lastKnownFalloff = CURRENT_FALLOFF;
				_falloff = _lastKnownFalloff;
			}


			float dist = 0;

			List<int> WorkingList = new List<int>();
			Queue<int> indexQueue = new Queue<int>();

			GameObject testGo = null;
			foreach (GameObject go in Selection.gameObjects)
			{
				VertexDeform vd = go.GetComponent<VertexDeform>();
				if (vd)
				{
					vd.Selected = true;
					if(vd.Active)
						testGo = go;
				}
			}

			if (testGo!=null && testGo == this.gameObject)
			{
				
				if (Active)
				{
					
					if (_currentSel != Selection.activeGameObject)
					{
						_currentSel = Selection.activeGameObject;
						for (int i = 0; i < _parent.Deforms.Length; i++)
						{
							_parent.Deforms[i].ColorVal = Color.white;
						}
					}



					WorkingList.Add(CurrentIndex);
					Editted = true;
					ColorVal = Color.green;

					_parent.verts[CurrentIndex] = _parent.Deforms[CurrentIndex].transform.position;

					for (int c = 0; c < ConnectedIndexes.Count; c++)
					{
						indexQueue.Enqueue(ConnectedIndexes[c]);
					}
					Color blendColor = Color.white;
					while (indexQueue.Count > 0)
					{
						int workingIndex = indexQueue.Dequeue();
						if (!_parent.Deforms[workingIndex].Editted)
						{
							_parent.Deforms[workingIndex].ColorVal = blendColor;
							WorkingList.Add(workingIndex);
							_parent.Deforms[workingIndex].Editted = true;

							dist = Vector3.Distance(IntialPos, _parent.Deforms[workingIndex].IntialPos);
							if (dist < _falloff)
							{
								float blend = (1 - (dist / _falloff));
								if (!_parent.Deforms[workingIndex].Selected)
								{
									_parent.Deforms[workingIndex].transform.position += (transform.position - CurrentPos) * blend;
								}
								_parent.verts[workingIndex] = _parent.Deforms[workingIndex].transform.position;

								blendColor = Color.Lerp(Color.white, Color.green, blend);
								_parent.Deforms[workingIndex].ColorVal = blendColor;

								for (int c = 0; c < _parent.Deforms[workingIndex].ConnectedIndexes.Count; c++)
								{
									indexQueue.Enqueue(_parent.Deforms[workingIndex].ConnectedIndexes[c]);
								}
							}
						}

					}


					foreach (int val in WorkingList)
					{
						_parent.Deforms[val].CurrentPos = _parent.Deforms[val].gameObject.transform.position;
						_parent.Deforms[val].Editted = false;
						_parent.Deforms[val].Selected = false;
					}
				}

			}

			





		}
		/*float ProcessChildDeforms(float dist,List<int> ConnectedIndexes, int currentIndex, List<int> WorkingList)
		{
			WorkingList.Add(currentIndex);
			for (int c = 0; c < ConnectedIndexes.Count; c++)
			{
				if (!WorkingList.Contains( ConnectedIndexes[c]))
				{
					dist = Vector3.Distance(_parent.Deforms[ConnectedIndexes[c]].IntialPos, _parent.Deforms[currentIndex].IntialPos);
					if (dist < _falloff)
					{
						//Debug.Log(ConnectedIndexes[c]);
						_parent.Deforms[ConnectedIndexes[c]].transform.position += (_parent.Deforms[currentIndex].transform.position - _parent.Deforms[currentIndex].IntialPos);
						dist = ProcessChildDeforms(dist, _parent.Deforms[ConnectedIndexes[c]].ConnectedIndexes, _parent.Deforms[ConnectedIndexes[c]].CurrentIndex, WorkingList);
					}
				}
			}
			return dist;
		}*/
		void OnDrawGizmos()
		{
			if (Active)
			{
				//float dist = 0;
				/*if (Selection.activeGameObject != null)
				{
					VertexDeform selGizmo = Selection.activeGameObject.GetComponent<VertexDeform>();
					if (selGizmo != null)
					{
						dist = Vector3.Distance(selGizmo.CurrentPos, CurrentPos) * (1f / CURRENT_FALLOFF);
					}
				}
				dist = Mathf.Clamp01(dist);*/
				//
				/*if (Selection.activeGameObject != null)
				{
					VertexDeform selGizmo = Selection.activeGameObject.GetComponent<VertexDeform>();
					if (selGizmo != null)
					{
						for (int c = 0; c < ConnectedIndexes.Count; c++)
						{
							_parent.Deforms[ConnectedIndexes[c]].ColorVal = Color.green;
						}
					}
				}*/

				Gizmos.color = ColorVal*2f;// Color.Lerp(Color.white, Color.green,  dist);
				Gizmos.DrawSphere(transform.position, CURRENT_SIZE);
			}
			
		}
	}
}
#endif