using UnityEngine;
using System;
using System.Xml;
using System.Text;

/**
 * Collada Exporter
 *
 * @author      Michael Grenier
 * @author_url  http://mgrenier.me
 * @copyright   2011 (c) Michael Grenier
 * @license     MIT - http://www.opensource.org/licenses/MIT
 *
 * @example
 *
 *              ColladaExporter export = new ColladaExporter("path/to/export.dae", replace_or_not);
 *              export.AddGeometry("MyMeshId", mesh_object);
 *              export.AddGeometryToScene("MyMeshId", "MyMeshName");
 *              export.Save();
 *
 */
namespace Puppet3D
{
	public class ColladaExporter : IDisposable
	{
		protected string path;
		public const string COLLADA = "http://www.collada.org/2005/11/COLLADASchema";
		public XmlDocument xml
		{
			get;
			protected set;
		}
		public XmlNamespaceManager nsManager
		{
			get;
			protected set;
		}

		protected XmlNode root;
		protected XmlNode cameras;
		protected XmlNode lights;
		protected XmlNode images;
		protected XmlNode effects;
		protected XmlNode materials;
		protected XmlNode geometries;
		protected XmlNode animations;
		protected XmlNode controllers;
		protected XmlNode visual_scenes;
		protected XmlNode default_scene;
		protected XmlNode scene;

		public ColladaExporter(String path)
		: this(path, true)
		{
		}

		public ColladaExporter(String path, bool replace)
		{
			this.path = path;
			this.xml = new XmlDocument();

			this.nsManager = new XmlNamespaceManager(this.xml.NameTable);
			this.nsManager.AddNamespace("x", COLLADA);

			if (!replace)
			{
				try
				{
					XmlTextReader reader = new XmlTextReader(path);
					this.xml.Load(reader);
					reader.Close();
					reader = null;
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}
			}
			else
				this.xml.AppendChild(this.xml.CreateXmlDeclaration("1.0", "UTF-8", null));

			XmlAttribute attr;

			this.root = this.xml.SelectSingleNode("/x:COLLADA", this.nsManager);
			if (this.root == null)
			{
				this.root = this.xml.AppendChild(this.xml.CreateElement("COLLADA", COLLADA));
				attr = this.xml.CreateAttribute("version");
				attr.Value = "1.4.1";
				this.root.Attributes.Append(attr);
			}

			XmlNode node;

			// Create asset
			{
				node = this.root.SelectSingleNode("/x:asset", this.nsManager);
				if (node == null)
				{
					this.root
					.AppendChild(
						this.xml.CreateElement("asset", COLLADA)
						.AppendChild(
							this.xml.CreateElement("contributor", COLLADA)
							.AppendChild(
								this.xml.CreateElement("author", COLLADA)
								.AppendChild(this.xml.CreateTextNode("Unity3D User"))
								.ParentNode
							)
							.ParentNode
							.AppendChild(
								this.xml.CreateElement("author_tool", COLLADA)
								.AppendChild(this.xml.CreateTextNode("Unity " + Application.unityVersion))
								.ParentNode
							)
							.ParentNode
						)
						.ParentNode
						.AppendChild(
							this.xml.CreateElement("created", COLLADA)
							.AppendChild(this.xml.CreateTextNode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00")))
							.ParentNode
						)
						.ParentNode
						.AppendChild(
							this.xml.CreateElement("modified", COLLADA)
							.AppendChild(this.xml.CreateTextNode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00")))
							.ParentNode
						)
						.ParentNode
						.AppendChild(
							this.xml.CreateElement("up_axis", COLLADA)
							.AppendChild(this.xml.CreateTextNode("Y_UP"))
							.ParentNode
						)
						.ParentNode
					);
				}
			}

			// Create libraries
			this.cameras = this.root.SelectSingleNode("/x:library_cameras", this.nsManager);
			if (this.cameras == null)
				this.cameras = this.root.AppendChild(this.xml.CreateElement("library_cameras", COLLADA));
			this.lights = this.root.SelectSingleNode("/x:library_lights", this.nsManager);
			if (this.lights == null)
				this.lights = this.root.AppendChild(this.xml.CreateElement("library_lights", COLLADA));
			this.images = this.root.SelectSingleNode("/x:library_images", this.nsManager);
			if (this.images == null)
				this.images = this.root.AppendChild(this.xml.CreateElement("library_images", COLLADA));
			this.effects = this.root.SelectSingleNode("/x:library_effects", this.nsManager);
			if (this.effects == null)
				this.effects = this.root.AppendChild(this.xml.CreateElement("library_effects", COLLADA));
			this.materials = this.root.SelectSingleNode("/x:library_materials", this.nsManager);
			if (this.materials == null)
				this.materials = this.root.AppendChild(this.xml.CreateElement("library_materials", COLLADA));
			this.geometries = this.root.SelectSingleNode("/x:library_geometries", this.nsManager);
			if (this.geometries == null)
				this.geometries = this.root.AppendChild(this.xml.CreateElement("library_geometries", COLLADA));
			this.animations = this.root.SelectSingleNode("/x:library_animations", this.nsManager);
			if (this.animations == null)
				this.animations = this.root.AppendChild(this.xml.CreateElement("library_animations", COLLADA));
			this.controllers = this.root.SelectSingleNode("/x:library_controllers", this.nsManager);
			if (this.controllers == null)
				this.controllers = this.root.AppendChild(this.xml.CreateElement("library_controllers", COLLADA));
			this.visual_scenes = this.root.SelectSingleNode("/x:library_visual_scenes", this.nsManager);
			if (this.visual_scenes == null)
			{
				this.visual_scenes = this.root.AppendChild(this.xml.CreateElement("library_visual_scenes", COLLADA));
				this.default_scene = this.visual_scenes.AppendChild(this.xml.CreateElement("visual_scene", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = "Scene";
				this.default_scene.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("name");
				attr.Value = "Scene";
				this.default_scene.Attributes.Append(attr);
			}
			this.scene = this.root.SelectSingleNode("/x:library_scene", this.nsManager);
			if (this.scene == null)
			{
				this.scene = this.root.AppendChild(this.xml.CreateElement("scene", COLLADA));
				node = this.scene.AppendChild(this.xml.CreateElement("instance_visual_scene", COLLADA));
				attr = this.xml.CreateAttribute("url");
				attr.Value = "#Scene";
				node.Attributes.Append(attr);
			}
		}

		public void Dispose()
		{
		}

		public void Save()
		{
			this.xml.Save(this.path);
		}
		public XmlNode AddController(string id, Mesh sourceMesh, SkinnedMeshRenderer smr)
		{
			XmlNode nodeA, nodeB;
			XmlAttribute attr;
			StringBuilder str;

			XmlNode controller = this.controllers.AppendChild(this.xml.CreateElement("controller", COLLADA));
			XmlNode skin = controller.AppendChild(this.xml.CreateElement("skin", COLLADA));
			attr = this.xml.CreateAttribute("source");
			attr.Value = ("#" + id + "-lib");
			skin.Attributes.Append(attr);



			attr = this.xml.CreateAttribute("id");
			attr.Value = id + "Controller";
			controller.Attributes.Append(attr);
			XmlNode bind_shape_matrix = skin.AppendChild(this.xml.CreateElement("bind_shape_matrix", COLLADA));

			str = new StringBuilder();
			bind_shape_matrix.AppendChild(this.xml.CreateTextNode(smr.transform.localToWorldMatrix.ToString()));

			/*if (sourceMesh.bindposes.Length > 0)
			{
				for (int i = 0; i < sourceMesh.bindposes.Length; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						str.Append(sourceMesh.bindposes[i][j].ToString());
						str.Append(" ");
					}
				}
				bind_shape_matrix.AppendChild(this.xml.CreateTextNode(str.ToString()));

			}*/
			string[] jointsArray = new string[smr.bones.Length];
			string[] matrixesArray = new string[smr.bones.Length];
			string[] weightsArray = new string[sourceMesh.boneWeights.Length*2];
			string[] boneAssignedArray = new string[sourceMesh.boneWeights.Length];

			

			for (int i = 0; i < smr.sharedMesh.bindposes.Length; i++)
			{
				//Matrix4x4 mat =  /*Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one)*/smr.bones[i].localToWorldMatrix;
				//mat = Matrix4x4.Transpose(mat);
				Matrix4x4 negScaleX = new Matrix4x4();
				negScaleX.SetColumn(0, new Vector4(-1, 0, 0, 0));
				negScaleX.SetColumn(1, new Vector4(0, 1, 0, 0));
				negScaleX.SetColumn(2, new Vector4(0, 0, 1, 0));
				negScaleX.SetColumn(3, new Vector4(0, 0, 0, 1));
				Matrix4x4 mat = negScaleX*smr.sharedMesh.bindposes[i]* negScaleX;
				/*if (parent != null)
				{
					mat = parent.worldToLocalMatrix * mat;
				}*/
				//mat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one) * mat;

				matrixesArray[i] = (mat.ToString() + " ");

				//Debug.Log(matrixesArray[i]);
			}
			for (int i = 0; i < smr.bones.Length; i++)
			{
				jointsArray[i] = smr.bones[i].name;


			}

			for (int i = 0; i < weightsArray.Length; i+=2)
			{
				//weightsArray[i] = sourceMesh.boneWeights[i].weight0 + " " + sourceMesh.boneWeights[i].weight1;
				//weightsArray[i] = "1.000000";
				weightsArray[i] = sourceMesh.boneWeights[i/2].weight0.ToString();
				weightsArray[i+1] = sourceMesh.boneWeights[i/2].weight1.ToString();

				//boneAssignedArray[i] = sourceMesh.boneWeights[i].boneIndex0 + " " + (i).ToString() + " " + sourceMesh.boneWeights[i].boneIndex1 + " " + ((i)).ToString();
				//boneAssignedArray[i] = "0 0";
				//boneAssignedArray[i] = sourceMesh.boneWeights[i].boneIndex0.ToString() + " " + i.ToString();
			}
			for (int i = 0; i < boneAssignedArray.Length; i++)
			{
				//weightsArray[i] = sourceMesh.boneWeights[i].weight0 + " " + sourceMesh.boneWeights[i].weight1;
				//weightsArray[i] = "1.000000";
				//weightsArray[i] = sourceMesh.boneWeights[i].weight0.ToString();
				boneAssignedArray[i] = sourceMesh.boneWeights[i].boneIndex0 + " " + (i*2).ToString() + " " + sourceMesh.boneWeights[i].boneIndex1 + " " + (i*2+1).ToString();
				//boneAssignedArray[i] = "0 0";
				//boneAssignedArray[i] = sourceMesh.boneWeights[i].boneIndex0.ToString() + " " + i.ToString();
			}
			skin = BuildSourceArrayTechniqueCommonAccessorParam(skin, (id + "Controller-Joints"), "Name_array", (id + "Controller-Joints-array"), jointsArray, "name");
			skin = BuildSourceArrayTechniqueCommonAccessorParam(skin, (id + "Controller-Matrices"), "float_array", (id + "Controller-Matrices-array"), matrixesArray, "float4x4");
			skin = BuildSourceArrayTechniqueCommonAccessorParam(skin, (id + "Controller-Weights"), "float_array", (id + "Controller-Weights-array"), weightsArray, "float");

			nodeA = skin.AppendChild(this.xml.CreateElement("joints", COLLADA));
			nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
			attr = this.xml.CreateAttribute("semantic");
			attr.Value = "JOINT";
			nodeB.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("source");
			attr.Value = ("#" + id + "Controller-Joints");
			nodeB.Attributes.Append(attr);

			nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
			attr = this.xml.CreateAttribute("semantic");
			attr.Value = "INV_BIND_MATRIX";
			nodeB.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("source");
			attr.Value = ("#" + id + "Controller-Matrices");
			nodeB.Attributes.Append(attr);

			nodeA = skin.AppendChild(this.xml.CreateElement("vertex_weights", COLLADA));
			attr = this.xml.CreateAttribute("count");
			attr.Value = (sourceMesh.vertexCount).ToString();
			nodeA.Attributes.Append(attr);

			nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
			attr = this.xml.CreateAttribute("semantic");
			attr.Value = "JOINT";
			nodeB.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("offset");
			attr.Value = "0";
			nodeB.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("source");
			attr.Value = ("#" + id + "Controller-Joints");
			nodeB.Attributes.Append(attr);

			nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
			attr = this.xml.CreateAttribute("semantic");
			attr.Value = "WEIGHT";
			nodeB.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("offset");
			attr.Value = "1";
			nodeB.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("source");
			attr.Value = ("#" + id + "Controller-Weights");
			nodeB.Attributes.Append(attr);

			nodeB = nodeA.AppendChild(this.xml.CreateElement("vcount", COLLADA));
			str = new StringBuilder();
			for (int i = 0; i < sourceMesh.vertexCount; ++i)
			{
				str.Append("2 ");

			}
			nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));

			nodeB = nodeA.AppendChild(this.xml.CreateElement("v", COLLADA));
			str = new StringBuilder();
			for (int i = 0, n = boneAssignedArray.Length; i < n; ++i)
			{
				str.Append(boneAssignedArray[i]);

				if (i + 1 != n)
					str.Append(" ");
			}
			nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
			return controller;
		}
		public XmlNode BuildSourceArrayTechniqueCommonAccessorParam(XmlNode sourceNode, string sourceID, string ArrayName, string ArrayID, string[] array, string paramTypeName)
		{
			XmlNode nodeA, nodeB, nodeC, nodeD;
			XmlAttribute attr;
			StringBuilder str;

			nodeA = sourceNode.AppendChild(this.xml.CreateElement("source", COLLADA));
			attr = this.xml.CreateAttribute("id");
			attr.Value = sourceID;
			nodeA.Attributes.Append(attr);

			nodeB = nodeA.AppendChild(this.xml.CreateElement(ArrayName, COLLADA));
			attr = this.xml.CreateAttribute("id");
			attr.Value = ArrayID;
			nodeB.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("count");
			if (paramTypeName == "float4x4")
				attr.Value = (array.Length * 16).ToString();
			/*else if (paramTypeName == "float")
				attr.Value = (array.Length * 2).ToString();*/
			else
				attr.Value = (array.Length).ToString();

			nodeB.Attributes.Append(attr);

			str = new StringBuilder();
			for (int i = 0, n = array.Length; i < n; ++i)
			{
				if (i % 16 == 0)
					str.Append("\n");
				str.Append(array[i]);
				
				if (i + 1 != n && (i+1) % 16 != 0)
					str.Append(" ");
			}
			nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
			str = null;

			nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
			nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
			attr = this.xml.CreateAttribute("source");
			attr.Value = "#" + ArrayID;
			nodeC.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("count");
			attr.Value = array.Length.ToString();
			nodeC.Attributes.Append(attr);

			nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
			attr = this.xml.CreateAttribute("type");
			attr.Value = paramTypeName;
			nodeD.Attributes.Append(attr);

			return sourceNode;
		}
		public XmlNode AddGeometry(string id, Mesh sourceMesh, SkinnedMeshRenderer smr)
		{
			AddController(id, sourceMesh, smr);
			XmlNode geometry = this.geometries.AppendChild(this.xml.CreateElement("geometry", COLLADA));
			XmlNode mesh = geometry.AppendChild(this.xml.CreateElement("mesh", COLLADA));
			XmlNode nodeA, nodeB, nodeC, nodeD;
			XmlAttribute attr;
			StringBuilder str;

			attr = this.xml.CreateAttribute("id");
			attr.Value = id + "-lib";
			geometry.Attributes.Append(attr);

			attr = this.xml.CreateAttribute("name");
			attr.Value = id + "Mesh";
			geometry.Attributes.Append(attr);

			// Positions
			if (sourceMesh.vertexCount > 0)
			{
				nodeA = mesh.AppendChild(this.xml.CreateElement("source", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-POSITION";
				nodeA.Attributes.Append(attr);

				nodeB = nodeA.AppendChild(this.xml.CreateElement("float_array", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-POSITION-array";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = (sourceMesh.vertexCount * 3).ToString();
				nodeB.Attributes.Append(attr);

				str = new StringBuilder();
				for (int i = 0, n = sourceMesh.vertexCount; i < n; ++i)
				{
					str.Append((-sourceMesh.vertices[i].x).ToString());
					str.Append(" ");
					str.Append(sourceMesh.vertices[i].y.ToString());
					str.Append(" ");
					str.Append(sourceMesh.vertices[i].z.ToString());
					if (i + 1 != n)
						str.Append(" ");
				}
				nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
				str = null;

				nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
				nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
				attr = this.xml.CreateAttribute("source");
				attr.Value = "#" + id + "-POSITION-array";
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = sourceMesh.vertexCount.ToString();
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("stride");
				attr.Value = "3";
				nodeC.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "X";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "Y";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "Z";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
			}

			// Colors
			if (sourceMesh.colors.Length > 0)
			{
				nodeA = mesh.AppendChild(this.xml.CreateElement("source", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-colors";
				nodeA.Attributes.Append(attr);

				nodeB = nodeA.AppendChild(this.xml.CreateElement("float_array", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-colors-array";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = (sourceMesh.colors.Length * 3).ToString();
				nodeB.Attributes.Append(attr);

				str = new StringBuilder();
				for (int i = 0, n = sourceMesh.colors.Length; i < n; ++i)
				{
					//str.Append(mesh.colors[i].a.ToString());
					//str.Append(" ");
					str.Append(sourceMesh.colors[i].r.ToString());
					str.Append(" ");
					str.Append(sourceMesh.colors[i].g.ToString());
					str.Append(" ");
					str.Append(sourceMesh.colors[i].b.ToString());
					if (i + 1 != n)
						str.Append(" ");
				}
				nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
				str = null;

				nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
				nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
				attr = this.xml.CreateAttribute("source");
				attr.Value = "#" + id + "-colors-array";
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = sourceMesh.colors.Length.ToString();
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("stride");
				attr.Value = "3";
				nodeC.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "R";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "G";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "B";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
			}

			// Normals
			if (sourceMesh.normals.Length > 0)
			{
				nodeA = mesh.AppendChild(this.xml.CreateElement("source", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-Normal0";
				nodeA.Attributes.Append(attr);

				nodeB = nodeA.AppendChild(this.xml.CreateElement("float_array", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-Normal0-array";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = (sourceMesh.normals.Length * 3).ToString();
				nodeB.Attributes.Append(attr);

				str = new StringBuilder();
				for (int i = 0, n = sourceMesh.normals.Length; i < n; ++i)
				{
					str.Append((-sourceMesh.normals[i].x).ToString());
					str.Append(" ");
					str.Append(sourceMesh.normals[i].y.ToString());
					str.Append(" ");
					str.Append(sourceMesh.normals[i].z.ToString());
					if (i + 1 != n)
						str.Append(" ");
				}
				nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
				str = null;

				nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
				nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
				attr = this.xml.CreateAttribute("source");
				attr.Value = "#" + id + "-Normal0-array";
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = sourceMesh.normals.Length.ToString();
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("stride");
				attr.Value = "3";
				nodeC.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "X";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "Y";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "Z";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
			}
			// UVS
			if (sourceMesh.uv.Length > 0)
			{
				nodeA = mesh.AppendChild(this.xml.CreateElement("source", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-map1";
				nodeA.Attributes.Append(attr);

				nodeB = nodeA.AppendChild(this.xml.CreateElement("float_array", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-map1-array";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = (sourceMesh.uv.Length).ToString();
				nodeB.Attributes.Append(attr);

				str = new StringBuilder();
				for (int i = 0, n = sourceMesh.uv.Length; i < n; ++i)
				{
					str.Append((sourceMesh.uv[i].x).ToString());
					str.Append(" ");
					str.Append(sourceMesh.uv[i].y.ToString());
					if (i + 1 != n)
						str.Append(" ");
				}
				nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
				str = null;

				nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
				nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
				attr = this.xml.CreateAttribute("source");
				attr.Value = "#" + id + "-map1-array";
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("count");
				attr.Value = sourceMesh.normals.Length.ToString();
				nodeC.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("stride");
				attr.Value = "2";
				nodeC.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "S";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
				attr = this.xml.CreateAttribute("name");
				attr.Value = "T";
				nodeD.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("type");
				attr.Value = "float";
				nodeD.Attributes.Append(attr);
				nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
			}
			// Vertices
			{
				nodeA = mesh.AppendChild(this.xml.CreateElement("vertices", COLLADA));
				attr = this.xml.CreateAttribute("id");
				attr.Value = id + "-vertices";
				nodeA.Attributes.Append(attr);

				if (sourceMesh.vertexCount > 0)
				{
					nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
					attr = this.xml.CreateAttribute("semantic");
					attr.Value = "POSITION";
					nodeB.Attributes.Append(attr);
					attr = this.xml.CreateAttribute("source");
					attr.Value = "#" + id + "-POSITION";
					nodeB.Attributes.Append(attr);
				}

				if (sourceMesh.normals.Length > 0)
				{
					nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
					attr = this.xml.CreateAttribute("semantic");
					attr.Value = "NORMAL";
					nodeB.Attributes.Append(attr);
					attr = this.xml.CreateAttribute("source");
					attr.Value = "#" + id + "-Normal0";
					nodeB.Attributes.Append(attr);
				}

				if (sourceMesh.colors.Length > 0)
				{
					nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
					attr = this.xml.CreateAttribute("semantic");
					attr.Value = "COLOR";
					nodeB.Attributes.Append(attr);
					attr = this.xml.CreateAttribute("source");
					attr.Value = "#" + id + "-colors";
					nodeB.Attributes.Append(attr);
				}
			}

			// Triangles
			{
				nodeA = mesh.AppendChild(this.xml.CreateElement("triangles", COLLADA));
				attr = this.xml.CreateAttribute("count");
				attr.Value = (sourceMesh.triangles.Length / 3).ToString();
				nodeA.Attributes.Append(attr);

				nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
				attr = this.xml.CreateAttribute("semantic");
				attr.Value = "VERTEX";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("source");
				attr.Value = "#" + id + "-vertices";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("offset");
				attr.Value = "0";
				nodeB.Attributes.Append(attr);

				nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
				attr = this.xml.CreateAttribute("semantic");
				attr.Value = "NORMAL";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("source");
				attr.Value = "#" + id + "-Normal0";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("offset");
				attr.Value = "1";
				nodeB.Attributes.Append(attr);

				nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
				attr = this.xml.CreateAttribute("semantic");
				attr.Value = "TEXCOORD";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("source");
				attr.Value = "#" + id + "-map1";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("offset");
				attr.Value = "2";
				nodeB.Attributes.Append(attr);
				attr = this.xml.CreateAttribute("set");
				attr.Value = "0";
				nodeB.Attributes.Append(attr);	

				nodeB = nodeA.AppendChild(this.xml.CreateElement("p", COLLADA));

				str = new StringBuilder();
				//for (int i = 0, n = sourceMesh.triangles.Length; i < n; ++i)
				for (int i = sourceMesh.triangles.Length - 1; i >= 0; --i)
				{
					str.Append(sourceMesh.triangles[i].ToString());
					str.Append(" ");
					str.Append(sourceMesh.triangles[i].ToString());
					str.Append(" ");
					str.Append(sourceMesh.triangles[i].ToString());
					if (i != 0)
						str.Append(" ");
				}

				nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
				str = null;
			}

			return geometry;
		}
		public int HasParent(int testIndex, Transform[] posibilities)
		{
			for (int i = 0; i < posibilities.Length; i++)
			{
				if (testIndex != i && posibilities[testIndex].parent == posibilities[i])
				{
					return i;
				}
			}
			return -1;
		}
		public bool HasThis(Transform check, Transform[] posibilities)
		{
			for (int i = 0; i < posibilities.Length; i++)
			{
				if (check == posibilities[i])
				{
					return true;
				}
			}
			return false;
		}
		public void AddControllerToScene(Transform[] bones)
		{
			for (int i = 0; i < bones.Length; i++)
			{
				int parentIndex = HasParent(i, bones);
				if (parentIndex < 0)
				{
					AddControllerToSceneRecursive(bones[i], null, bones);
				}

			}

		}
		public void AddControllerToSceneRecursive(Transform bone, XmlNode ParentNode, Transform[] bones)
		{
			Transform parent = bone.parent;
			Matrix4x4 mat = bone.localToWorldMatrix;
			Matrix4x4 negScaleX = new Matrix4x4();
			negScaleX.SetColumn(0, new Vector4(-1, 0, 0, 0));
			negScaleX.SetColumn(1, new Vector4(0, 1, 0, 0));
			negScaleX.SetColumn(2, new Vector4(0, 0, 1, 0));
			negScaleX.SetColumn(3, new Vector4(0, 0, 0, 1));
			if (parent != null)
			{
				mat = negScaleX*parent.worldToLocalMatrix * mat* negScaleX;
			}
			
			//mat = negScaleX * mat;
			//mat = Matrix4x4.Transpose (mat);
			//mat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 90, 0), Vector3.one) * mat;
			ParentNode = AddControllerToScene(bone.name, bone.name, mat, ParentNode);
			foreach (Transform child in bone.transform)
			{
				if (HasThis(child, bones))
					AddControllerToSceneRecursive(child, ParentNode, bones);
			}


		}
		public XmlNode AddControllerToScene(string id, string name, Matrix4x4 matrix, XmlNode parent)
		{
			XmlNode nodeA, nodeB;
			XmlAttribute attr;

			if (parent == null)
				parent = this.default_scene;

			nodeA = parent.AppendChild(this.xml.CreateElement("node", COLLADA));
			attr = this.xml.CreateAttribute("id");
			attr.Value = id;
			nodeA.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("type");
			attr.Value = "JOINT";
			nodeA.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("name");
			attr.Value = name;
			nodeA.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("sid");
			attr.Value = id;
			nodeA.Attributes.Append(attr);


			nodeB = nodeA.AppendChild(this.xml.CreateElement("matrix", COLLADA));
			attr = this.xml.CreateAttribute("sid");
			attr.Value = "matrix";
			nodeB.Attributes.Append(attr);
			nodeB.AppendChild(this.xml.CreateTextNode(matrix.ToString()));

			/*nodeB = nodeA.AppendChild(this.xml.CreateElement("instance_geometry", COLLADA));
			attr = this.xml.CreateAttribute("url");
			attr.Value = "#" + id + "-mesh";
			nodeB.Attributes.Append(attr);*/

			return nodeA;
		}
		public XmlNode AddGeometryToScene(string id, string name)
		{
			return AddGeometryToScene(id, name, Matrix4x4.identity, null);
		}

		public XmlNode AddGeometryToScene(string id, string name, Matrix4x4 matrix)
		{
			return AddGeometryToScene(id, name, matrix, null);
		}

		public XmlNode AddGeometryToScene(string id, string name, Matrix4x4 matrix, XmlNode parent)
		{
			XmlNode nodeA, nodeB;
			XmlAttribute attr;

			if (parent == null)
				parent = this.default_scene;

			nodeA = parent.AppendChild(this.xml.CreateElement("node", COLLADA));
			attr = this.xml.CreateAttribute("id");
			attr.Value = id;
			nodeA.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("sid");
			attr.Value = id;
			nodeA.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("name");
			attr.Value = name;
			nodeA.Attributes.Append(attr);

			nodeB = nodeA.AppendChild(this.xml.CreateElement("instance_controller", COLLADA));
			attr = this.xml.CreateAttribute("url");
			attr.Value = ("#" + id + "Controller");
			nodeB.Attributes.Append(attr);


			nodeB = nodeA.AppendChild(this.xml.CreateElement("instance_geometry", COLLADA));
			attr = this.xml.CreateAttribute("url");
			attr.Value = "#" + id + "-mesh";
			nodeB.Attributes.Append(attr);

			return nodeA;
		}

		public XmlNode AddEmptyToScene(string id, string name)
		{
			return AddEmptyToScene(id, name, Matrix4x4.identity, null);
		}

		public XmlNode AddEmptyToScene(string id, string name, Matrix4x4 matrix)
		{
			return AddEmptyToScene(id, name, matrix, null);
		}

		public XmlNode AddEmptyToScene(string id, string name, Matrix4x4 matrix, XmlNode parent)
		{
			XmlNode nodeA, nodeB;
			XmlAttribute attr;

			if (parent == null)
				parent = this.default_scene;

			nodeA = parent.AppendChild(this.xml.CreateElement("node", COLLADA));
			attr = this.xml.CreateAttribute("id");
			attr.Value = id;
			nodeA.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("type");
			attr.Value = "Node";
			nodeA.Attributes.Append(attr);
			attr = this.xml.CreateAttribute("name");
			attr.Value = name;
			nodeA.Attributes.Append(attr);

			nodeB = nodeA.AppendChild(this.xml.CreateElement("matrix", COLLADA));
			attr = this.xml.CreateAttribute("sid");
			attr.Value = "matrix";
			nodeB.Attributes.Append(attr);
			nodeB.AppendChild(this.xml.CreateTextNode(matrix.ToString()));

			return nodeA;
		}

	}
}