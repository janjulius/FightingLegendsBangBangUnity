using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEditor;
namespace Puppet3D
{
	public class ColladaExporterExport : Editor
	{
		
		public class material
		{
			[XmlAttribute("id")]
			public string ID;
			[XmlAttribute("name")]
			public string Name;

		}
		public class geometry
		{
			[XmlAttribute("id")]
			public string ID;
			[XmlAttribute("name")]
			public string Name;

			public class source
			{
				[XmlAttribute("id")]
				public string ID;
				public class floatarray
				{
					[XmlAttribute("id")]
					public string ID;
					[XmlAttribute("count")]
					public float count;

					public string FloatString;
				}
				public floatarray float_array = new floatarray();
			}
			public List<source> mesh = new List<source>();

		}
		public List<material> library_materials = new List<material>();
		public List<geometry> library_geometries = new List<geometry>();


		// Use this for initialization
		[MenuItem("GameObject/Puppet3D/export collada")]
		static public void ExportCollad()
		{
			if (Selection.activeGameObject == null)
				return;
			SkinnedMeshRenderer smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
			if (smr == null)
			{
				Debug.LogWarning("Please selct an object with a skinnedMeshRenderer");
				return;
			}

			string path = AssetDatabase.GetAssetPath(smr.sharedMesh);
			path = path.Remove(path.Length - 6, 6);
			string newPath = AssetDatabase.GenerateUniqueAssetPath(path + ".dae");
			//ColladaExporter export = new ColladaExporter(("Assets/" + Selection.activeGameObject.name + ".dae"), true);
			Debug.Log(newPath);
			ColladaExporter export = new ColladaExporter(newPath, true);

			List<Transform> bones = new List<Transform>();
			GameObject[] objects = Selection.gameObjects;
			foreach (GameObject obj in objects)
			{
				smr = obj.GetComponent<SkinnedMeshRenderer>();
				if (smr == null)
				{
					Debug.LogWarning("Please selct an object with a skinnedMeshRenderer");
					break;
				}
				
				Mesh mesh =smr.sharedMesh;

				if (mesh.subMeshCount > 1)
				{
					Mesh newMesh = new Mesh();
					for (int index = 0; index < mesh.subMeshCount; index++)
					{
						newMesh = new Mesh();
						newMesh.vertices = mesh.vertices;
						newMesh.colors = mesh.colors;
						newMesh.normals = mesh.normals;
						newMesh.uv = mesh.uv;
						newMesh.bindposes = mesh.bindposes;
						newMesh.boneWeights = mesh.boneWeights;
						newMesh.tangents = mesh.tangents;
						newMesh.subMeshCount = 1;
						
						newMesh.SetTriangles(mesh.GetTriangles(index), 0);
						export.AddGeometry(obj.name + index, newMesh, smr);
						export.AddGeometryToScene(obj.name + index, obj.name + index);
					}
				}
				else
				{
					export.AddGeometry(obj.name, mesh, smr);
					export.AddGeometryToScene(obj.name, obj.name);
				}

				

				foreach (Transform bone in smr.bones)
				{
					if(!bones.Contains(bone))
						bones.Add(bone);
				}

				

			}
			export.AddControllerToScene(bones.ToArray());

			export.Save();
			return;
			
		}

		public void Save(string path)
		{
			var serializer = new XmlSerializer(typeof(ColladaExporterExport));
			using (var stream = new FileStream(path, FileMode.Create))
			{
				serializer.Serialize(stream, this);
			}
		}

		public static ColladaExporterExport Load(string path)
		{
			var serializer = new XmlSerializer(typeof(ColladaExporterExport));
			using (var stream = new FileStream(path, FileMode.Open))
			{
				return serializer.Deserialize(stream) as ColladaExporterExport;
			}
		}

		//Loads the xml directly from the given string. Useful in combination with www.text.
		public static ColladaExporterExport LoadFromText(string text)
		{
			var serializer = new XmlSerializer(typeof(ColladaExporterExport));
			return serializer.Deserialize(new StringReader(text)) as ColladaExporterExport;
		}
		
	}
}
