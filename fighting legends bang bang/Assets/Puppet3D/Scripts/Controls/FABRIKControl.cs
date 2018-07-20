using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class FABRIKControl : MonoBehaviour 
	{
		public class BoneInfo
		{
			public Transform Trans;
			public Vector3 Pos;
			public float L;
			public Transform Source;
			public Quaternion Offset= Quaternion.identity;

			public BoneInfo(Transform TransSet, Transform SourceSet)
			{
				Trans = TransSet;
				if(SourceSet != null)
					Source = SourceSet;
				else
					Source = Trans;
				Pos = TransSet.position;
				L = 0f;
				if(TransSet.parent)
				{
					L = Vector3.Distance(TransSet.position, TransSet.parent.position);
				}
				if(Trans.childCount>0)
				{
					Quaternion startRot = Trans.rotation;
					Trans.LookAt(Trans.GetChild(0).position);
					Quaternion endRot = Trans.rotation;
					Trans.rotation = startRot;
					Offset = Quaternion.Inverse(endRot)*startRot;
				}
			}
		}
		public BoneInfoChain BoneChain;

		public void Awake()
		{
			Init ();
		}
		public void OnValidate()
		{
			Init ();
		}
		public void OnEnable()
		{
			Init ();
		}
		public void Init()
		{
			BoneChain = new BoneInfoChain (transform);
			BoneChain.Print ();
			BoneChain.SetInitialBones ();

		}
		public void Update()
		{
			BoneChain.SetInitialBones ();
			/*if (BoneChain.IsTooFar ()) 
			{
				BoneChain.StrechedSolve ();
				//Debug.Log("STRETCHED");
			} 
			else */
			{
				//for (int i = 0; i < 50; i++)
				{
					BoneChain.Backward();
					BoneChain.Forward(transform.position, true);
				}
			}
			BoneChain.SetBones ();

		}

		public class BoneInfoChain
		{
			public Transform Effector;
			public Transform Root;
			public Vector3 EffectorPos;
			public IKControl ikControl;
			public float ik;


			private List<BoneInfo> _boneInfos = new List<BoneInfo>();
			private float _MinDistance = 0.01f;
			List<BoneInfoChain> _childBoneChains = new List<BoneInfoChain>();

			public BoneInfoChain (Transform root)
			{
				Transform boneInfoTransform = root;
				IKFKBlend ikfkBlend = boneInfoTransform.GetComponent<IKFKBlend>();

				if (ikfkBlend != null)
					_boneInfos.Add(new BoneInfo(boneInfoTransform, ikfkBlend.FK));

				while(boneInfoTransform.childCount ==1 && boneInfoTransform.GetComponent<IKFKBlend>()!=null && boneInfoTransform.GetComponent<IKFKBlend>().Handle==null)
				{
					//Debug.Log("adding "+ boneInfoTransform.name +" " +count);
					boneInfoTransform = boneInfoTransform.GetChild(0);

					ikfkBlend = boneInfoTransform.GetComponent<IKFKBlend>();
					if(ikfkBlend!=null)
						_boneInfos.Add(new BoneInfo(boneInfoTransform, ikfkBlend.FK));
					else
						break;
				}

				// Set End Effector

				ikfkBlend = boneInfoTransform.GetComponent<IKFKBlend>();

				if (ikfkBlend!=null && ikfkBlend.Handle !=null)
				{														
					Effector = ikfkBlend.Handle;
					ikControl = Effector.GetComponent<IKControl>();
					ik = ikControl.IKFK;

					EffectorPos = Effector.position;
				}
				else
				{
					// multi children
					if (boneInfoTransform.childCount >1)
					{
						foreach(Transform child in boneInfoTransform)
						{
							if (ikfkBlend == null && child.GetComponent<IKFKBlend>() == null)
							{; }
							else
								_childBoneChains.Add(new BoneInfoChain (child));
						}
					}

						
				}



					
			}
			public void Print()
			{
				Debug.Log ("Chain :");
				foreach(BoneInfo bi in _boneInfos)
				{
					Debug.Log ("Bone " + bi.Trans.name);

				}
				if(Effector!=null)					
					Debug.Log ("Effector " + Effector.name);

				foreach(BoneInfoChain chain in _childBoneChains)
				{
					Debug.Log ("<<<<<< :");

					chain.Print ();

					Debug.Log (">>>>>> :");

				}
			}
			public Vector3 Backward()
			{
				if (Effector != null) {
					EffectorPos = Effector.position;

				} 
				else if (_childBoneChains.Count > 0) {


					// Move all chain children towards effectors
					Vector3 newEffectPos = Vector3.zero;
					int count = _childBoneChains.Count;
					if (count > 0) {
						foreach (BoneInfoChain chain in _childBoneChains) {
							newEffectPos += chain.Backward ();
						}
						EffectorPos = newEffectPos / count;

					}
				}
				else if(_boneInfos.Count>0)
				{
					return _boneInfos[0].Source.position;

				}
				
					

				// Move this chain back to effector
				if (_boneInfos.Count > 0) {
					_boneInfos [_boneInfos.Count - 1].Pos =Vector3.Lerp( EffectorPos, _boneInfos[_boneInfos.Count - 1].Pos, ik);
					for (int i = _boneInfos.Count - 2; i >= 0; i--) {
						Vector3 dir = (_boneInfos [i].Pos - _boneInfos [i + 1].Pos).normalized;
						_boneInfos [i].Pos = _boneInfos [i + 1].Pos + _boneInfos [i + 1].L * dir;						
					}
					return _boneInfos [0].Pos;
				} else
					return Vector3.zero;

			}
			public void Forward(Vector3 newRootPos, bool isRoot = false)
			{

				//_boneInfos[0].Pos = newRootPos;
				if (isRoot) 
				{
					//_boneInfos [0].Pos = _boneInfos [0].Source.position;
					_boneInfos[0].Pos = Vector3.Lerp(_boneInfos[0].Source.position, _boneInfos[0].Pos, ik);
					for (int i = 1; i < _boneInfos.Count; i++) 
					{
						Vector3 dir;


						dir = (_boneInfos [i].Pos - _boneInfos [i - 1].Pos).normalized;
						_boneInfos [i].Pos = _boneInfos [i - 1].Pos + _boneInfos [i].L * dir;

					}
				} 
				else 
				{

					for (int i = 0; i < _boneInfos.Count; i++) {
						Vector3 dir;

						if (i == 0) 
						{
								dir = (_boneInfos [i].Pos - newRootPos).normalized;
								_boneInfos [i].Pos = newRootPos + _boneInfos [i].L * dir;

						} 
						else 
						{
							dir = (_boneInfos [i].Pos - _boneInfos [i - 1].Pos).normalized;
							_boneInfos [i].Pos = _boneInfos [i - 1].Pos + _boneInfos [i].L * dir;
						}

					}
				}

				// Move all chain children towards Root

				foreach (BoneInfoChain chain in _childBoneChains) 
				{
					chain.Forward (_boneInfos[_boneInfos.Count-1].Pos);
				}


			}
			bool MinDistanceCheck()
			{
				if (Vector3.Distance (_boneInfos[_boneInfos.Count - 1].Pos, EffectorPos) < _MinDistance)
					return true;
				else
					return false;
			}
			public bool IsTooFar()
			{	
				float dist = SumDistances ();
				if (dist > Vector3.Distance (EffectorPos, _boneInfos[0].Pos))
					return false;
				else
					return true;
			}
			float SumDistances()
			{
				float dist = 0;
				for(int i=0;i< _boneInfos.Count;i++) {
					dist += _boneInfos[i].L;
				}
				return dist;
			}
			public void SetInitialBones()
			{
				//ik = 0;
				if (Effector != null)
				{
					EffectorPos = Effector.position;
					ik = ikControl.IKFK;
				}
				
				for (int i = 0; i < _boneInfos.Count; i++)
				{
					_boneInfos[i].Pos = _boneInfos[i].Source.position;
				}

				/*float ikVal = ik;
				int count = _childBoneChains.Count;
				if (count > 0)
				{
					foreach (BoneInfoChain chain in _childBoneChains)
					{
						ikVal += chain.SetInitialBones();
					}
					ik = ikVal / count;

				}*/
				
				foreach (BoneInfoChain chain in _childBoneChains) 
				{				

					chain.SetInitialBones();
				}
				//return ik;
			}
			public void SetBones()
			{
				if (Effector == null && _childBoneChains.Count == 0)
				{

					/*for (int i = 0; i < _boneInfos.Count - 1; i++)
					{
						_boneInfos[i].Trans.position = _boneInfos[i].Source.position;
						_boneInfos[i].Trans.rotation = _boneInfos[i].Source.rotation;


					}*/
				}
				else
				{
					if (_boneInfos.Count > 0)
					{
						_boneInfos[0].Trans.position = (_boneInfos[0].Pos);
						//_boneInfos[0].Trans.position = Vector3.Lerp((_boneInfos[0].Pos), _boneInfos[0].Trans.position, ik);

					}


					for (int i = 0; i < _boneInfos.Count - 1; i++)
					{
						_boneInfos[i].Trans.LookAt(_boneInfos[i + 1].Pos);
						_boneInfos[i].Trans.rotation *= _boneInfos[i].Offset;
						//_boneInfos[i].Trans.rotation = Quaternion.Lerp(_boneInfos[i].Trans.rotation, _boneInfos[i].Source.rotation, ik);
					}

					/*for (int i = 0; i < _boneInfos.Count; i++)
					{
						_boneInfos[i].Trans.position = (_boneInfos[i].Pos);
					}*/
					foreach (BoneInfoChain chain in _childBoneChains)
					{
						chain.SetBones();
					}
				}
			}
			public void StrechedSolve()
			{
				Vector3 dir = (EffectorPos - _boneInfos[0].Pos).normalized;
				Vector3 calcPos = _boneInfos[0].Pos;

				for (int i = 1; i < _boneInfos.Count; i++)
				{
					_boneInfos[i].Pos = calcPos + _boneInfos[i].L * dir;
					calcPos = _boneInfos[i].Pos;
				}
			}
			/*
			// Update is called once per frame
			void Update () {
				// Check If Control is further than sum of distances
				foreach (List<BoneInfo> boneinfo in _boneInfos)
				{
					if (IsTooFar(boneinfo))
					{
						Vector3 dir = (boneinfo[0].IKHandle.position - boneinfo[0].Pos).normalized;
						Vector3 calcPos = boneinfo[0].Pos;

						for (int i = 1; i < boneinfo.Count; i++)
						{
							boneinfo[i].Pos = calcPos + boneinfo[i].L * dir;
							calcPos = boneinfo[i].Pos;
						}
					}
					else
					{
						int count = 0;
						while (!MinDistanceCheck(boneinfo) && count < 9)
						{
							Backward(boneinfo);
							Forward(boneinfo);
							count++;
						}
					}

				}

				foreach (List<BoneInfo> boneinfo in _boneInfos)
				{
					for (int i = 0; i < boneinfo.Count - 1; i++)
					{
						boneinfo[i].Trans.LookAt(boneinfo[i + 1].Pos);
					}
				}
				
			}
			*/


		}
	}
}
