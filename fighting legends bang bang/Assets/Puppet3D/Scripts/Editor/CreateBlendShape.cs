using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Puppet3D
{
	public class CreateBlendShape : MonoBehaviour
	{
		static public void MakeBlendShape()
		{
			GameObject sel = Selection.activeGameObject;
			if (sel != null && sel.GetComponent<SkinnedMeshRenderer>()!= null)
			{
				
				GameObject newBlendShape = new GameObject("newBlendShape");
				BlendShape bs = newBlendShape.AddComponent<BlendShape>();
				bs.SourceSkin = sel.GetComponent<SkinnedMeshRenderer>();

				MeshFilter meshFilter = newBlendShape.AddComponent<MeshFilter>();
				Mesh newMesh = Skinning.SaveFBXMesh(bs.SourceSkin.sharedMesh, true);
				meshFilter.sharedMesh = newMesh;
				MeshRenderer mr = newBlendShape.AddComponent<MeshRenderer>();
				bs.TargetMeshFilter = meshFilter;
				mr.sharedMaterials = bs.SourceSkin.sharedMaterials;
				bs.Init();
			}

			
		}
		static public void ClearBlendShapes()
		{
			GameObject sel = Selection.activeGameObject;
			if (sel != null && sel.GetComponent<SkinnedMeshRenderer>() != null)
			{
				sel.GetComponent<SkinnedMeshRenderer>().sharedMesh.ClearBlendShapes();
			}
		}

	}
}
