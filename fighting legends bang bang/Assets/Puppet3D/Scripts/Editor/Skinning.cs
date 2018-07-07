using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
namespace Puppet3D
{
	public class Skinning : Editor
	{
		private static GameObject _workingMesh;
		private static GameObject[] _workingBones;


		[MenuItem("GameObject/Puppet3D/Skin/Parent Mesh To Bones")]
		public static void BindRigidSkinMenu()
		{
			BindRigidSkin();
		}

		public static void BindRigidSkin(List<GameObject> selectedBones = null, List<GameObject> selectedMeshes = null)
		{
			if (selectedMeshes == null)
			{
				GameObject[] selection = Selection.gameObjects;
				selectedBones = new List<GameObject>();
				selectedMeshes = new List<GameObject>();

				foreach (GameObject Obj in selection)
				{
					if (Obj.GetComponent<SpriteRenderer>())
					{
						if (Obj.GetComponent<SpriteRenderer>().sprite && Obj.GetComponent<SpriteRenderer>().sprite.name.Contains("ffd"))
							selectedMeshes.Add(Obj.transform.parent.gameObject);
						else
						{
							if (Obj.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
							{

								if (Obj.transform.childCount > 0)
								{
									selectedBones.Add(Obj);

								}
								else
									selectedBones.Add(Obj);

							}
							else
							{
								selectedMeshes.Add(Obj);
							}
						}
					}
					else
					{
						selectedMeshes.Add(Obj);
					}
				}
			}
			if ((selectedBones.Count == 0) || (selectedMeshes.Count == 0))
			{
				Debug.LogWarning("You need to select at least one bone and one other object");
				return;
			}
			foreach (GameObject mesh in selectedMeshes)
			{
				float testdist = 1000000;
				GameObject closestBone = null;
				for (int i = 0; i < selectedBones.Count; i++)
				{
					GameObject bone = selectedBones[i];
					//float dist = Vector2.Distance(new Vector2(bone.GetComponent<SpriteRenderer>().bounds.center.x, bone.GetComponent<Renderer>().bounds.center.y), new Vector2(mesh.transform.position.x, mesh.transform.position.y));
					Vector3 centre;
					if (bone.transform.parent != null)
						centre = (bone.transform.position + bone.transform.parent.position) / 2f;
					else
						centre = bone.transform.position;
					float dist = Vector2.Distance(new Vector2(centre.x, centre.y), new Vector2(mesh.transform.position.x, mesh.transform.position.y));

					if (dist < testdist)
					{

						testdist = dist;

						closestBone = bone.transform.parent.gameObject;
					}

				}

				Undo.SetTransformParent(mesh.transform, closestBone.transform, "parent bone");

			}

		}
		static bool ContainsPoint(Vector3[] polyPoints, Vector3 p)
		{
			bool inside = false;
			float a1 = Vector3.Angle(polyPoints[0] - p, polyPoints[1] - p);
			float a2 = Vector3.Angle(polyPoints[1] - p, polyPoints[2] - p);
			float a3 = Vector3.Angle(polyPoints[2] - p, polyPoints[0] - p);

			if (Mathf.Abs((a1 + a2 + a3) - 360) < 0.1f)
			{
				inside = true;
				//Debug.Log((a1 + a2 + a3));
			}
			//        for (int index = 0; index < polyPoints.Length; j = index++) 
			//        { 
			//            if ( ((polyPoints[index].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[index].y)) && 
			//                (p.x < (polyPoints[j].x - polyPoints[index].x) * (p.y - polyPoints[index].y) / (polyPoints[j].y - polyPoints[index].y) + polyPoints[index].x)) 
			//                inside = !inside; 
			//        } 
			return inside;
		}
		static Vector3 Barycentric(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
		{
			Vector3 v0 = b - a;
			Vector3 v1 = c - a;
			Vector3 v2 = p - a;
			float d00 = Vector3.Dot(v0, v0);
			float d01 = Vector3.Dot(v0, v1);
			float d11 = Vector3.Dot(v1, v1);
			float d20 = Vector3.Dot(v2, v0);
			float d21 = Vector3.Dot(v2, v1);
			float denom = d00 * d11 - d01 * d01;


			float v = (d11 * d20 - d01 * d21) / denom;
			float w = (d00 * d21 - d01 * d20) / denom;
			float u = 1.0f - v - w;
			return new Vector3(v, w, u);
		}

		
		[MenuItem("GameObject/Puppet3D/Skin/Bind Smooth Skin")]
		public static void BindSmoothSkinMenu()
		{
			BindSmoothSkin();
		}

		public static void DetatchSkin()
		{
			GameObject[] selection = Selection.gameObjects;
			List<GameObject> selectedMeshes = new List<GameObject>();


			foreach (GameObject Obj in selection)
			{
				if ((Obj.GetComponent<SkinnedMeshRenderer>()))
				{

					selectedMeshes.Add(Obj);

				}
			}
			
			foreach (GameObject mesh in selectedMeshes)
			{
				if (EditorUtility.DisplayDialog("Detatch Skin?", "Do you want to detatch the Skin From the bones?", "Detach", "Do Not Detach"))
				{
					SkinnedMeshRenderer smr = mesh.GetComponent<SkinnedMeshRenderer>();
					Mesh sharedMesh = null;
					if (smr)
					{
						Material[] mat = smr.sharedMaterials;
						sharedMesh = smr.sharedMesh;
						Undo.DestroyObjectImmediate(smr);
						MeshRenderer mr = mesh.AddComponent<MeshRenderer>();
						mr.sharedMaterials = mat;
					}
					MeshFilter mf = mesh.GetComponent<MeshFilter>();
					if (!mf)
					{
						mf = mesh.AddComponent<MeshFilter>();
						mf.sharedMesh = sharedMesh;

					}
				}
				
			}
			
		}
		public static GameObject BindSmoothSkin(int isGeosedic = 1)
		{
			GameObject[] selection = Selection.gameObjects;
			List<Transform> selectedBones = new List<Transform>();
			List<GameObject> selectedMeshes = new List<GameObject>();
			List<GameObject> ffdControls = new List<GameObject>();
						

			foreach (GameObject Obj in selection)
			{
				if ((Obj.GetComponent<MeshRenderer>()) || (Obj.GetComponent<SkinnedMeshRenderer>()))
				{

					selectedMeshes.Add(Obj);

				}
				else if (Obj.GetComponent<Bone>())
				{

					selectedBones.Add(Obj.transform);


				}
				else
				{
					Debug.LogWarning("Please select a mesh with a MeshRenderer");
					//return null;
				}
			}

			if (selectedBones.Count == 0)
			{
				if (selectedMeshes.Count > 0)
				{
					return null;



				}
				return null;
			}


			for (int ind = 0; ind < selectedMeshes.Count; ind++)
			{
				if (isGeosedic == 0 /*&& selectedMeshes[ind].transform.rotation == Quaternion.identity*/)
					GeosedicSkinWeights(selectedMeshes[ind], selectedBones);
				else
					EuclidianSkinWeights(selectedMeshes[ind], selectedBones, ffdControls);

			}

			if (selectedMeshes.Count > 0)
			{
				return selectedMeshes[0];
			}
			else
				return null;


		}
		static Voxel GetVoxel(Voxel[,,] voxels, Bounds bounds, Vector3 pos, int resolution)
		{
			//float height = ((bounds.size.x+ bounds.size.y + bounds.size.z)/3f);
			//float voxelSize = height/(float)resolution;
			float voxelSize = GetVoxelSize(new Vector3(bounds.size.x, bounds.size.y, bounds.size.z), resolution);


			int x = Mathf.RoundToInt((pos.x-bounds.min.x - (voxelSize*.5f))/voxelSize) +1;
			int y = Mathf.RoundToInt((pos.y-bounds.min.y - (voxelSize * .5f)) /voxelSize) + 1;
			int z = Mathf.RoundToInt((pos.z-bounds.min.z - (voxelSize * .5f)) /voxelSize) + 1;

			

			if (x > -1 && y > -1 && z > -1 && x < voxels.GetLength(0) && y < voxels.GetLength(1) && z < voxels.GetLength(2))
			{
				return voxels[x, y, z];
			}
			else
			{
				return null;
			}
		}
		static Voxel GetVoxelInRadius(Voxel[,,] voxels, Bounds bounds, Vector3 pos, float radius, int resolution)
		{
			//float height = ((bounds.size.x+ bounds.size.y + bounds.size.z)/3f);
			//float voxelSize = height/resolution;
			float voxelSize = GetVoxelSize(new Vector3(bounds.size.x, bounds.size.y, bounds.size.z), resolution);


			int x = (int)((pos.x-bounds.min.x)/voxelSize);
			int y = (int)((pos.y-bounds.min.y)/voxelSize);
			int z = (int)((pos.z-bounds.min.z)/voxelSize);


			return voxels[x,y,z];
		}

		static bool IsVoxelInside (Bounds bounds, Collider col, Vector3[] verts, float x, float y, float z, int i, GameObject cube)
		{
			Vector3 Start = new Vector3 (x, y, z);
			Vector3 End = new Vector3 (x, y, bounds.max.z);
			// check forward
			int hitNumber = 0;
			RaycastHit hit = new RaycastHit ();
			while (Physics.Linecast (Start, End, out hit) && hitNumber < 100) {
				hitNumber++;
				Vector3 dir = (End - Start).normalized;
				Start = hit.point + dir * .01f;
			}
			if (hitNumber == 100)
				Debug.LogError ("hit 100!");
			// check back
			Start = new Vector3 (x, y, bounds.max.z);
			End = new Vector3 (x, y, z);
			// check forward
			hit = new RaycastHit ();
			while (Physics.Linecast (Start, End, out hit) && hitNumber < 100) {
				hitNumber++;
				Vector3 dir = (End - Start).normalized;
				Start = hit.point + dir * .01f;
			}
			//Debug.Log(hitNumber);
			bool destroyCube = true;
			if (hitNumber % 2 == 0) 
			{
				destroyCube = true;
			}
			else
				destroyCube = false;
			return destroyCube;
		}
		static void SetInteriorVoxels(Voxel[,,] voxels, Bounds bounds, Collider col, Vector3[] verts, float x, float y, int resolution, List<Voxel> voxelList)
		{
			//float increment = bounds.size.z / 1000f;

			float frac = GetVoxelSize(new Vector3(bounds.size.x, bounds.size.y, bounds.size.z), resolution);
			//Debug.Log("x " + x + " Y " + y);
			List<Vector3> forwardHitIndexes = new List<Vector3>();
			List<Vector3> backHitIndexes = new List<Vector3>();

			Vector3 Start = new Vector3(x, y, bounds.min.z - 10f);
			Vector3 End = new Vector3(x, y, bounds.max.z + 10f);
			// check forward

			RaycastHit hit = new RaycastHit();
			while (Physics.Linecast(Start, End, out hit))
			{
				Vector3 dir = (End - Start).normalized;
				Start = hit.point + dir * 0.0001f;
				forwardHitIndexes.Add(hit.point);

								
			}

			
			// check back
			Start = new Vector3(x, y, bounds.max.z + 10f);
			End = new Vector3(x, y, bounds.min.z - 10f);
			// check forward
			
			hit = new RaycastHit();
			while (Physics.Linecast(Start, End, out hit) )
			{
				Vector3 dir = (End - Start).normalized;
				Start = hit.point + dir * 0.001f;
				backHitIndexes.Add(hit.point);
			}
			//Debug.Log("forwardHitIndexes " + forwardHitIndexes.Count + " backHitIndexes " + backHitIndexes.Count);
			int j = forwardHitIndexes.Count-1;
			
			for (int i = 0; i < forwardHitIndexes.Count; i++)
			{
				if(j< backHitIndexes.Count)
					FillLine3D(voxels, bounds, forwardHitIndexes[i] + Vector3.back * frac, backHitIndexes[j]+ Vector3.forward * frac, resolution, voxelList);

				j--;
			}
			
			
		}
		public static List<Voxel> FillLine3D(Voxel[,,] voxels, Bounds bounds, Vector3 p1, Vector3 p2, int resolution, List<Voxel> voxelList)
		{
			List<Voxel> retList = new List<Voxel>();

			//float frac = Vector3.Distance(p1,p2)/1000f;

			Voxel voxel = GetVoxel(voxels, bounds, p1, resolution);
			if (voxel != null)
			{
				voxel.Inside = true;
				voxelList.Add(voxel);
			}
			
			Voxel voxelEnd = GetVoxel(voxels, bounds, p2, resolution);





			while (voxel != null && voxel != voxelEnd)
			{
				voxel = voxels[(int)voxel.NeighbourIndexes[5].x, (int)voxel.NeighbourIndexes[5].y, (int)voxel.NeighbourIndexes[5].z];
				if (voxel != null)
				{
					voxel.Inside = true;
					voxelList.Add(voxel);
				}


			}
			
			/*Vector3 pos = p1;
			//float frac = ((bounds.size.x+ bounds.size.y + bounds.size.z)/3f)/resolution;
			float frac = GetVoxelSize(new Vector3(bounds.size.x, bounds.size.y, bounds.size.z), resolution);

			float ctr = 0;
			//Color col = new Color(Random.Range(0f,1f), Random.Range(0f,1f),Random.Range(0f,1f));
			while (Vector3.Distance(pos, p2) > 0.001f)
			{
				pos = Vector3.Lerp(p1, p2, ctr);
				ctr += frac;
				Voxel voxel = GetVoxel(voxels, bounds, pos, resolution);
				if (voxel != null)
				{
					voxel.Inside = true;
					voxelList.Add(voxel);
				}
			}*/
			return retList;

		}
		public static List<Voxel> DrawLine3D(Voxel[,,] voxels, Bounds bounds, Vector3 p1, Vector3 p2, int BoneIndex, int resolution)
		{
			List<Voxel> retList = new List<Voxel>();
			Vector3 pos = p1;
			//float frac = ((bounds.size.x+ bounds.size.y + bounds.size.z)/3f)/resolution;
			float frac = GetVoxelSize(new Vector3(bounds.size.x, bounds.size.y, bounds.size.z), resolution);

			float ctr = 0;
			//Color col = new Color(Random.Range(0f,1f), Random.Range(0f,1f),Random.Range(0f,1f));
			while(Vector3.Distance(pos, p2)>0.001f)
			{
				pos = Vector3.Lerp(p1, p2, ctr);
				ctr+=frac;
				Voxel voxel = GetVoxel(voxels, bounds, pos, resolution);
				if(voxel!=null)
				{
					voxel.Weight0 = 1f;
					voxel.Bone0 = BoneIndex;
					//voxel.GetComponent<Renderer>().material.color = col;
					retList.Add(voxel);
				}
			}
			return retList;
			
		}
		static float GetVoxelSize(Vector3 size, int resolution)
		{
			return Mathf.Pow(((size.x * size.y * size.z) / resolution), 1f/3f);
		}
		static Voxel[,,] GenerateVoxels (Bounds bounds,Collider col ,Vector3[] verts, int resolution , out List<Voxel> voxelList )
		{
			
			voxelList = new List<Voxel>();

			/*float sizeY = (bounds.size.x+ bounds.size.y + bounds.size.z)/3f;
			float voxelSize = sizeY/resolution;*/
			float voxelSize = GetVoxelSize(new Vector3(bounds.size.x , bounds.size.y , bounds.size.z) ,resolution);


			int width = (int)((bounds.max.x - bounds.min.x)/voxelSize)+2;
			int height = (int)((bounds.max.y- bounds.min.y)/voxelSize)+2;
			int depth = (int)((bounds.max.z - bounds.min.z)/voxelSize)+2;
			//Debug.Log(width + " " + height + " " + depth);

			Voxel[,,] voxels = new Voxel[width+1,height+1,depth+1];

			float x, y, z;
			EditorUtility.DisplayProgressBar("Generating Voxels for " + col.gameObject.name,"", 0);
			float total = (float)width*height*depth;
			int count = 0;
			int stepToShow = (int)(total/100f);
			float halfVoxel = voxelSize * .5f;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					
					for (int k=0;k<depth;k++)
					{
						if(count%stepToShow == 0)						
							EditorUtility.DisplayProgressBar("Generating Voxels for " + col.gameObject.name,(count + " out of " + total), (float)count/total);
						count++;
						x = bounds.min.x + voxelSize*i;
						y = bounds.min.y + voxelSize*j;
						z = bounds.min.z + voxelSize*k;
														

						voxels[i, j, k] = new Voxel();
						voxels[i,j,k].NeighbourIndexes = new Vector3[6];
						voxels[i,j,k].NeighbourIndexes[0] =i>0?new Vector3(i-1,j,k):new Vector3(0,j,k);
						voxels[i,j,k].NeighbourIndexes[1] =i<width?new Vector3(i+1,j,k):new Vector3(width-1,j,k);
						voxels[i,j,k].NeighbourIndexes[2] =j>0?new Vector3(i,j-1,k):new Vector3(i,0,k);
						voxels[i,j,k].NeighbourIndexes[3] =j<height?new Vector3(i,j+1,k):new Vector3(i,height-1,k);
						voxels[i,j,k].NeighbourIndexes[4] =k>0?new Vector3(i,j,k-1):new Vector3(i,j,0);
						voxels[i,j,k].NeighbourIndexes[5] =k<depth?new Vector3(i,j,k+1):new Vector3(i,j,depth-1);
						voxels[i,j,k].Inside = false;


						voxels[i, j, k].Pos = new Vector3(x,y, z);
						voxels[i, j, k].Scale = new Vector3(voxelSize, voxelSize, voxelSize);


					}

				}

			}
			count = 0;
			for (int i=0;i<width;i++)
			{
				for(int j=0;j<height;j++)
				{
					if (count % stepToShow == 0)
						EditorUtility.DisplayProgressBar("Generating Voxels for " + col.gameObject.name, (count + " out of " + total), (float)count / total);
					count++;
					x = bounds.min.x + voxelSize * i;
					y = bounds.min.y + voxelSize * j;
					
					SetInteriorVoxels(voxels, bounds, col, verts, x + halfVoxel, y + halfVoxel, resolution, voxelList);
					
				}

			}
			EditorUtility.ClearProgressBar();
			return voxels;
		}

		static private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
		{
			Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
			Color[] rpixels = result.GetPixels(0);
			float incX = (1.0f / (float)targetWidth);
			float incY = (1.0f / (float)targetHeight);
			for (int px = 0; px < rpixels.Length; px++)
			{
				rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
			}
			result.SetPixels(rpixels, 0);
			result.Apply();
			return result;
		}
		static void GeosedicSkinWeights(GameObject mesh, List<Transform> selectedBones)
		{
			int resolution = (int)Puppet3DEditor.VoxelScale * 1000;
			Bounds bounds;
			string sortingLayer;
			int sortingOrder;
			SkinnedMeshRenderer renderer;
			Mesh sharedMesh;
			Vector3[] verts;
			Matrix4x4[] bindPoses;
			BoneWeight[] weights;
			Material[] mat;

			SetupSkinnedMesh(mesh, selectedBones, out bounds, out sortingLayer, out sortingOrder, out renderer, out sharedMesh, out verts, out bindPoses, out weights, out mat);

			MeshCollider col = mesh.GetComponent<MeshCollider>();
			if (col == null)
			{
				col = mesh.AddComponent<MeshCollider>();
				col.sharedMesh = sharedMesh;
			}


			List<Voxel> voxelList = new List<Voxel>();
			Voxel[,,] voxels = GenerateVoxels(bounds, col, verts, resolution, out voxelList);

			Queue<Voxel> voxelsQueue = new Queue<Voxel>();

			EditorUtility.DisplayProgressBar("Queuing Voxels", "", 0);
			for (int boneIndex = 0; boneIndex < selectedBones.Count; boneIndex++)
			{
				Vector3 startPos3D = selectedBones[boneIndex].transform.position;
				//voxelsQueue.Enqueue(GetVoxel(voxels, bounds, startPos3D, resolution));
				if (selectedBones[boneIndex].childCount > 0)
				{
					for (int childIndex = 0; childIndex < selectedBones[boneIndex].childCount; childIndex++)
					{
						List<Voxel> voxelLine = DrawLine3D(voxels, bounds, startPos3D, selectedBones[boneIndex].GetChild(childIndex).position, boneIndex, resolution);
						foreach (Voxel v in voxelLine)
						{
							voxelsQueue.Enqueue(v);
						}
					}
				}
				else
				{
					// Draw Bone Point
					Voxel voxel = GetVoxel(voxels, bounds, selectedBones[boneIndex].position, resolution);
					if (voxel != null)
					{
						voxel.Weight0 = 1f;
						voxel.Bone0 = boneIndex;
						voxelsQueue.Enqueue(voxel);
					}
				}
			}
			EditorUtility.ClearProgressBar();

			EditorUtility.DisplayProgressBar("Setting Voxel weights", "", 0);

			Queue<Voxel> outsideVoxelQueue = new Queue<Voxel>();

			bool atLeastOneBoneInsideMesh = false;
			// ITERATE THROUGH VOXELS SETTING ALL BONE WEIGHTS
			while (voxelsQueue.Count > 0)
			{
				Voxel voxel = voxelsQueue.Dequeue();

				foreach (Vector3 neighbourIndex in voxel.NeighbourIndexes)
				{
					Voxel neighbour = voxels[(int)neighbourIndex.x, (int)neighbourIndex.y, (int)neighbourIndex.z];
					if (neighbour != null && neighbour.Inside && neighbour.Bone0 == -1)
					{
						neighbour.Bone0 = voxel.Bone0;
						neighbour.Weight0 = 1f;
						atLeastOneBoneInsideMesh = true;
						//neighbour.GetComponent<Renderer>().material = voxel.GetComponent<Renderer>().sharedMaterial;
						voxelsQueue.Enqueue(neighbour);
					}
					/*else if (neighbour != null && !neighbour.Inside)
					{
						neighbour.Bone0 = voxel.Bone0;
						neighbour.Weight0 = 1f;
						//outsideVoxelQueue.Enqueue(neighbour);
					}*/
				}
			}
			EditorUtility.ClearProgressBar();

			// BLUR WEIGHTS

			EditorUtility.DisplayProgressBar("Blur Voxel Weights", "", 0);
			//int blurIterations =6;
			float blurMult = Puppet3DEditor.VoxelScale / 1000f;
			if (blurMult < 1f)
				blurMult = 1f;
			//Debug.Log(blurMult);
			for (int iter = 0; iter < Puppet3DEditor.BlurIter* blurMult; iter++)
			{
				foreach (Voxel voxel in voxelList)
				{
					float total = 1f;
					float totalWeight1 = 0;
					if (voxel.Bone1 != -1)
						totalWeight1 += voxel.Weight1;
					foreach (Vector3 neighbourIndex in voxel.NeighbourIndexes)
					{
						Voxel neighbour = voxels[(int)neighbourIndex.x, (int)neighbourIndex.y, (int)neighbourIndex.z];
						if (neighbour != null && neighbour.Inside && neighbour.Bone0 != -1)
						{
							if (voxel.Bone1 == -1)
							{
								if (voxel.Bone0 != neighbour.Bone0)
								{
									voxel.Bone1 = neighbour.Bone0;
									totalWeight1 += neighbour.Weight0;
									total++;
								}
								else
								{
									voxel.Bone1 = neighbour.Bone1;
									totalWeight1 += neighbour.Weight1;
									total++;
								}
							}
							else if (voxel.Bone1 == neighbour.Bone0)
							{
								total++;
								totalWeight1 += neighbour.Weight0;
							}
							else if (voxel.Bone1 == neighbour.Bone1)
							{
								total++;
								totalWeight1 += neighbour.Weight1;
							}


						}
					}

					voxel.Weight1 = (totalWeight1 / total);
					voxel.Weight0 = 1f - voxel.Weight1;

				}
			}
			if (atLeastOneBoneInsideMesh)
			{
				foreach (Voxel voxel in voxelList)
				{
					foreach (Vector3 neighbourIndex in voxel.NeighbourIndexes)
					{
						Voxel neighbour = voxels[(int)neighbourIndex.x, (int)neighbourIndex.y, (int)neighbourIndex.z];
						if (neighbour != null && !neighbour.Inside)
						{
							neighbour.Bone0 = voxel.Bone0;
							neighbour.Weight0 = voxel.Weight0;
							neighbour.Bone1 = voxel.Bone1;
							neighbour.Weight1 = voxel.Weight1;

							outsideVoxelQueue.Enqueue(neighbour);
						}
					}
				}
			}
			EditorUtility.ClearProgressBar();

			// ITERATE THROUGH ALL OUTSIDE VOXELS
			EditorUtility.DisplayProgressBar("Setting Outside Voxel Weights", "", 0);

			while (outsideVoxelQueue.Count > 0)
			{
				Voxel voxel = outsideVoxelQueue.Dequeue();
				// If theres a weight next to it get it
				/*foreach (Vector3 neighbourIndex in voxel.NeighbourIndexes)
				{
					Voxel neighbour = voxels[(int)neighbourIndex.x, (int)neighbourIndex.y, (int)neighbourIndex.z];

					if (neighbour != null && neighbour.Inside )
					{
						voxel.Bone0 = neighbour.Bone0;
						voxel.Weight0 = neighbour.Weight0;
						voxel.Bone1 = neighbour.Bone1;
						voxel.Weight1 = neighbour.Weight1;
						break;
					}

				}*/

				foreach (Vector3 neighbourIndex in voxel.NeighbourIndexes)
				{
					Voxel neighbour = voxels[(int)neighbourIndex.x, (int)neighbourIndex.y, (int)neighbourIndex.z];

					if (neighbour != null && !neighbour.Inside && neighbour.Bone0 == -1)
					{
						neighbour.Bone0 = voxel.Bone0;
						neighbour.Weight0 = voxel.Weight0;
						neighbour.Bone1 = voxel.Bone1;
						neighbour.Weight1 = voxel.Weight1;
						outsideVoxelQueue.Enqueue(neighbour);
					}

				}
			}
			EditorUtility.ClearProgressBar();


			EditorUtility.DisplayProgressBar("copying Voxels to mesh", "", 0);
			//List<Object> gos = new List<Object>();
			voxelsQueue.Clear();
			for (int i = 0; i < verts.Length; i++)
			{
				weights[i].boneIndex0 = 0;
				weights[i].boneIndex1 = 0;
				weights[i].boneIndex2 = 0;
				weights[i].boneIndex3 = 0;
				weights[i].weight0 = 1;
				weights[i].weight1 = 0;
				weights[i].weight2 = 0;
				weights[i].weight3 = 0;

				Vector3 vert = mesh.transform.TransformPoint(verts[i]);
				Voxel v = GetVoxel(voxels, bounds, vert, resolution);
				List<Voxel> UsedVoxels = new List<Voxel>();
				if (v != null)
				{
					
					if (v.Bone0 == -1)
					{
						

						voxelsQueue.Enqueue(v);
						int searchDepth = 0;
						while (voxelsQueue.Count > 0 && searchDepth < 200)
						{
							Voxel voxel = voxelsQueue.Dequeue();

							foreach (Vector3 neighbourIndex in voxel.NeighbourIndexes)
							{
								Voxel neighbour = voxels[(int)neighbourIndex.x, (int)neighbourIndex.y, (int)neighbourIndex.z];
								if (neighbour != null && !UsedVoxels.Contains(neighbour))
								{
									if (neighbour.Inside && neighbour.Bone0 != -1)
									{
										v = neighbour;
										voxelsQueue.Clear();
										break;
									}
									else
									{
										UsedVoxels.Add(neighbour);
										voxelsQueue.Enqueue(neighbour);
									}

								}

							}
							searchDepth++;
						}

					}


				}
				

				if (v != null)
				{
					if (v.Bone0 > -1)
					{
						weights[i].boneIndex0 = v.Bone0;
						weights[i].weight0 = v.Weight0;
					}
					if (v.Bone1 > -1)
					{
						weights[i].boneIndex1 = v.Bone1;
						weights[i].weight1 = v.Weight1;

					}
				}

			}

			EditorUtility.ClearProgressBar();
			GameObject VoxelsGO = null;

			GameObject VoxelsGOParent = null;
			if (Puppet3DEditor.KeepVoxels)
			{
				VoxelsGOParent = new GameObject();
				VoxelsGOParent.name = "Voxels";
				Undo.RegisterCreatedObjectUndo(VoxelsGOParent, "voxel");
			}

			MeshFilter voxMF = null;
			Mesh voxMesh = null;
			List<Vector3> vertList = new List<Vector3>();
			List<int> triList = new List<int>();
			List<Color> colorList = new List<Color>();
			int indexVoxel = 0;
			CreateNewVoxelModel(ref VoxelsGOParent, ref VoxelsGO, ref voxMF, ref voxMesh, ref indexVoxel);

			// Make Proxy Cube
			GameObject cubeTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Mesh cubeMesh = cubeTemp.GetComponent<MeshFilter>().sharedMesh;

			DestroyImmediate(cubeTemp);

			EditorUtility.DisplayProgressBar("Creating Voxel models", "", 0);

			int index = 0;
			foreach (Voxel vox in voxels)
			{
				if (vertList.Count + cubeMesh.vertexCount > 65000)
				{
					CompleteVoxelModel(voxMF, voxMesh, vertList, triList, colorList);
					indexVoxel++;
					index = 0;
					CreateNewVoxelModel(ref VoxelsGOParent, ref VoxelsGO, ref voxMF, ref voxMesh, ref indexVoxel);

				}
				if (Puppet3DEditor.KeepVoxels)
				{
					if (vox != null && vox.Inside)
					{
						float hue = ((float)vox.Bone0 / selectedBones.Count);
						float hue2 = ((float)vox.Bone1 / selectedBones.Count);
						Color Color1 = (Color.HSVToRGB(hue * 1f, 1f, 1f));//+  HsvToRgb(hue2*255f, 255, 255)*boneWeights[i].weight1 );
						Color Color2 = (Color.HSVToRGB(hue2 * 1f, 1f, 1f));//+  HsvToRgb(hue2*255f, 255, 255)*boneWeights[i].weight1 );	

						/*Mesh newMesh = new Mesh();
						newMesh.vertices = cubeMesh.vertices;
						newMesh.triangles = cubeMesh.triangles;
						Color[] cols = new Color[newMesh.vertices.Length];
						for (int c = 0; c < cols.Length; c++)
						{

							cols[c] = Color.Lerp(Color2, Color1, vox.Weight0) ;
						}

						newMesh.colors = cols;

						vox.gameObject.AddComponent<MeshFilter>().sharedMesh = newMesh;
						MeshRenderer cubeMR = vox.gameObject.AddComponent<MeshRenderer>();
						cubeMR.sharedMaterial = new Material(Shader.Find("Puppet3D/vertColor"));
						*/						
						for (int i = 0; i < cubeMesh.vertexCount; i++)
						{
							Vector3 newPos = vox.Pos + vox.Scale.x * cubeMesh.vertices[i];
							vertList.Add(newPos);
							colorList.Add(Color.Lerp(Color2, Color1, vox.Weight0));
						}
						for (int i = 0; i < cubeMesh.triangles.Length; i++)
						{
							triList.Add(index + cubeMesh.triangles[i]);
						}

						index += 24;
					}
				}


			}
			CompleteVoxelModel(voxMF, voxMesh, vertList, triList, colorList);
			EditorUtility.ClearProgressBar();

			renderer.quality = SkinQuality.Bone2;

			FinishSkinnedMesh(mesh, selectedBones, bounds, sortingLayer, sortingOrder, renderer, sharedMesh, bindPoses, weights, mat);


			renderer.sharedMaterials = mat;


			return;

		}

		private static void CompleteVoxelModel(MeshFilter voxMF, Mesh voxMesh, List<Vector3> vertList, List<int> triList, List<Color> colorList)
		{
			if (Puppet3DEditor.KeepVoxels)
			{
				voxMesh.vertices = vertList.ToArray();
				voxMesh.colors = colorList.ToArray();
				voxMesh.triangles = triList.ToArray();
				voxMF.sharedMesh = voxMesh;
				vertList.Clear();
				colorList.Clear();
				triList.Clear();
			}
		}

		private static void CreateNewVoxelModel(ref GameObject VoxelsGOParent, ref GameObject VoxelsGO, ref MeshFilter voxMF, ref Mesh voxMesh, ref int index)
		{
			if (Puppet3DEditor.KeepVoxels)
			{
				VoxelsGO = new GameObject();
				VoxelsGO.transform.parent = VoxelsGOParent.transform;
				Undo.RegisterCreatedObjectUndo(VoxelsGO, "voxel");
				VoxelsGO.name = "Voxels";
				voxMF = VoxelsGO.AddComponent<MeshFilter>();
				voxMesh = new Mesh();
				voxMesh.name = index.ToString();
				MeshRenderer cubeMR = VoxelsGO.AddComponent<MeshRenderer>();
				cubeMR.sharedMaterial = new Material(Shader.Find("Puppet3D/vertColor"));

			}
		}

		static int[] GetFirstSecondBone(int vertIndex, float[,] weights)
		{
			float weightCheck = 9999999f;
			int[] boneReturn = new int[2];
			boneReturn[0] = -1;
			boneReturn[1] = -1;

			for (int i = 0; i < weights.GetLength(0); i++)
			{

				if (weights[i, vertIndex] >= 0 && weightCheck >= weights[i, vertIndex])
				{
					weightCheck = weights[i, vertIndex];
					boneReturn[0] = i;

				}

			}
			float weightCheck2 = 999999f;
			for (int i = 0; i < weights.GetLength(0); i++)
			{
				if (boneReturn[0] != i)
				{
					if (weights[i, vertIndex] >= 0 && weightCheck2 >= weights[i, vertIndex])
					{
						weightCheck2 = weights[i, vertIndex];
						boneReturn[1] = i;

					}
				}

			}
			if (boneReturn[0] == -1)
			{
				boneReturn[0] = 0;

				Debug.LogError("Your image might have some rough edges - this has caused a few errors in the default skinning");

			}
			if (boneReturn[1] == -1)
				boneReturn[1] = 0;

			return boneReturn;
		}

		public static List<Vector2> DrawLine(Vector2 p1, Vector2 p2)
		{
			List<Vector2> retList = new List<Vector2>();
			Vector2 t = p1;
			float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
			float ctr = 0;

			while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
			{
				t = Vector2.Lerp(p1, p2, ctr);
				ctr += frac;
				retList.Add(new Vector2(t.x, t.y));
				//Debug.Log(t.x + " " + t.y);
			}
			return retList;
		}

		static Vector2 WorldPosToXY(Vector3 worldPos, Bounds bounds, int arrayWidth, int arrayHeight)
		{

			Vector3 localPos = worldPos;
			localPos.x = (localPos.x - bounds.min.x) / bounds.size.x;
			localPos.y = (localPos.y - bounds.min.y) / bounds.size.y;

			float x = localPos.x * arrayWidth;
			float y = localPos.y * arrayHeight; ;

			return new Vector2((int)x, (int)y);
		}
		static int WorldPosToIndex(Vector3 worldPos, Bounds bounds, int arrayWidth, int arrayHeight)
		{

			Vector3 localPos = worldPos;
			localPos.x = (localPos.x - bounds.min.x) / bounds.size.x;
			localPos.y = (localPos.y - bounds.min.y) / bounds.size.y;

			float x = localPos.x * arrayWidth;
			float y = localPos.y * arrayHeight; ;


			int index = (int)(y) * arrayWidth + (int)x;

			

			return index;
		}
		public static void SetTextureImporterFormat(Texture2D texture, bool isReadable)
		{
			if (null == texture) return;

			string assetPath = AssetDatabase.GetAssetPath(texture);
			var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (tImporter != null)
			{

				tImporter.isReadable = isReadable;

				AssetDatabase.ImportAsset(assetPath);
				AssetDatabase.Refresh();
			}
		}
		public static void SortVertices(GameObject gameObject = null)
		{
			if (gameObject == null)
				gameObject = Selection.activeGameObject;
			if (Selection.activeGameObject == null)
				return;
			SkinnedMeshRenderer smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
			if (smr == null)
				return;
			Mesh mesh = smr.sharedMesh;
			SpriteRenderer[] bones = new SpriteRenderer[smr.bones.Length];
			for (int c = 0; c < bones.Length; c++)
				bones[c] = smr.bones[c].GetComponent<SpriteRenderer>();

			int[] tris = mesh.triangles;

			tris = BubbleSort(tris, bones, mesh);


			mesh.triangles = tris;


			Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
		}
		public static int[] BubbleSort(int[] tris, SpriteRenderer[] bones, Mesh mesh)
		{

			int i, j;
			int N = tris.Length / 3;
			for (j = N - 1; j > 0; j--)
			{
				for (i = 0; i < j; i++)
				{
					if (bones[mesh.boneWeights[tris[i * 3]].boneIndex0].sortingOrder > bones[mesh.boneWeights[tris[((i * 3) + 3)]].boneIndex0].sortingOrder)
					{
						tris = exchange(tris, i * 3, ((i * 3) + 3));
					}
				}
			}

			return tris;
		}
		public static int[] exchange(int[] data, int m, int n)
		{

			int temporary1 = data[m];
			int temporary2 = data[m + 1];
			int temporary3 = data[m + 2];

			data[m] = data[n];
			data[m + 1] = data[n + 1];
			data[m + 2] = data[n + 2];

			data[n] = temporary1;
			data[n + 1] = temporary2;
			data[n + 2] = temporary3;

			return data;
		}
		static void EuclidianSkinWeights(GameObject mesh, List<Transform> selectedBones, List<GameObject> ffdControls)
		{
			Bounds SortingLayerBounds;
			string sortingLayer;
			int sortingOrder;
			SkinnedMeshRenderer renderer;
			Mesh sharedMesh;
			Vector3[] verts;
			Matrix4x4[] bindPoses;
			BoneWeight[] weights;
			Material[] mat;

			SetupSkinnedMesh(mesh, selectedBones, out SortingLayerBounds, out sortingLayer, out sortingOrder, out renderer, out sharedMesh, out verts, out bindPoses, out weights, out mat);

			int index = 0;
			int index2 = 0;
			for (int j = 0; j < weights.Length; j++)
			{
				float testdist = 1000000;
				float testdist2 = 1000000;
				for (int i = 0; i < selectedBones.Count; i++)
				{
					Vector3 worldPt = mesh.transform.TransformPoint(verts[j]);
					Vector3 checkPos = selectedBones[i].position;
					if (selectedBones[i].childCount > 0)
					{
						checkPos += selectedBones[i].GetChild(0).position;
						checkPos /= 2f;
					}

					float dist = Vector3.Distance(checkPos, worldPt);
					if (dist < testdist)
					{
						testdist = dist;
						index = selectedBones.IndexOf(selectedBones[i]);
					}
				}
				
				index2 = index;
				float weight1 = 1;

				if (selectedBones[index].parent && selectedBones.Contains(selectedBones[index].parent))
				{
					Vector3 worldPt = mesh.transform.TransformPoint(verts[j]);

					index2 = selectedBones.IndexOf(selectedBones[index].parent);

					Vector3 checkPos = selectedBones[index].position;

					checkPos += selectedBones[index].parent.position;
					checkPos /= 2f;


					testdist2 = Vector3.Distance(checkPos, worldPt);
					float combinedDistance = testdist / (testdist + testdist2);

					weight1 = 1 - (combinedDistance);

				}


			
				if (Puppet3DEditor._numberBonesToSkinToIndex == 1)
				{
					renderer.quality = SkinQuality.Bone2;
					weights[j].boneIndex0 = index;
					weights[j].weight0 = weight1;
					weights[j].boneIndex1 = index2;
					weights[j].weight1 = 1 - weight1;
				}
				else
				{
					renderer.quality = SkinQuality.Bone1;
					weights[j].boneIndex0 = index;
					weights[j].weight0 = 1;
				}
			}
			FinishSkinnedMesh(mesh, selectedBones, SortingLayerBounds, sortingLayer, sortingOrder, renderer, sharedMesh, bindPoses, weights, mat);

		}

		static void SetupSkinnedMesh(GameObject mesh, List<Transform> selectedBones, out Bounds bounds, out string sortingLayer, out int sortingOrder, out SkinnedMeshRenderer renderer, out Mesh sharedMesh, out Vector3[] verts, out Matrix4x4[] bindPoses, out BoneWeight[] weights, out Material[] mat)
		{
			mat = null;
			sortingLayer = "";
			sortingOrder = 0;
			bounds = new Bounds();
			if (mesh.GetComponent<MeshRenderer>() != null)
            {
                mat = mesh.GetComponent<MeshRenderer>().sharedMaterials;
                sortingLayer = mesh.GetComponent<Renderer>().sortingLayerName;
                sortingOrder = mesh.GetComponent<Renderer>().sortingOrder;
                bounds = mesh.GetComponent<MeshRenderer>().bounds;

                Undo.DestroyObjectImmediate(mesh.GetComponent<MeshRenderer>());
            }
            renderer = mesh.GetComponent<SkinnedMeshRenderer>();
            if (renderer == null)
            {
                renderer = Undo.AddComponent<SkinnedMeshRenderer>(mesh);

            }
            else
            {
                bounds = renderer.bounds;

                mat = renderer.sharedMaterials;
                bounds = renderer.bounds;

                Undo.DestroyObjectImmediate(renderer);
                renderer = Undo.AddComponent<SkinnedMeshRenderer>(mesh);

            }
			renderer.updateWhenOffscreen = true;
			
			sharedMesh = SaveFBXMesh(mesh.transform.GetComponent<MeshFilter>().sharedMesh);
			mesh.transform.GetComponent<MeshFilter>().sharedMesh = sharedMesh;
			verts = sharedMesh.vertices;
			bindPoses = new Matrix4x4[selectedBones.Count];
			List<Transform> closestBones = new List<Transform>();
			closestBones.Clear();
			weights = new BoneWeight[verts.Length];
			for (int i = 0; i < selectedBones.Count; i++)
			{
				Transform bone = selectedBones[i];
				bindPoses[i] = bone.worldToLocalMatrix * mesh.transform.localToWorldMatrix;
			}

		}

		static void FinishSkinnedMesh(GameObject mesh, List<Transform> selectedBones, Bounds SortingLayerBounds, string sortingLayer, int sortingOrder, SkinnedMeshRenderer renderer, Mesh sharedMesh, Matrix4x4[] bindPoses, BoneWeight[] weights, Material[] mat)
		{
			
			sharedMesh.boneWeights = weights;
			sharedMesh.bindposes = bindPoses;
			renderer.bones = selectedBones.ToArray();
			if (sharedMesh.colors.Length == 0)
			{
				Color[] newColors = new Color[sharedMesh.vertices.Length];
				for (int i = 0; i < sharedMesh.vertices.Length; i++)
				{
					newColors[i] = new Color(1f, 1f, 1f, 1f);
				}
				sharedMesh.colors = newColors;
			}
			else
			{
				Color[] newColors = new Color[sharedMesh.vertices.Length];
				for (int i = 0; i < sharedMesh.vertices.Length; i++)
				{
					newColors[i] = sharedMesh.colors[i];
					/*if (sharedMesh.colors[i] !=null)
						newColors[i] = sharedMesh.colors[i];
					else
						newColors[i] = new Color(1f, 1f, 1f, 1f);*/
				}
				sharedMesh.colors = newColors;
			}

			renderer.sharedMesh = sharedMesh;
			if (mat.Length!=0)
				renderer.sharedMaterials = mat;
			renderer.sortingLayerName = sortingLayer;
			renderer.sortingOrder = sortingOrder;
			
			EditorUtility.SetDirty(mesh);
			EditorUtility.SetDirty(sharedMesh);
			AssetDatabase.SaveAssets();
			AssetDatabase.SaveAssets();
			
		}
		public static Mesh SaveFBXMesh(Mesh mesh, bool Duplicate = false)
		{
			string path = AssetDatabase.GetAssetPath(mesh);
			string extension = Path.GetExtension(path);
			//Debug.Log("extension is " + extension);

			if (extension == ".asset" && !Duplicate )
			{
				return mesh;
			}
			else
			{
				string[] pathSplit = path.Split('/');
				string meshPath = "";
				for (int i = 0; i < pathSplit.Length - 1; i++)
				{
					meshPath += pathSplit[i] + "/";
				}
				if (meshPath == "")
					meshPath = "Assets/";

				
				string outMeshPath = meshPath + mesh.name + "P3D.asset";

				outMeshPath = AssetDatabase.GenerateUniqueAssetPath(outMeshPath);


				//Debug.Log("path is " + outMeshPath);
				Mesh newMesh = new Mesh();
				newMesh.vertices = mesh.vertices;
				newMesh.colors = mesh.colors;
				newMesh.normals = mesh.normals;
				newMesh.uv = mesh.uv;
				newMesh.bindposes = mesh.bindposes;
				newMesh.boneWeights = mesh.boneWeights;
				newMesh.tangents = mesh.tangents;
				newMesh.subMeshCount = mesh.subMeshCount;
				for (int index = 0; index < mesh.subMeshCount ; index++)
				{
					newMesh.SetTriangles(mesh.GetTriangles(index), index);
				}

				AssetDatabase.CreateAsset(newMesh, outMeshPath);
				Debug.Log("Saving mesh into " + outMeshPath);
				return AssetDatabase.LoadAssetAtPath(outMeshPath, typeof(Mesh)) as Mesh;
			}
		}
		[MenuItem("GameObject/Puppet3D/Skin/Edit Skin Weights")]
		public static bool EditWeights()
		{
			GameObject[] selection = Selection.gameObjects;

			foreach (GameObject sel in selection)
			{
				if ((sel.GetComponent<Bakedmesh>() != null))
				{
					Debug.LogWarning("Already in edit mode");
					return false;
				}
				if ((sel.GetComponent<SkinnedMeshRenderer>()))
				{
					SkinnedMeshRenderer renderer = sel.GetComponent<SkinnedMeshRenderer>();
					Undo.RecordObject(sel, "add mesh to meshes being editted");
					Undo.AddComponent<Bakedmesh>(sel);
					Mesh mesh = sel.GetComponent<MeshFilter>().sharedMesh;


					Vector3[] verts = mesh.vertices;
					BoneWeight[] boneWeights = mesh.boneWeights;

					for (int i = 0; i < verts.Length; i++)
					{
						Vector3 vert = verts[i];
						Vector3 vertPos = sel.transform.TransformPoint(vert);
						GameObject handle = new GameObject("vertex" + i);
						handle.transform.position = vertPos;
						handle.transform.parent = sel.transform;

						handle.AddComponent<VertexWeight>();
						SkinWeightControl editSkinWeights = handle.AddComponent<SkinWeightControl>();

						editSkinWeights.verts = mesh.vertices;

						editSkinWeights.Weight0 = boneWeights[i].weight0;
						editSkinWeights.Weight1 = boneWeights[i].weight1;
						editSkinWeights.Weight2 = boneWeights[i].weight2;
						editSkinWeights.Weight3 = boneWeights[i].weight3;

						if (boneWeights[i].weight0 > 0)
						{
							editSkinWeights.Bone0 = renderer.bones[boneWeights[i].boneIndex0].gameObject;
							editSkinWeights.boneIndex0 = boneWeights[i].boneIndex0;
						}
						else
							editSkinWeights.Bone0 = null;

						if (boneWeights[i].weight1 > 0)
						{
							editSkinWeights.Bone1 = renderer.bones[boneWeights[i].boneIndex1].gameObject;
							editSkinWeights.boneIndex1 = boneWeights[i].boneIndex1;
						}
						else
						{
							editSkinWeights.Bone1 = null;
							editSkinWeights.boneIndex1 = renderer.bones.Length;
						}

						if (boneWeights[i].weight2 > 0)
						{
							editSkinWeights.Bone2 = renderer.bones[boneWeights[i].boneIndex2].gameObject;
							editSkinWeights.boneIndex2 = boneWeights[i].boneIndex2;
						}
						else
						{
							editSkinWeights.Bone2 = null;
							editSkinWeights.boneIndex2 = renderer.bones.Length;
						}

						if (boneWeights[i].weight3 > 0)
						{
							editSkinWeights.Bone3 = renderer.bones[boneWeights[i].boneIndex3].gameObject;
							editSkinWeights.boneIndex3 = boneWeights[i].boneIndex3;
						}
						else
						{
							editSkinWeights.Bone3 = null;
							editSkinWeights.boneIndex3 = renderer.bones.Length;
						}

						editSkinWeights.mesh = mesh;
						editSkinWeights.meshRenderer = renderer;
						editSkinWeights.vertNumber = i;
					}

				}
				else
				{
					Debug.LogWarning("Selection does not have a meshRenderer");
					return false;
				}


			}
			return true;
		}

		[MenuItem("GameObject/Puppet3D/Skin/Finish Editting Skin Weights")]
		public static Object[] FinishEditingWeights()
		{
			SpriteRenderer[] sprs = FindObjectsOfType<SpriteRenderer>();
			Bakedmesh[] skinnedMeshesBeingEditted = FindObjectsOfType<Bakedmesh>();
			List<Object> returnObjects = new List<Object>();
			foreach (SpriteRenderer spr in sprs)
			{
				if (spr.sprite)
					if (spr.sprite.name.Contains("Bone"))
						spr.gameObject.GetComponent<SpriteRenderer>().color = Color.white;

			}
			foreach (Bakedmesh bakedMesh in skinnedMeshesBeingEditted)
			{
				GameObject sel = bakedMesh.gameObject;
				returnObjects.Add(sel);

				DestroyImmediate(bakedMesh);

				int numberChildren = sel.transform.childCount;
				List<GameObject> vertsToDestroy = new List<GameObject>();
				for (int i = 0; i < numberChildren; i++)
				{
					vertsToDestroy.Add(sel.transform.GetChild(i).gameObject);


				}
				foreach (GameObject vert in vertsToDestroy)
					DestroyImmediate(vert);
			}
			return returnObjects.ToArray();
		}

		static Mesh SmoothSkinWeights(Mesh sharedMesh)
		{
			//        Debug.Log("smoothing skin weights");
			int[] triangles = sharedMesh.GetTriangles(0);
			BoneWeight[] boneWeights = sharedMesh.boneWeights;

			for (int i = 0; i < triangles.Length; i += 3)
			{
				BoneWeight v1 = boneWeights[triangles[i]];
				BoneWeight v2 = boneWeights[triangles[i + 1]];
				BoneWeight v3 = boneWeights[triangles[i + 2]];

				List<int> v1Bones = new List<int>(new int[] { v1.boneIndex0, v1.boneIndex1, v1.boneIndex2, v1.boneIndex3 });
				List<int> v2Bones = new List<int>(new int[] { v2.boneIndex0, v2.boneIndex1, v2.boneIndex2, v2.boneIndex3 });
				List<int> v3Bones = new List<int>(new int[] { v3.boneIndex0, v3.boneIndex1, v3.boneIndex2, v3.boneIndex3 });

				List<float> v1Weights = new List<float>(new float[] { v1.weight0, v1.weight1, v1.weight2, v1.weight3 });
				List<float> v2Weights = new List<float>(new float[] { v2.weight0, v2.weight1, v2.weight2, v2.weight3 });
				List<float> v3Weights = new List<float>(new float[] { v3.weight0, v3.weight1, v3.weight2, v3.weight3 });


				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						if (v1Bones[j] == v2Bones[k])
						{
							for (int l = 0; l < 2; l++)
							{
								if (v1Bones[j] == v3Bones[l])
								{

									v1Weights[j] = (v1Weights[j] + v2Weights[k] + v3Weights[l]) / 3;
									v2Weights[k] = (v1Weights[j] + v2Weights[k] + v3Weights[l]) / 3;
									v3Weights[l] = (v1Weights[j] + v2Weights[k] + v3Weights[l]) / 3;


								}
							}
						}
					}

				}
				boneWeights[triangles[i]].weight0 = v1Weights[0];
				boneWeights[triangles[i]].weight1 = v1Weights[1];


				boneWeights[triangles[i + 1]].weight0 = v2Weights[0];
				boneWeights[triangles[i + 1]].weight1 = v2Weights[1];


				boneWeights[triangles[i + 2]].weight0 = v3Weights[0];
				boneWeights[triangles[i + 2]].weight1 = v3Weights[1];


			}
			sharedMesh.boneWeights = boneWeights;
			return sharedMesh;
		}

		public static void DrawHandle(Ray ray)
		{
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Handles.color = Color.white;

				Handles.DrawWireDisc(hit.point, hit.normal, Puppet3DEditor.EditSkinWeightRadius);

				Handles.color =new Color( Puppet3DEditor.paintControlColor.r, Puppet3DEditor.paintControlColor.g, Puppet3DEditor.paintControlColor.b, Puppet3DEditor.paintWeightsStrength);

				Handles.DrawSolidDisc(hit.point, hit.normal, Puppet3DEditor.EditSkinWeightRadius );
				SceneView.RepaintAll();
			}

		}

		public static void PaintWeights(Ray ray, float weightStrength)
		{
			
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				
				Vector3[] vertices = Puppet3DEditor.currentSelectionMesh.vertices;
				Vector3[] normals = Puppet3DEditor.currentSelectionMesh.normals;

				Color[] colrs = Puppet3DEditor.currentSelectionMesh.colors;
				BoneWeight[] boneWeights = Puppet3DEditor.currentSelectionMesh.boneWeights;

				Vector3 pos = Puppet3DEditor.currentSelection.transform.InverseTransformPoint(hit.point);
				//Undo.RecordObject(Puppet3DEditor.currentSelectionMesh, "Weight paint");
				pos = new Vector3(pos.x, pos.y, pos.z);

				SkinnedMeshRenderer smr = Puppet3DEditor.currentSelection.GetComponent<SkinnedMeshRenderer>();
				int boneIndex = smr.bones.ToList().IndexOf(Puppet3DEditor.paintWeightsBone.transform);

				if (boneIndex < 0)
				{
					Debug.LogWarning(Puppet3DEditor.paintWeightsBone.name + " is not connected to skin");
					return;
				}
				Vector3 hitNormal = hit.normal;
				for (int i = 0; i < vertices.Length; i++)
				{
					
					if (boneWeights[i].boneIndex0 < 0)
						boneWeights[i].boneIndex0 = 0;
					if (boneWeights[i].boneIndex1 < 0)
						boneWeights[i].boneIndex1 = 0;

					

					float sqrMagnitude = (vertices[i] - pos).magnitude ;
					if (sqrMagnitude > Puppet3DEditor.EditSkinWeightRadius)
						continue;

					float vertDir =( Vector3.Dot(normals[i], hitNormal));
					if (vertDir < 0.5)
						continue;
					float weightFloat = Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength;


					if (boneWeights[i].boneIndex0 == boneIndex)
					{
						if (weightStrength > 0)
							boneWeights[i].weight0 += weightFloat;
						else
							boneWeights[i].weight0 -= weightFloat;

						boneWeights[i].weight0 = Mathf.Clamp01(boneWeights[i].weight0);
						boneWeights[i].weight1 = 1 - boneWeights[i].weight0;
						colrs[i] = new Color(boneWeights[i].weight0, boneWeights[i].weight0, boneWeights[i].weight0, 1);
						

					}
					else if (boneWeights[i].boneIndex1 == boneIndex)
					{
						if (weightStrength > 0)
							boneWeights[i].weight1 += weightFloat;
						else
							boneWeights[i].weight1 -= weightFloat;

						boneWeights[i].weight1 = Mathf.Clamp01(boneWeights[i].weight1);
						boneWeights[i].weight0 = 1 - boneWeights[i].weight1;
						colrs[i] = new Color(boneWeights[i].weight1, boneWeights[i].weight1, boneWeights[i].weight1, 1);

						//boneWeights[i].weight1 = colrs[i].r;
						//boneWeights[i].weight0 = 1-colrs[i].r;


					}
					else //if (weightFloat != 0 || boneWeights[i].weight1 + boneWeights[i].weight2 + boneWeights[i].weight3 > 0)
					{
						if (boneWeights[i].weight0 < boneWeights[i].weight1)
						{

							if (weightStrength > 0)
								boneWeights[i].weight0 += weightFloat;
							else
								boneWeights[i].weight0 -= weightFloat;

							boneWeights[i].weight0 = Mathf.Clamp01(boneWeights[i].weight0);
							//boneWeights[i].weight1 = 1 - boneWeights[i].weight0;
							colrs[i] = new Color(boneWeights[i].weight0, boneWeights[i].weight0, boneWeights[i].weight0, 1);

							boneWeights[i].boneIndex0 = boneIndex;


						}
						else
						{
							if (weightStrength > 0)
								boneWeights[i].weight1 += weightFloat;
							else
								boneWeights[i].weight1 -= weightFloat;

							boneWeights[i].weight1 = Mathf.Clamp01(boneWeights[i].weight1);
							//boneWeights[i].weight0 = 1 - boneWeights[i].weight1;
							colrs[i] = new Color(boneWeights[i].weight1, boneWeights[i].weight1, boneWeights[i].weight1, 1);

							boneWeights[i].boneIndex1 = boneIndex;


						}
					}
					

				}				
				Puppet3DEditor.currentSelectionMesh.boneWeights = boneWeights;
				if (Puppet3DEditor.BlackAndWhiteWeights)
					Puppet3DEditor.currentSelectionMesh.colors = colrs;
				else
					Puppet3DEditor.currentSelectionMesh.colors = SetColors(boneWeights);
			}

			//		 
			
		}

		public static Color[] SetColors(BoneWeight[] boneWeights)
		{
			SkinnedMeshRenderer smr = Puppet3DEditor.currentSelection.GetComponent<SkinnedMeshRenderer>();



			Color[] colrs = Puppet3DEditor.currentSelectionMesh.colors;



			//Shader.SetGlobalInt("_BonesCount", smr.bones);

			for (int i = 0; i < boneWeights.Length; i++)
			{
				float hue = ((float)boneWeights[i].boneIndex0 / smr.bones.Length);
				float hue2 = ((float)boneWeights[i].boneIndex1 / smr.bones.Length);
				//hue /= 2f;

				Color Color1 = (Color.HSVToRGB(hue * 1f, 1f, 1f));//+  HsvToRgb(hue2*255f, 255, 255)*boneWeights[i].weight1 );
				Color Color2 = (Color.HSVToRGB(hue2 * 1f, 1f, 1f));//+  HsvToRgb(hue2*255f, 255, 255)*boneWeights[i].weight1 );

				colrs[i] = Color.Lerp(Color2, Color1, boneWeights[i].weight0 / (boneWeights[i].weight0 + boneWeights[i].weight1));


				//colrs[i] += new Color ((float)boneWeights[i].boneIndex1/smr.bones.Length, (float)boneWeights[i].boneIndex1/smr.bones.Length, boneWeights[i].boneIndex1/smr.bones.Length, 1);
				//   colrs[i] /= 2f;

				//Shader.SetGlobalColor("_Bones" + i.ToString(), new Color(i/ smr.bones.Length,i/ smr.bones.Length,i/ smr.bones.Length,1));


			}

			return colrs;
		}
		static public List<int> GetNeighbors(int[] triangles, int index)
		{
			List<int> verts = new List<int>();

			for (int i = 0; i < triangles.Length / 3; i++)
			{
				// see if the triangle contains the index

				bool found = false;
				for (int j = 0; j < 3; j++)
				{
					int cur = triangles[i * 3 + j];
					if (cur == index) found = true;
				}
				// if we found the index in the triangle, append the others.
				if (found)
				{
					for (int j = 0; j < 3; j++)
					{
						int cur = triangles[i * 3 + j];
						if (verts.IndexOf(cur) == -1 && cur != index)
						{
							verts.Add(cur);
						}
					}
				}
			}
			return verts;
		}
		public static void PaintSmoothWeights(Ray ray)
		{
			//		Debug.Log("Paint Smooth Weights");
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Vector3[] vertices = Puppet3DEditor.currentSelectionMesh.vertices;
				Color[] colrs = Puppet3DEditor.currentSelectionMesh.colors;
				BoneWeight[] boneWeights = Puppet3DEditor.currentSelectionMesh.boneWeights;

				Vector3 pos = Puppet3DEditor.currentSelection.transform.InverseTransformPoint(hit.point);
				//Undo.RecordObject(Puppet3DEditor.currentSelectionMesh, "Weight paint");
				pos = new Vector3(pos.x, pos.y, pos.z);

				SkinnedMeshRenderer smr = Puppet3DEditor.currentSelection.GetComponent<SkinnedMeshRenderer>();
				int boneIndex = smr.bones.ToList().IndexOf(Puppet3DEditor.paintWeightsBone.transform);

				if (boneIndex < 0)
				{
					Debug.LogWarning(Puppet3DEditor.paintWeightsBone.name + " is not connected to skin");
					return;
				}
				List<int> vertsInCircle = new List<int>();

				List<float> averageWeights = new List<float>();

				List<int> trianglesInRange = new List<int>();
				for (int i = 0; i < smr.sharedMesh.triangles.Count(); i++)
				{
					float sqrMagnitude = (vertices[smr.sharedMesh.triangles[i]] - pos).magnitude;
					if (sqrMagnitude > Puppet3DEditor.EditSkinWeightRadius)
						continue;

					trianglesInRange.Add(smr.sharedMesh.triangles[i]);

				}
				for (int i = 0; i < vertices.Length; i++)
				{


					float sqrMagnitude = (vertices[i] - pos).magnitude;
					if (sqrMagnitude > Puppet3DEditor.EditSkinWeightRadius)
						continue;



					List<int> connectedVert = GetNeighbors(trianglesInRange.ToArray(), i);

					float combinedWeights = 0f;
					int numberConnectedVerts = 1;

					if (boneWeights[i].boneIndex0 == boneIndex)
					{
						combinedWeights += boneWeights[i].weight0;


					}
					else if (boneWeights[i].boneIndex1 == boneIndex)
					{

						combinedWeights += boneWeights[i].weight1;

					}

					foreach (int vert in connectedVert)
					{
						sqrMagnitude = (vertices[vert] - pos).magnitude;
						if (sqrMagnitude > Puppet3DEditor.EditSkinWeightRadius)
							continue;

						numberConnectedVerts++;
						if (boneWeights[vert].boneIndex0 == boneIndex)
						{
							combinedWeights += boneWeights[vert].weight0;


						}
						else if (boneWeights[vert].boneIndex1 == boneIndex)
						{

							combinedWeights += boneWeights[vert].weight1;

						}
					}
					if (numberConnectedVerts != 0)
					{

						vertsInCircle.Add(i);
						combinedWeights /= numberConnectedVerts;
						averageWeights.Add(combinedWeights);

						//				Debug.Log ("vert " + i + " has " + numberConnectedVerts + " connected verts, with new combined weight of  " + combinedWeights );
					}

				}



				//Debug.Log ("number verts " + vertsInCircle.Count + " combined weights " + combinedWeights);
				for (int j = 0; j < vertsInCircle.Count; j++)
				{
					int i = vertsInCircle[j];


					float newWeight = Mathf.Lerp(colrs[i].r, averageWeights[j], Puppet3DEditor.paintWeightsStrength);


					if (boneWeights[i].boneIndex0 < 0)
						boneWeights[i].boneIndex0 = 0;
					if (boneWeights[i].boneIndex1 < 0)
						boneWeights[i].boneIndex1 = 0;

					//boneWeights[i].weight0 = newWeight;
					//boneWeights[i].weight1 = 1-newWeight;
					colrs[i] = new Color(newWeight, newWeight, newWeight);


					if (boneWeights[i].boneIndex0 == boneIndex)
					{

						boneWeights[i].weight0 = colrs[i].r;
						boneWeights[i].weight1 = 1 - colrs[i].r;

					}
					else if (boneWeights[i].boneIndex1 == boneIndex)
					{

						boneWeights[i].weight1 = colrs[i].r;
						boneWeights[i].weight0 = 1 - colrs[i].r;

					}
					else if (colrs[i].r != 0)
					{
						if (boneWeights[i].weight0 == 0)
						{
							boneWeights[i].weight0 = colrs[i].r;
							boneWeights[i].boneIndex0 = boneIndex;
							boneWeights[i].weight1 = 1 - colrs[i].r;
						}
						else if (boneWeights[i].weight1 == 0)
						{
							boneWeights[i].weight1 = colrs[i].r;
							boneWeights[i].boneIndex1 = boneIndex;
							boneWeights[i].weight0 = 1 - colrs[i].r;
						}
					}
					



				}

				Puppet3DEditor.currentSelectionMesh.boneWeights = boneWeights;
				if (Puppet3DEditor.BlackAndWhiteWeights)
					Puppet3DEditor.currentSelectionMesh.colors = colrs;
				else
					Puppet3DEditor.currentSelectionMesh.colors = SetColors(boneWeights);
			}
		}
		public static void PaintSmoothWeightsOld(Ray ray)
		{

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Vector3[] vertices = Puppet3DEditor.currentSelectionMesh.vertices;
				Color[] colrs = Puppet3DEditor.currentSelectionMesh.colors;
				BoneWeight[] boneWeights = Puppet3DEditor.currentSelectionMesh.boneWeights;

				Vector3 pos = Puppet3DEditor.currentSelection.transform.InverseTransformPoint(hit.point);
				//Undo.RecordObject(Puppet3DEditor.currentSelectionMesh, "Weight paint");
				pos = new Vector3(pos.x, pos.y, pos.z);

				SkinnedMeshRenderer smr = Puppet3DEditor.currentSelection.GetComponent<SkinnedMeshRenderer>();
				int boneIndex = smr.bones.ToList().IndexOf(Puppet3DEditor.paintWeightsBone.transform);

				if (boneIndex < 0)
				{
					Debug.LogWarning(Puppet3DEditor.paintWeightsBone.name + " is not connected to skin");
					return;
				}

				List<int> vertsInRange = new List<int>();
				float weightTemp = 0f;
				float blend = 1;
				for (int i = 0; i < vertices.Length; i++)
				{
					float sqrMagnitude = (vertices[i] - pos).magnitude;
					if (sqrMagnitude < Puppet3DEditor.EditSkinWeightRadius)
					{
						vertsInRange.Add(i);
						if (boneWeights[i].boneIndex0 == boneIndex)
						{
							weightTemp += boneWeights[i].weight0;
							blend++;
						}
						else if (boneWeights[i].boneIndex1 == boneIndex)
						{
							weightTemp += boneWeights[i].weight1;
							blend++;
						}

						
					}
				}
				weightTemp /= blend;

				for (int i = 0; i < vertsInRange.Count; i++)
				{
					int index = vertsInRange[i];
					if (boneWeights[index].boneIndex0 < 0)
						boneWeights[index].boneIndex0 = 0;
					if (boneWeights[index].boneIndex1 < 0)
						boneWeights[index].boneIndex1 = 0;

					
					
					

					if (boneWeights[index].boneIndex0 == boneIndex)
					{
						
						boneWeights[index].weight0 = Mathf.Lerp(boneWeights[index].weight0, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
						boneWeights[index].weight1 = 1 - boneWeights[index].weight0;
						colrs[index] = new Color(boneWeights[index].weight0, boneWeights[index].weight0, boneWeights[index].weight0);
					}
					else if (boneWeights[index].boneIndex1 == boneIndex)
					{						

						boneWeights[index].weight1 = Mathf.Lerp(boneWeights[index].weight1, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
						boneWeights[index].weight0 = 1 - boneWeights[index].weight1;
						colrs[index] = new Color(boneWeights[index].weight1, boneWeights[index].weight1, boneWeights[index].weight1);
					}
					else if (weightTemp > 0f)
					{
						if (boneWeights[index].weight0 < boneWeights[index].weight1)
						{
							boneWeights[index].boneIndex0 = boneIndex;

							boneWeights[index].weight0 = Mathf.Lerp(boneWeights[index].weight0, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
							boneWeights[index].weight1 = 1 - boneWeights[index].weight0;
							colrs[index] = new Color(boneWeights[index].weight0, boneWeights[index].weight0, boneWeights[index].weight0);
						}
						else
						{
							boneWeights[index].boneIndex1 = boneIndex; 

							boneWeights[index].weight1 = Mathf.Lerp(boneWeights[index].weight1, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
							boneWeights[index].weight0 = 1 - boneWeights[index].weight1;
							colrs[index] = new Color(boneWeights[index].weight1, boneWeights[index].weight1, boneWeights[index].weight1);
						}
					}
					


						

					


				}

				Puppet3DEditor.currentSelectionMesh.boneWeights = boneWeights;
				if (Puppet3DEditor.BlackAndWhiteWeights)
					Puppet3DEditor.currentSelectionMesh.colors = colrs;
				else
					Puppet3DEditor.currentSelectionMesh.colors = SetColors(boneWeights);
			}

		}

		public static void PaintSmoothWeightsOld2(Ray ray)
		{

			//		Debug.Log("Paint Smooth Weights Old");
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Vector3[] vertices = Puppet3DEditor.currentSelectionMesh.vertices;
				Color[] colrs = Puppet3DEditor.currentSelectionMesh.colors;
				BoneWeight[] boneWeights = Puppet3DEditor.currentSelectionMesh.boneWeights;
				int[] tris = Puppet3DEditor.currentSelectionMesh.triangles;

				Vector3 pos = Puppet3DEditor.currentSelection.transform.InverseTransformPoint(hit.point);
				Undo.RecordObject(Puppet3DEditor.currentSelectionMesh, "Weight paint");
				pos = new Vector3(pos.x, pos.y, pos.z);

				SkinnedMeshRenderer smr = Puppet3DEditor.currentSelection.GetComponent<SkinnedMeshRenderer>();
				int boneIndex = smr.bones.ToList().IndexOf(Puppet3DEditor.paintWeightsBone.transform);

				if (boneIndex < 0)
				{
					Debug.LogWarning(Puppet3DEditor.paintWeightsBone.name + " is not connected to skin");
					return;
				}


				for (int i = 0; i < tris.Length; i++)
				{

					if (boneWeights[tris[i]].boneIndex0 < 0)
						boneWeights[tris[i]].boneIndex0 = 0;
					if (boneWeights[tris[i]].boneIndex1 < 0)
						boneWeights[tris[i]].boneIndex1 = 0;

					int indexB = 0;
					int indexC = 0;

					if (i % 3 == 2)
					{
						indexB = tris[i - 1];
						indexC = tris[i - 2];

					}
					else if ((i) % 3 == 1)
					{
						indexB = tris[i - 1];
						indexC = tris[i + 1];
					}
					else if ((i) % 3 == 0)
					{

						indexB = tris[i + 1];
						indexC = tris[i + 2];

					}
					float sqrMagnitude = (vertices[tris[i]] - pos).magnitude;
					if (sqrMagnitude < Puppet3DEditor.EditSkinWeightRadius)
					{

						//Debug.Log("h");
						float weightTemp = 0f;

						float blend = 1;
						

						if (boneWeights[indexB].boneIndex0 == boneIndex)
						{
							weightTemp += boneWeights[indexB].weight0;
							blend++;
						}
						else if (boneWeights[indexB].boneIndex1 == boneIndex)
						{
							weightTemp += boneWeights[indexB].weight1;
							blend++;
						}
						
						if (boneWeights[indexC].boneIndex0 == boneIndex)
						{
							weightTemp += boneWeights[indexC].weight0;
							blend++;
						}
						else if (boneWeights[indexC].boneIndex1 == boneIndex)
						{
							weightTemp += boneWeights[indexC].weight1; 
							blend++;
						}
						
						if (boneWeights[tris[i]].boneIndex0 == boneIndex)
						{
							blend++;
							weightTemp += boneWeights[tris[i]].weight0; weightTemp /= blend;

							boneWeights[tris[i]].weight0 = Mathf.Lerp(boneWeights[tris[i]].weight0, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
							boneWeights[tris[i]].weight1 = 1 - boneWeights[tris[i]].weight0;
							colrs[tris[i]] = new Color(boneWeights[tris[i]].weight0, boneWeights[tris[i]].weight0, boneWeights[tris[i]].weight0);
						}
						else if (boneWeights[tris[i]].boneIndex1 == boneIndex)
						{
							blend++;

							weightTemp += boneWeights[tris[i]].weight1; weightTemp /= blend;

							boneWeights[tris[i]].weight1 = Mathf.Lerp(boneWeights[tris[i]].weight1, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
							boneWeights[tris[i]].weight0 = 1 - boneWeights[tris[i]].weight1;
							colrs[tris[i]] = new Color(boneWeights[tris[i]].weight1, boneWeights[tris[i]].weight1, boneWeights[tris[i]].weight1);
						}
						else if (weightTemp >0f)
						{
							if (boneWeights[tris[i]].weight0 < boneWeights[tris[i]].weight1)
							{								
								boneWeights[tris[i]].boneIndex0 = boneIndex; weightTemp /= blend;

								boneWeights[tris[i]].weight0 = Mathf.Lerp(boneWeights[tris[i]].weight0, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
								boneWeights[tris[i]].weight1 = 1 - boneWeights[tris[i]].weight0;
								colrs[tris[i]] = new Color(boneWeights[tris[i]].weight0, boneWeights[tris[i]].weight0, boneWeights[tris[i]].weight0);
							}
							else
							{
								boneWeights[tris[i]].boneIndex1 = boneIndex; weightTemp /= blend;

								boneWeights[tris[i]].weight1 = Mathf.Lerp(boneWeights[tris[i]].weight1, weightTemp, Puppet3DEditor.paintWeightsStrength * Puppet3DEditor.paintWeightsStrength);
								boneWeights[tris[i]].weight0 = 1 - boneWeights[tris[i]].weight1;
								colrs[tris[i]] = new Color(boneWeights[tris[i]].weight1, boneWeights[tris[i]].weight1, boneWeights[tris[i]].weight1);
							}
						}


						
						

					}


				}

				Puppet3DEditor.currentSelectionMesh.boneWeights = boneWeights;
				if (Puppet3DEditor.BlackAndWhiteWeights)
					Puppet3DEditor.currentSelectionMesh.colors = colrs;
				else
					Puppet3DEditor.currentSelectionMesh.colors = SetColors(boneWeights);
			}

		}

		public static void ChangePaintRadius(Vector3 pos)
		{
			Puppet3DEditor.EditSkinWeightRadius = (pos - Puppet3DEditor.ChangeRadiusStartPosition).x + Puppet3DEditor.ChangeRadiusStartValue;

		}
		public static void ChangePaintStrength(Vector3 pos)
		{

			Puppet3DEditor.paintWeightsStrength = (pos - Puppet3DEditor.ChangeRadiusStartPosition).x * 0.1f + Puppet3DEditor.ChangeRadiusStartValue;
			Puppet3DEditor.paintWeightsStrength = Mathf.Clamp01(Puppet3DEditor.paintWeightsStrength);
		}
		public static float GetNeighbourWeight(Vector3[] vertices, BoneWeight[] boneWeights, List<int> indexes, int index, int boneIndex)
		{
			float distance = 1000000f;
			int closestIndex = indexes[0];
			for (int i = 0; i < indexes.Count; i++)
			{
				float checkDistance = (vertices[indexes[i]] - vertices[index]).magnitude;
				if (checkDistance < distance)
				{
					closestIndex = indexes[i];
					distance = checkDistance;
				}

			}
			if (boneWeights[closestIndex].boneIndex0 == boneIndex)
				return boneWeights[closestIndex].weight0;
			if (boneWeights[closestIndex].boneIndex1 == boneIndex)
				return boneWeights[closestIndex].weight1;
			if (boneWeights[closestIndex].boneIndex2 == boneIndex)
				return boneWeights[closestIndex].weight2;
			if (boneWeights[closestIndex].boneIndex3 == boneIndex)
				return boneWeights[closestIndex].weight3;
			return 0;

		}
	}
}
