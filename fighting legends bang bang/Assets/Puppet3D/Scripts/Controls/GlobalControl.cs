using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Puppet3D
{
	[ExecuteInEditMode]
	public class GlobalControl : Control
	{		
		public bool BonesVisiblity = true;
		public bool ControlsVisiblity = true;

		public List<SplineControl> _SplineControls = new List<SplineControl>();
		public List<IKControl> _Ikhandles = new List<IKControl>();
		public List<ParentControl> _ParentControls = new List<ParentControl>();
		public List<IKFKBlend> _IKFKBlends = new List<IKFKBlend>();
		public List<DrivenKey> _DrivenKeys = new List<DrivenKey>();

		[Range(0f, 1f)]
		public float Body = 0f;
		[Range (0f, 1f)]
		public float ArmLeft = 0f;
		[Range(0f, 1f)]
		public float ArmRight = 0f;
		[Range(0f, 1f)]
		public float LegLeft = 0f;
		[Range(0f, 1f)]
		public float LegRight = 0f;
		
		[HideInInspector]
		public List<Control> _Controls = new List<Control>();
		[HideInInspector]
		public List<Bone> _Bones = new List<Bone>();

		

		//public bool CombineMeshes = false;
		
		public bool AutoRefresh = true;
		public bool ControlsEnabled = true;
		public bool lateUpdate = true;

		[HideInInspector]
		public int _flipCorrection = 1;

		public float HandleRadius = 1f;

		// CACHED VALUES

		private Transform myTransform;
		private SplineControl[] _SplineControlsArray;
		private IKControl[] _IkhandlesArray;
		private ParentControl[] _ParentControlsArray;

		private Mesh _mesh;

		//public float boneSize;
		// Use this for initialization
		void OnEnable()
		{

			if (AutoRefresh)
			{
				_Ikhandles.Clear();
				_SplineControls.Clear();
				_ParentControls.Clear();
				_DrivenKeys.Clear();
				_Controls.Clear();
				_Bones.Clear();
				_IKFKBlends.Clear();
				TraverseHierarchy(transform);
				InitializeArrays();
			}

		}
		public void Refresh()
		{
			_Ikhandles.Clear();
			_SplineControls.Clear();
			_ParentControls.Clear();
			_DrivenKeys.Clear();
			_IKFKBlends.Clear();
			_Controls.Clear();
			_Bones.Clear();
			TraverseHierarchy(transform);
			InitializeArrays();
		}
		void Awake()
		{
			this.myTransform = this.GetComponent<Transform>();

			


		}
		void Start()
		{
			string path = "globalControl";
			_mesh = (Mesh)Resources.Load(path, typeof(Mesh));
		}
		public void InitializeArrays()
		{
			_SplineControlsArray = _SplineControls.ToArray();
			_IkhandlesArray = _Ikhandles.ToArray();
			_ParentControlsArray = _ParentControls.ToArray();
		}
		public void Init()
		{

			_Ikhandles.Clear();
			_SplineControls.Clear();
			_ParentControls.Clear();
			_DrivenKeys.Clear();
			_Controls.Clear();
			_Bones.Clear();
			TraverseHierarchy(transform);
			InitializeArrays();

		}
		void OnValidate()
		{
			UpdateVisibility();
		}

		public void UpdateVisibility()
		{
			if (AutoRefresh)
			{
				_Ikhandles.Clear();
				_SplineControls.Clear();
				_ParentControls.Clear();
				_DrivenKeys.Clear();
				_IKFKBlends.Clear();
				_Controls.Clear();
				_Bones.Clear();
				TraverseHierarchy(transform);
				InitializeArrays();
			}
			foreach (Control ctrl in _Controls)
			{
				if (ctrl && ctrl.Active != ControlsVisiblity)
					ctrl.Active = ControlsVisiblity;
			}
			foreach (Bone bone in _Bones)
			{
				if (bone && bone.enabled != BonesVisiblity)
					bone.enabled = BonesVisiblity;
			}
		
		}
		private void OnDrawGizmos()
		{
			if (Active && this.enabled)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireMesh(_mesh, 0, transform.position, transform.rotation, transform.localScale * 200f * HandleRadius);
			}

		}
		void Update()
		{
			if (!lateUpdate)
			{

#if UNITY_EDITOR
				if (AutoRefresh)
				{
					for (int i = _ParentControls.Count - 1; i >= 0; i--)
					{
						if (_ParentControls[i] == null)
							_ParentControls.RemoveAt(i);
					}
					for (int i = _Ikhandles.Count - 1; i >= 0; i--)
					{
						if (_Ikhandles[i] == null)
							_Ikhandles.RemoveAt(i);
					}
					for (int i = _SplineControls.Count - 1; i >= 0; i--)
					{
						if (_SplineControls[i] == null)
							_SplineControls.RemoveAt(i);
					}
					for (int i = _DrivenKeys.Count - 1; i >= 0; i--)
					{
						if (_DrivenKeys[i] == null)
							_DrivenKeys.RemoveAt(i);
					}
					for (int i = _IKFKBlends.Count - 1; i >= 0; i--)
						{
							if (_IKFKBlends[i] == null)
								_IKFKBlends.RemoveAt(i);
						}
				}
#endif

				if (ControlsEnabled)
					Run();

				
			}





		}
		void LateUpdate()
		{
			if (lateUpdate)
			{

#if UNITY_EDITOR
				if (AutoRefresh)
				{
					for (int i = _ParentControls.Count - 1; i >= 0; i--)
					{
						if (_ParentControls[i] == null)
							_ParentControls.RemoveAt(i);
					}
					for (int i = _Ikhandles.Count - 1; i >= 0; i--)
					{
						if (_Ikhandles[i] == null)
							_Ikhandles.RemoveAt(i);
					}
					for (int i = _SplineControls.Count - 1; i >= 0; i--)
					{
						if (_SplineControls[i] == null)
							_SplineControls.RemoveAt(i);
					}
					for (int i = _DrivenKeys.Count - 1; i >= 0; i--)
					{
						if (_DrivenKeys[i] == null)
							_DrivenKeys.RemoveAt(i);
					}
					for (int i = _IKFKBlends.Count - 1; i >= 0; i--)
					{
						if (_IKFKBlends[i] == null)
							_IKFKBlends.RemoveAt(i);
					}

				}
#endif

				if (ControlsEnabled)
					Run();

				
			}





		}
		public void Run()
		{
			for (int i = 0; i < _IKFKBlends.Count; i++)
			{
				if (_IKFKBlends[i] && _IKFKBlends[i].GroupID == IKFKBlend.IKFKType.Body)
				{
					_IKFKBlends[i].RunConstrianedControls();

				}
			}
			for (int i = 0; i < _SplineControlsArray.Length; i++)
			{
				if (_SplineControlsArray[i])
					_SplineControlsArray[i].Run();
			}
			for (int i = 0; i < _ParentControlsArray.Length; i++)
			{
				if (_ParentControlsArray[i])
					_ParentControlsArray[i].ParentControlRun();
			}
			for (int i = 0; i < _DrivenKeys.Count; i++)
			{
				if (_DrivenKeys[i])
					_DrivenKeys[i].Run();
			}
			FaceCamera();
			for (int i = 0; i < _IkhandlesArray.Length; i++)
			{
				if (_IkhandlesArray[i])
					_IkhandlesArray[i].CalculateIK();
			}
			
			for (int i = 0; i < _IKFKBlends.Count; i++)
			{
				if (_IKFKBlends[i])
				{
					switch (_IKFKBlends[i].GroupID)
					{
						case IKFKBlend.IKFKType.Body:
							_IKFKBlends[i].IKFK = Body;
							break;
						case IKFKBlend.IKFKType.ArmL :
							_IKFKBlends[i].IKFK = ArmLeft;
							break;
						case IKFKBlend.IKFKType.ArmR:
							_IKFKBlends[i].IKFK = ArmRight;
							break;
						case IKFKBlend.IKFKType.LegL:
							_IKFKBlends[i].IKFK = LegLeft;
							break;
						case IKFKBlend.IKFKType.LegR:
							_IKFKBlends[i].IKFK = LegRight;
							break;
						
					
					}

					_IKFKBlends[i].Run();
				}
			}
			



		}

		public void TraverseHierarchy(Transform root)
		{
			Control globalControl = root.transform.GetComponent<Control>();
			_Controls.Add(globalControl);

			foreach (Transform child in root)
			{
				GameObject Go = child.gameObject;

				Control control = Go.transform.GetComponent<Control>();
				_Controls.Add(control);

				Bone bone = Go.transform.GetComponent<Bone>();
				_Bones.Add(bone);

				ParentControl newParentCtrl = Go.transform.GetComponent<ParentControl>();

				if (newParentCtrl)
				{
					_ParentControls.Add(newParentCtrl);

				}
				IKControl newIKCtrl = Go.transform.GetComponent<IKControl>();
				if (newIKCtrl)
					_Ikhandles.Add(newIKCtrl);

				
				SplineControl splineCtrl = Go.transform.GetComponent<SplineControl>();
				if (splineCtrl)
					_SplineControls.Add(splineCtrl);

				IKFKBlend ikfkBlend = Go.transform.GetComponent<IKFKBlend>();
				if (ikfkBlend)
					_IKFKBlends.Add(ikfkBlend);

				DrivenKey newDrivenKey = Go.transform.GetComponent<DrivenKey>();

				if (newDrivenKey)
				{
					_DrivenKeys.Add(newDrivenKey);

				}

				TraverseHierarchy(child);

			}

		}
		void CombineAllMeshes()
		{
			Vector3 originalScale = transform.localScale;
			Quaternion originalRot = transform.rotation;
			Vector3 originalPos = transform.position;
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
			transform.position = Vector3.zero;

			SkinnedMeshRenderer[] smRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			List<Transform> bones = new List<Transform>();
			List<BoneWeight> boneWeights = new List<BoneWeight>();
			List<CombineInstance> combineInstances = new List<CombineInstance>();
			List<Texture2D> textures = new List<Texture2D>();

			Material currentMaterial = null;

			int numSubs = 0;
			var smRenderersDict = new Dictionary<SkinnedMeshRenderer, float>(smRenderers.Length);

			bool updateWhenOffscreen = false;

			foreach (SkinnedMeshRenderer smr in smRenderers)
			{
				smRenderersDict.Add(smr, smr.transform.position.z);
				updateWhenOffscreen = smr.updateWhenOffscreen;
			}


			var items = from pair in smRenderersDict
						orderby pair.Key.sortingOrder ascending
						select pair;

			items = from pair in items
					orderby pair.Value descending
					select pair;
			foreach (KeyValuePair<SkinnedMeshRenderer, float> pair in items)
			{
				//Debug.Log(pair.Key.name + " " + pair.Value);
				numSubs += pair.Key.sharedMesh.subMeshCount;
			}


			int[] meshIndex = new int[numSubs];
			int boneOffset = 0;

			int s = 0;
			foreach (KeyValuePair<SkinnedMeshRenderer, float> pair in items)
			{
				SkinnedMeshRenderer smr = pair.Key;

				if (currentMaterial == null)
					currentMaterial = smr.sharedMaterial;
				else if (currentMaterial.mainTexture && smr.sharedMaterial.mainTexture && currentMaterial.mainTexture != smr.sharedMaterial.mainTexture)
					continue;

				
				BoneWeight[] meshBoneweight = smr.sharedMesh.boneWeights;

				foreach (BoneWeight bw in meshBoneweight)
				{
					BoneWeight bWeight = bw;

					bWeight.boneIndex0 += boneOffset;
					bWeight.boneIndex1 += boneOffset;
					bWeight.boneIndex2 += boneOffset;
					bWeight.boneIndex3 += boneOffset;

					boneWeights.Add(bWeight);
				}
				boneOffset += smr.bones.Length;

				Transform[] meshBones = smr.bones;
				foreach (Transform bone in meshBones)
					bones.Add(bone);

				if (smr.material.mainTexture != null)
					textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);



				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				meshIndex[s] = ci.mesh.vertexCount;
				ci.transform = smr.transform.localToWorldMatrix;
				combineInstances.Add(ci);

				Object.Destroy(smr.gameObject);
				s++;
			}


			List<Matrix4x4> bindposes = new List<Matrix4x4>();

			for (int b = 0; b < bones.Count; b++)
			{
				
				bindposes.Add(bones[b].worldToLocalMatrix * transform.worldToLocalMatrix);
			}

			SkinnedMeshRenderer r = gameObject.AddComponent<SkinnedMeshRenderer>();

			r.updateWhenOffscreen = updateWhenOffscreen;
			r.sharedMesh = new Mesh();
			r.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

			Material combinedMat;
			if (currentMaterial != null)
				combinedMat = currentMaterial;
			else
				combinedMat = new Material(Shader.Find("Unlit/Transparent"));

			combinedMat.mainTexture = textures[0];
			r.sharedMesh.uv = r.sharedMesh.uv;
			r.sharedMaterial = combinedMat;

			r.bones = bones.ToArray();
			r.sharedMesh.boneWeights = boneWeights.ToArray();
			r.sharedMesh.bindposes = bindposes.ToArray();
			r.sharedMesh.RecalculateBounds();


			transform.localScale = originalScale;
			transform.rotation = originalRot;
			transform.position = originalPos;
		}
		private void FaceCamera()
		{
			for (int i = 0; i < this._IkhandlesArray.Length; ++i)
			{
				this._IkhandlesArray[i].AimDirection = this.myTransform.forward.normalized * _flipCorrection;
			}
		}
	}	

}
