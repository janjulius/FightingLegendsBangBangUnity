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
	public class Puppet3DEditor : EditorWindow
	{

		public static bool SkinWeightsPaint = false;
		public static Mesh currentSelectionMesh;
		public static GameObject currentSelection;
		public static Color[] previousVertColors;
		public static float EditSkinWeightRadius = 5f;
		public static GameObject paintWeightsBone;
		public static List<Shader> previousShader;
		public static Vector3 ChangeRadiusStartPosition;
		public static float ChangeRadiusStartValue = 0f;
		public static bool ChangingRadius = false;
		public static float paintWeightsStrength = 0.25f;
		public static Color paintControlColor = new Color(.8f, 1f, .8f, .5f);
		public bool showLayerSizePanel = true;
		public string layerSizePanel = "Hide";

		public static bool BoneCreation = false;
		static bool EditSkinWeights = false;
		public static bool SplineCreation = false;
		public static bool FFDCreation = false;

		GameObject currentBone;
		GameObject previousBone;

		public bool ReverseNormals;

		public static int _triangulationIndex, _numberBonesToSkinToIndex = 1, _skinningType = 1;

		public static Sprite boneNoJointSprite = null;
		public static Sprite boneSprite = null;
		public static Sprite boneHiddenSprite = null;
		public static Sprite boneOriginal = null;

		public static GameObject currentActiveBone = null;

		public static int numberSplineJoints = 4;
			

		public static bool BlackAndWhiteWeights = true;


		private string pngSequPath, checkPath;
		bool recordPngSequence = false;
		private int imageCount = 0;
		private float recordDelta = 0f;
		bool ExportPngAlpha;

		public static List<List<string>> selectedControls = new List<List<string>>();
		public static List<List<string>> selectedControlsData = new List<List<string>>();

		static public string _puppet3DPath;

		static public bool HasGuides = false;
		static public bool KeepVoxels = false;
		static public float VoxelScale = 100f;
		static public float VoxelScaleMax = 10000f;
		static public int BlurIter = 10;
		static public bool Fingers = false;

		static public DrivenKey DrivenKeyActive = null;
		public enum GUIChoice
		{
			AutoRig,
			BoneCreation,
			RigginSetup,
			Skinning,
			Animation,
			About,
		}
		GUIChoice currentGUIChoice = GUIChoice.AutoRig;

		[MenuItem("GameObject/Puppet3D/Window/Puppet3D")]
		[MenuItem("Window/Puppet3D")]
		static void Init()
		{
			Puppet3DEditor window = (Puppet3DEditor)EditorWindow.GetWindow(typeof(Puppet3DEditor));
			window.titleContent = new GUIContent();
			window.titleContent.text = "Puppet3D";
			window.position = new Rect(100, 200, 300, 500);
			window.Show();
		}
		void OnEnable()
		{

			RecursivelyFindFolderPath("Assets");
			
			Puppet3DSelection.GetSelectionString();
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		void OnGUI()
		{
						
			Texture autoRigTexture = AssetDatabase.LoadAssetAtPath(Puppet3DEditor._puppet3DPath + "/Textures/GUI/autoRigRobot.png", typeof(Texture)) as Texture;
			Texture autoRigGuidesTexture = AssetDatabase.LoadAssetAtPath(Puppet3DEditor._puppet3DPath + "/Textures/GUI/autoRigRobotGuides.png", typeof(Texture)) as Texture;


			Color bgColor = GUI.backgroundColor;

			if (currentGUIChoice == GUIChoice.BoneCreation)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(0, 0, 100, 20), "Skeleton"))
			{
				currentGUIChoice = GUIChoice.BoneCreation;
			}

			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.RigginSetup)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(100, 0, 100, 20), "Rigging"))
			{
				currentGUIChoice = GUIChoice.RigginSetup;
			}
			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.Skinning)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(200, 0, 100, 20), "Skinning"))
			{
				currentGUIChoice = GUIChoice.Skinning;
			}
			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.Animation)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(0, 20, 100, 20), "Animation"))
			{
				currentGUIChoice = GUIChoice.Animation;
			}
			GUI.backgroundColor = bgColor;

			if (currentGUIChoice == GUIChoice.AutoRig)
				GUI.backgroundColor = Color.green;
			if (GUI.Button(new Rect(100, 20, 100, 20), "Auto Rig"))
			{
				currentGUIChoice = GUIChoice.AutoRig;
			}
			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.About)
				GUI.backgroundColor = Color.green;
			if (GUI.Button(new Rect(200, 20, 100, 20), "About"))
			{
				currentGUIChoice = GUIChoice.About;
			}
			GUI.backgroundColor = bgColor;


			if (EditSkinWeights || SplineCreation || FFDCreation)
				GUI.backgroundColor = Color.grey;

			int offsetControls = 30;
			
			GUILayout.Space(offsetControls + 50);

			if (currentGUIChoice == GUIChoice.BoneCreation)
			{
				

				if (BoneCreation)
					GUI.backgroundColor = Color.green;


				if (GUI.Button(new Rect(80, 60 + offsetControls, 150, 30), "Create Bone Tool"))
				{
					BoneCreation = true;
					currentActiveBone = null;
					EditorPrefs.SetBool("Puppet3D_BoneCreation", BoneCreation);

				}
				if (BoneCreation)
					GUI.backgroundColor = bgColor;


				if (GUI.Button(new Rect(80, 90 + offsetControls, 150, 30), "Finish Bone"))
				{
					Puppet3D.BoneCreation.BoneFinishCreation();
				}

				if (BoneCreation)
					GUI.backgroundColor = Color.grey;



				if (SplineCreation)
				{
					GUI.backgroundColor = Color.green;
				}
				if (GUI.Button(new Rect(80, 150 + offsetControls, 150, 30), "Create Spline Tool"))
				{

					SplineEditor.CreateSplineTool();
				}
				if (SplineCreation)
				{
					GUI.backgroundColor = bgColor;
				}
				numberSplineJoints = EditorGUI.IntSlider(new Rect(80, 190 + offsetControls, 150, 20), numberSplineJoints, 1, 10);

				if (GUI.Button(new Rect(80, 220 + offsetControls, 150, 30), "Finish Spline"))
				{
					SplineEditor.SplineFinishCreation();

				}
			}
			if (currentGUIChoice == GUIChoice.RigginSetup)
			{

				if (GUI.Button(new Rect(80, 60 + offsetControls, 150, 30), "Create IK Control"))
				{
					CreateControls.IKCreateTool(false, false);

				}
				if (GUI.Button(new Rect(80, 90 + offsetControls, 150, 30), "Create Limb Control"))
				{
					CreateControls.IKCreateTool(false, true);

				}
				if (GUI.Button(new Rect(80, 120 + offsetControls, 150, 30), "Create Parent Control"))
				{
					CreateControls.CreateParentControl();

				}
				if (GUI.Button(new Rect(80, 150 + offsetControls, 150, 30), "Create Orient Control"))
				{
					CreateControls.CreateOrientControl();

				}
				if (GUI.Button(new Rect(80, 150 + offsetControls, 150, 30), "Create Orient Control"))
				{
					CreateControls.CreateOrientControl();

				}
				GUI.Label(new Rect(30, 205 + offsetControls, 150, 30), "(Beta)");

				if (GUI.Button(new Rect(80, 200 + offsetControls, 150, 30), "Create Blend Shape"))
				{
					CreateBlendShape.MakeBlendShape();

				}
				GUI.Label(new Rect(30, 235 + offsetControls, 150, 30), "(Beta)");

				if (GUI.Button(new Rect(80, 230 + offsetControls, 150, 30), "Clear Blend Shapes"))
				{
					CreateBlendShape.ClearBlendShapes();

				}
				if (GUI.Button(new Rect(80, 290 + offsetControls, 150, 30), "Set Driver"))
				{
					DrivenKeyActive = Selection.activeGameObject.GetComponent<DrivenKey>();

					if (DrivenKeyActive == null)
					{
						DrivenKeyActive = Selection.activeGameObject.AddComponent<DrivenKey>();

						DrivenKeyActive.DriverColection = new DrivenObject[1];
					}

				}

				if (DrivenKeyActive != null)
				{
					GUI.Label(new Rect(80, 325 + offsetControls, 150, 30), DrivenKeyActive.gameObject.name);
					if (GUI.Button(new Rect(80, 350 + offsetControls, 150, 30), "Set Driven"))
					{

						DrivenKeyActive.DriverColection[0] = new DrivenObject(Selection.gameObjects);
					}
					if (GUI.Button(new Rect(80, 380 + offsetControls, 150, 30), "Set End Position"))
					{
						for (int i = 0; i < DrivenKeyActive.DriverColection[0].StartPositions.Length; i++)
						{
							DrivenKeyActive.DriverColection[0].EndPositions[i] = DrivenKeyActive.DriverColection[0].DrivenGOs[i].transform.localPosition;
							DrivenKeyActive.DriverColection[0].EndRotations[i] = DrivenKeyActive.DriverColection[0].DrivenGOs[i].transform.localRotation;

						}
					}
					/*if (GUI.Button (new Rect (80, 310 + offsetControls, 150, 30), "Set Start Position")) 
                    {
                        if (DrivenKeyActive == null) 
                        {
                    
                        }
                        for (int i = 0; i < DrivenKeyActive.DriverColection [0].StartPositions.Length; i++) 
                        {
                            DrivenKeyActive.DriverColection [0].StartPositions [i] = DrivenKeyActive.DriverColection [0].DrivenGOs [i].transform.localPosition;
                            DrivenKeyActive.DriverColection [0].StartRotations [i] = DrivenKeyActive.DriverColection [0].DrivenGOs [i].transform.localRotation;

                        }
                    }*/

				}

			}
			if (currentGUIChoice == GUIChoice.Skinning)
			{

				GUILayout.Space(5);
				GUIStyle labelNew = EditorStyles.label;
				labelNew.alignment = TextAnchor.LowerLeft;
				labelNew.contentOffset = new Vector2(80, 0);
				
				labelNew.contentOffset = new Vector2(80, 0);
				GUILayout.Label("Num Skin Bones: ", labelNew);
				labelNew.contentOffset = new Vector2(0, 0);
				string[] NumberBonesToSkinTo = { "1", "2" };

				_numberBonesToSkinToIndex = EditorGUI.Popup(new Rect(180, 50 + offsetControls, 50, 30), _numberBonesToSkinToIndex, NumberBonesToSkinTo);

				if (GUI.Button(new Rect(80, 100 + offsetControls, 150, 30), "Bind Smooth Skin"))
				{
					if (_numberBonesToSkinToIndex == 1)
						Skinning.BindSmoothSkin(_skinningType);
					else
						Skinning.BindSmoothSkin(1);
				}
				if (GUI.Button(new Rect(80, 130 + offsetControls, 150, 30), "Detach Skin"))
				{
					Skinning.DetatchSkin();					
				}

				
				string[] SkinningTypeChoice = { "Voxel", "Closest Point" };
				
				labelNew.alignment = TextAnchor.LowerLeft;
				labelNew.contentOffset = new Vector2(80, 0);
				GUILayout.Label("Skinning: ", labelNew);
				labelNew.contentOffset = new Vector2(0, 0);
				GUILayout.Space(40);
				_skinningType = EditorGUI.Popup(new Rect(140, 75 + offsetControls, 90, 30), _skinningType, SkinningTypeChoice);
				offsetControls += 40;
				GUILayout.Space(offsetControls -15);

				KeepVoxels = GUI.Toggle(new Rect(80, 130 + offsetControls, 256, 20), KeepVoxels, " Keep Voxels");
				GUILayout.Label(" Voxel Res", EditorStyles.boldLabel);

				VoxelScale = EditorGUI.Slider(new Rect(80, 150 + offsetControls, 150, 20), VoxelScale, 1f, VoxelScaleMax);


				GUILayout.Label(" Blur Iter", EditorStyles.boldLabel);

				BlurIter = EditorGUI.IntSlider(new Rect(80, 170 + offsetControls, 150, 20), BlurIter, 0, 20);

				if (GUI.Button(new Rect(80, 200 + offsetControls, 150, 30), "Export Skin & Bones "))
				{
					Puppet3D.ColladaExporterExport.ExportCollad();
				}

				if (EditSkinWeights || SkinWeightsPaint)
				{
					GUI.backgroundColor = Color.green;
				}
				offsetControls += 50;
				if (SkinWeightsPaint)
				{
					if (GUI.Button(new Rect(80, 190 + offsetControls, 150, 30), "Manually Edit Weights"))
					{
						// finish paint weights
						Selection.activeGameObject = currentSelection;
						if (currentSelection)
						{
							if (previousShader.Count > 0)
							{
								for(int i=0;i< previousShader.Count;i++)
									currentSelection.GetComponent<Renderer>().sharedMaterials[i].shader = previousShader[i];
							}
							SkinWeightsPaint = false;
							if (previousVertColors != null && previousVertColors.Length > 0)
								currentSelectionMesh.colors = previousVertColors;
							currentSelectionMesh = null;
							currentSelection = null;
							previousVertColors = null;
						}

						EditSkinWeights = Skinning.EditWeights();

					}
				}
				if (!SkinWeightsPaint)
				{
					if (GUI.Button(new Rect(80, 190 + offsetControls, 150, 30), "Paint Weights"))
					{
						if (EditSkinWeights)
						{
							EditSkinWeights = false;
							Object[] bakedMeshes = Skinning.FinishEditingWeights();

							Selection.objects = bakedMeshes;
						}

						if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>() && Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh)
						{
							SkinWeightsPaint = true;
							SkinnedMeshRenderer smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
							currentSelectionMesh = smr.sharedMesh;
							currentSelectionMesh = Skinning.SaveFBXMesh(currentSelectionMesh);
							foreach (Transform bone in smr.bones)
							{
								if (bone.GetComponent<Bone>() == null)
									bone.gameObject.AddComponent<Bone>();
							}
							if (smr.GetComponent<MeshCollider>() == null)
							{
								MeshCollider meshCol = smr.gameObject.AddComponent<MeshCollider>();
								meshCol.sharedMesh = currentSelectionMesh;
							}
							smr.sharedMesh = currentSelectionMesh;
							currentSelection = Selection.activeGameObject;
							previousShader = new List<Shader>();
							previousShader.Clear();
							foreach (Material m in currentSelection.GetComponent<Renderer>().sharedMaterials)
							{
								previousShader.Add(m.shader);
								m.shader = Shader.Find("Puppet3D/vertColor");

							}

							EditSkinWeightRadius = currentSelection.GetComponent<Renderer>().bounds.size.y / 20f;

							if (currentSelectionMesh.colors.Length != currentSelectionMesh.vertices.Length)
							{
								currentSelectionMesh.colors = new Color[currentSelectionMesh.vertices.Length];
								EditorUtility.SetDirty(currentSelection);
								EditorUtility.SetDirty(currentSelectionMesh);
								AssetDatabase.SaveAssets();
								AssetDatabase.SaveAssets();
							}
							else
								previousVertColors = currentSelectionMesh.colors;
							Selection.activeGameObject = smr.bones[0].gameObject;
						}
					}
				}



				if (EditSkinWeights || SkinWeightsPaint)
					GUI.backgroundColor = bgColor;

				if (GUI.Button(new Rect(80, 220 + offsetControls, 150, 30), "Finish Edit Skin Weights"))
				{
					if (SkinWeightsPaint)
					{
						if (currentSelection)
						{
							Selection.activeGameObject = currentSelection;
														
							if (previousShader.Count > 0)
							{
								for (int i = 0; i < previousShader.Count; i++)
									currentSelection.GetComponent<Renderer>().sharedMaterials[i].shader = previousShader[i];
							}
							SkinWeightsPaint = false;
							if (previousVertColors != null && previousVertColors.Length > 0)
								currentSelectionMesh.colors = previousVertColors;
							currentSelectionMesh = null;
							currentSelection = null;
							previousVertColors = null;

							Bone[] bones = Transform.FindObjectsOfType<Bone>();
							foreach (Bone bone in bones)
							{
								bone.Color = Color.white;
								
							}

						}
						else
							SkinWeightsPaint = false;
					}
					else
					{
						EditSkinWeights = false;
						Skinning.FinishEditingWeights();
					}

				}
								
				if (SkinWeightsPaint)
				{

					GUILayout.Space(offsetControls -30);
					GUILayout.Label(" Brush Size", EditorStyles.boldLabel);
					float modelSize = currentSelection.transform.GetComponent<Renderer>().bounds.size.y/2f;
					EditSkinWeightRadius = EditorGUI.Slider(new Rect(80, 265 + offsetControls, 150, 20), EditSkinWeightRadius, 0F, modelSize);
					GUILayout.Label(" Strength", EditorStyles.boldLabel);
					paintWeightsStrength = EditorGUI.Slider(new Rect(80, 295 + offsetControls, 150, 20), paintWeightsStrength, 0F, 1F);



					EditorGUI.BeginChangeCheck();
					BlackAndWhiteWeights = EditorGUI.Toggle(new Rect(5, 325 + offsetControls, 200, 20), "BlackAndWhite", BlackAndWhiteWeights);
					if (EditorGUI.EndChangeCheck())
					{
						RefreshMeshColors();

					}
				}

				if (EditSkinWeights || SkinWeightsPaint)
					GUI.backgroundColor = Color.grey;

				
				

			}
			if (currentGUIChoice == GUIChoice.Animation)
			{

				if (GUI.Button(new Rect(80, 60 + offsetControls, 150, 30), "Bake Animation"))
				{
					GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<GlobalControl>();
					for (int i = 0; i < globalCtrlScripts.Length; i++)
					{
						BakeAnimation BakeAnim = globalCtrlScripts[i].gameObject.AddComponent<BakeAnimation>();
						BakeAnim.Run();
						DestroyImmediate(BakeAnim);
						globalCtrlScripts[i].enabled = false;
					}
				}

				if (recordPngSequence && !ExportPngAlpha)
				GUI.backgroundColor = Color.green;
				if (GUI.Button(new Rect(80, 130 + offsetControls, 150, 30), "Render Animation"))
				{
					checkPath = EditorUtility.SaveFilePanel("Choose Directory", pngSequPath, "exportedAnim", "");
					if (checkPath != "")
					{
						pngSequPath = checkPath;
						recordPngSequence = true;
						EditorApplication.ExecuteMenuItem("Edit/Play");
					}
				}
				GUI.backgroundColor = bgColor;

				if (ExportPngAlpha || recordPngSequence)
					GUI.backgroundColor = bgColor;
				if (GUI.Button(new Rect(80, 200 + offsetControls, 150, 30), "Save Selection"))
				{
					selectedControls.Add(new List<string>());
					selectedControlsData.Add(new List<string>());

					foreach (GameObject go in Selection.gameObjects)
					{
						selectedControls[selectedControls.Count - 1].Add(Puppet3DSelection.GetGameObjectPath(go));
						selectedControlsData[selectedControlsData.Count - 1].Add(go.transform.localPosition.x + " " + go.transform.localPosition.y + " " + go.transform.localPosition.z + " " + go.transform.localRotation.x + " " + go.transform.localRotation.y + " " + go.transform.localRotation.z + " " + go.transform.localRotation.w + " " + go.transform.localScale.x + " " + go.transform.localScale.y + " " + go.transform.localScale.z + " ");

					}
					Puppet3DSelection.SetSelectionString();
				}
				if (GUI.Button(new Rect(80, 230 + offsetControls, 150, 30), "Clear Selections"))
				{
					selectedControls.Clear();
					selectedControlsData.Clear();
					Puppet3DSelection.SetSelectionString();
				}


				for (int i = 0; i < selectedControls.Count; i++)
				{
					int column = i % 3;
					int row = 0;

					row = i / 3;
					Rect newLoadButtonPosition = new Rect(80 + (50 * column), 265 + offsetControls + row * 30, 50, 30);

					if (Event.current.type == EventType.ContextClick)
					{
						Vector2 mousePos = Event.current.mousePosition;
						if ((Event.current.button == 1) && newLoadButtonPosition.Contains(mousePos))
						{
							GenericMenu menu = new GenericMenu();

							menu.AddItem(new GUIContent("Select Objects"), false, Puppet3DSelection.SaveSelectionLoad, i);
							menu.AddItem(new GUIContent("Remove Selection"), false, Puppet3DSelection.SaveSelectionRemove, i);
							menu.AddItem(new GUIContent("Append Selection"), false, Puppet3DSelection.SaveSelectionAppend, i);
							menu.AddItem(new GUIContent("Store Pose"), false, Puppet3DSelection.StorePose, i);
							menu.AddItem(new GUIContent("Load Pose"), false, Puppet3DSelection.LoadPose, i);



							menu.ShowAsContext();
							Event.current.Use();

						}

					}
					GUI.Box(newLoadButtonPosition, "Load");

				}


			}
			if (currentGUIChoice == GUIChoice.AutoRig)
			{



				if (!HasGuides)
					GUI.DrawTexture(new Rect(20, 90 + offsetControls, 256, 256), autoRigTexture, ScaleMode.StretchToFill, true, 10.0F);
				else
					GUI.DrawTexture(new Rect(20, 90 + offsetControls, 256, 256), autoRigGuidesTexture, ScaleMode.StretchToFill, true, 10.0F);



				if (GUI.Button(new Rect(80, 70 + offsetControls, 150, 30), "Make Guides"))
				{
					if(AutoRigEditor.MakeGuides(Fingers))
						HasGuides = true;
				}
				if (GUI.Button(new Rect(80, 350 + offsetControls, 150, 30), "Auto Rig"))
				{
					AutoRigEditor.AutoRig();
					HasGuides = false;


				}
				Fingers = GUI.Toggle(new Rect(80, 385 + offsetControls, 256, 20), Fingers, " Rig Fingers");

				if (GUI.Button(new Rect(80, 410 + offsetControls, 150, 30), "Create Mod Rig"))
				{
					AutoRigEditor.CreateModRig(Fingers);

				}

				GUILayout.Space(400 + offsetControls);

				KeepVoxels = GUI.Toggle(new Rect(80, 460 + offsetControls, 256, 20), KeepVoxels, " Keep Voxels");
				GUILayout.Label(" Voxel Res", EditorStyles.boldLabel);

				VoxelScale = EditorGUI.Slider(new Rect(80, 480 + offsetControls, 150, 20), VoxelScale, 1f, VoxelScaleMax);


				GUILayout.Label(" Blur Iter", EditorStyles.boldLabel);

				BlurIter = EditorGUI.IntSlider(new Rect(80, 500 + offsetControls, 150, 20), BlurIter, 0, 20);

				string[] SkinningTypeChoice = { "Voxel", "Closest Point" };

				GUILayout.Label(" Skinning", EditorStyles.boldLabel);

				_skinningType = EditorGUI.Popup(new Rect(80, 530 + offsetControls, 150, 30), _skinningType, SkinningTypeChoice);

			}
			if (currentGUIChoice == GUIChoice.About)
			{

				GUILayout.Label(
					"    Puppet3D by Puppetman.\n\n" +
					"    Version 1.5\n\n" +
					"    v1.5:\n\n" +
					"    - New Blend Shapes (Beta)\n" +
					"    - Optimised Paint Weights Performance\n" +
					"    - Painting Weights on prerigged model converts it to asset\n" +
					"    - Fixed ModRig Serializing values\n" +
					"    - Default To Closest Point Skinning (Faster) \n\n" +
					"    v1.4:\n\n" +
					"    - Added Export Skin & Bones Feature\n" +
					"    - Fixed Voxel Creation\n" +
					"    - Fixed Voxel Weights Copying\n\n" +
					"    v1.3:\n\n" +
					"    - Fixed Voxel Skinning bug\n" +
					"    - Driven Keys execute from Global_CTRL\n" +
					"    \n" +
					"    v1.2:\n\n" +
					"    - Finger Rigging\n" +
					"    - Improved Voxel Skinning Algorithm\n" +
					"    - Driver Keys Rigging Feature\n" +
					"    - Fixed Error for 2018\n" +
					"    \n" +
					"    v1.1\n" +
					"    \n" +
					"    - Now Supports Submeshes\n" +
					"    - Fixed - Global control moves ModRig\n" +
					"    - Fixed - hold Ctrl to remove weights\n" +
					"" +
  "\n", EditorStyles.boldLabel);
			}
		}
		void OnFocus()
		{

			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

			SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		}

		void OnDestroy()
		{

			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

			
			Puppet3DSelection.SetSelectionString();
		}

		void OnSceneGUI(SceneView sceneView)
		{



			Event e = Event.current;

			switch (e.type)
			{
				case EventType.KeyDown:
					{
						if (Event.current.keyCode == (KeyCode.Return))
						{
							if (BoneCreation && Event.current.control != true)
								Puppet3D.BoneCreation.BoneFinishCreation();
							if (SplineCreation)
								SplineEditor.SplineFinishCreation();
							
							Repaint();

						}
						if (Event.current.keyCode == (KeyCode.KeypadPlus) && SkinWeightsPaint)
						{
							EditSkinWeightRadius += 0.2f;

						}
						if (Event.current.keyCode == (KeyCode.KeypadMinus) && SkinWeightsPaint)
						{
							EditSkinWeightRadius -= 0.2f;
						}
						if (BoneCreation && Event.current.control != true)
						{
							if (Event.current.keyCode == (KeyCode.Backspace))
							{
								Puppet3D.BoneCreation.BoneDeleteMode();
							}
						}
						if (SkinWeightsPaint)
						{

							if (Event.current.keyCode == (KeyCode.N))
							{

								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								if (!ChangingRadius)
								{
									ChangeRadiusStartPosition = worldRay.GetPoint(0);
									ChangeRadiusStartValue = paintWeightsStrength;
								}

								Skinning.ChangePaintStrength(worldRay.GetPoint(0));
								ChangingRadius = true;

							}
							if (Event.current.keyCode == (KeyCode.B))
							{

								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								if (!ChangingRadius)
								{
									ChangeRadiusStartPosition = worldRay.GetPoint(0);
									ChangeRadiusStartValue = EditSkinWeightRadius;
								}

								Skinning.ChangePaintRadius(worldRay.GetPoint(0));
								ChangingRadius = true;


							}
						}
						break;
					}
				case EventType.MouseMove:
					{
						if (Event.current.button == 0)
						{

							if (BoneCreation && Event.current.control != true)
							{

								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								
								if (Event.current.shift == true)
								{
									Puppet3D.BoneCreation.BoneMoveIndividualMode(worldRay.GetPoint(0), worldRay);
								}

							}
							if (SplineCreation && Event.current.control != true)
							{
								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								if ((Event.current.shift == true))
								{
									MoveControl(worldRay.GetPoint(0), worldRay);
								}
							}

						}
						break;
					}
				case EventType.MouseDown:
					{

						if (Event.current.button == 0)
						{

							if (BoneCreation && Event.current.control != true)
							{
								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
								
								
								Puppet3D.BoneCreation.BoneCreationMode(worldRay.GetPoint(0), worldRay);
								


							}
							else if (SplineCreation && Event.current.control != true)
							{
								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								SplineEditor.SplineCreationMode(worldRay.GetPoint(0), worldRay);
							}							
							else if (SkinWeightsPaint)
							{

								Undo.RegisterCompleteObjectUndo(currentSelectionMesh, "Paint Weights");
								
								GameObject c = HandleUtility.PickGameObject(Event.current.mousePosition, true);
								if (c && c.GetComponent<Bone>())
								{
									Selection.activeGameObject = c;
								}
							}


						}

						else if (Event.current.button == 1)
						{
							if (BoneCreation && Event.current.control != true)
							{
								Puppet3D.BoneCreation.BoneFinishCreation();
								Selection.activeObject = null;
								currentActiveBone = null;
								BoneCreation = true;

							}
							
						}
						break;

					}
				case EventType.KeyUp:
					{
						if (Event.current.keyCode == (KeyCode.B) || Event.current.keyCode == (KeyCode.N))
						{
							if (SkinWeightsPaint)
							{
								ChangingRadius = false;

							}
						}
						break;
					}
				case EventType.MouseDrag:
					{

				if (Event.current.button == 0)
				{
					
					if (BoneCreation && Event.current.control != true)
					{
						
						Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
								
						Puppet3D.BoneCreation.BoneMoveMode(worldRay.GetPoint(0), worldRay);
						
					}
					if ( SplineCreation && Event.current.control != true)
					{
						Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
						
						//if ((Event.current.shift == true))
						{
							MoveControl(worldRay.GetPoint(0), worldRay);
						}
					}
					
				}

						paintControlColor = new Color(.8f, 1f, .8f, .5f);



						if (SkinWeightsPaint && ! (Event.current.alt))
						{

							Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
							if (Event.current.control == true )
							{
								paintControlColor = new Color(1f, .8f, .8f, .5f);
								if (Event.current.button == 0)
									Skinning.PaintWeights(worldRay, -1);
							}
							else if (Event.current.shift == true/* Event.current.control == true*/)
							{
								paintControlColor = new Color(.8f, .8f, 1f, .5f);

								if (Event.current.button == 0)
									Skinning.PaintSmoothWeightsOld(worldRay);

							}
							
							else
							{
								paintControlColor = new Color(.8f, 1f, .8f, .5f);
								if (Event.current.button == 0)
									Skinning.PaintWeights(worldRay, 1);
							}

						}


						break;
					}
			}
			if (SkinWeightsPaint)
			{
				 // subscribe to the event

				
				if ((Event.current.type == EventType.ValidateCommand &&
					Event.current.commandName == "UndoRedoPerformed"))
				{

					RefreshMeshColors();

				}


				Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				if (ChangingRadius)
					Skinning.DrawHandle(worldRay);
				else
					Skinning.DrawHandle(worldRay);
				Repaint();
				SceneView.RepaintAll();



			}

			Handles.BeginGUI();
			
			if (SkinWeightsPaint)
			{
				Handles.color = Color.blue;
				Handles.Label(new Vector3(20, -40, 0),
							  "Select Bones to paint their Weights\n" +
							  "Left Click Adds Weights\n" +
							  "Left Click & Ctrl Removes Weights\n" +
							  "Left Click & Shift Smooths Weights\n" +
							  "Hold B to Change Brush Size\n" +
							  "Hold N to Change Strength");

			}
			Handles.EndGUI();
			if (BoneCreation || SplineCreation )
			{
				if (Event.current.control != true)
				{
					int controlID2 = GUIUtility.GetControlID(FocusType.Passive);
					HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
					HandleUtility.AddDefaultControl(controlID2);
				}
			}
			if ( SkinWeightsPaint)
			{
				
				int controlID2 = GUIUtility.GetControlID(FocusType.Passive);
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				HandleUtility.AddDefaultControl(controlID2);
				
			}
		}
		void OnUndoRedo()
		{
			if (SkinWeightsPaint)
			{

				Debug.Log("Undo");	

				
			}
		}
		public static float lastZ = 10f;
		void MoveControl(Vector3 mousePos, Ray ray1)
		{
			GameObject selectedGO = Selection.activeGameObject;
			if (selectedGO && selectedGO.transform && selectedGO.transform.parent)
			{
				Transform ctrlGrp = selectedGO.transform.parent;
				ctrlGrp.position = new Vector3(mousePos.x, mousePos.y, 0);


				RaycastHit hit;
				Undo.RecordObject(selectedGO.transform.parent, "Transform Position");

				if (Physics.Raycast(ray1, out hit))
				{
					RaycastHit newHit;
					if (Physics.Raycast(hit.point + 10f * ray1.direction, -1f * ray1.direction, out newHit))
						ctrlGrp.position = (hit.point + newHit.point) / 2f;
					else
						ctrlGrp.position = (hit.point);
					lastZ = Vector3.Distance(ray1.origin, ctrlGrp.position);

				}
				else
					ctrlGrp.position = ray1.GetPoint(lastZ);
			}


		}

		private static void RecursivelyFindFolderPath(string dir)
		{
			string[] paths = Directory.GetDirectories(dir);
			foreach (string s in paths)
			{
				if (s.Contains("Puppet3D"))
				{
					_puppet3DPath = s;
					break;
				}
				else
				{
					RecursivelyFindFolderPath(s);
				}
			}
		}

		void OnSelectionChange()
		{
			if (SkinWeightsPaint)
			{

				RefreshMeshColors();
			}
		}

		static void RefreshMeshColors()
		{
			
			SkinnedMeshRenderer smr = currentSelection.GetComponent<SkinnedMeshRenderer>();

			if (!BlackAndWhiteWeights)
			{

				Bone[] bones = FindObjectsOfType<Bone>();
				for (int j = 0; j < bones.Length; j++)
				{

					int boneIndx = smr.bones.ToList().IndexOf(bones[j].transform);
					float hue = ((float)boneIndx / smr.bones.Length);
					Color col = Color.HSVToRGB(hue * 1f, 1f, 1f);
					if (hue >= 0)
					{
						bones[j].Color = col;
					}

				}
			}

			if (currentSelection == null)
				return;

			GameObject c = Selection.activeGameObject;
			if (c && c.GetComponent<Bone>())
			{
				paintWeightsBone = c;

			}

			Vector3[] vertices = currentSelectionMesh.vertices;
			Color[] colrs = currentSelectionMesh.colors;
			BoneWeight[] bws = currentSelectionMesh.boneWeights;
			int boneIndex = smr.bones.ToList().IndexOf(paintWeightsBone.transform);
			//Debug.Log("pos is " +pos);
			if (BlackAndWhiteWeights && boneIndex > -1)
			{
				for (int i = 0; i < vertices.Length; i++)
				{
					
					colrs[i] = Color.black;

					
					
					if (bws[i].boneIndex0 == boneIndex)
					{
						float col = bws[i].weight0;
						colrs[i] = new Color(col, col, col);
					}
					else if (bws[i].boneIndex1 == boneIndex)
					{
						float col = bws[i].weight1;
						colrs[i] = new Color(col, col, col);
					}
					


				}
			}
			if (Puppet3DEditor.BlackAndWhiteWeights)
				Puppet3DEditor.currentSelectionMesh.colors = colrs;
			else
				Puppet3DEditor.currentSelectionMesh.colors = Skinning.SetColors(currentSelectionMesh.boneWeights);
		}

		void Start()
		{
			imageCount = 0;
		}
		void Update()
		{
			if (recordPngSequence && Application.isPlaying)
			{
				Time.captureFramerate = 30;

				recordDelta += Time.deltaTime;

				if (recordDelta >= 1 / 30)
				{
					imageCount++;

					SaveScreenshotToFile(pngSequPath + "." + imageCount.ToString("D4") + ".png");

					recordDelta = 0f;
				}
				Repaint();

			}
			if (!Application.isPlaying && imageCount > 0)
			{
				recordPngSequence = false;
				imageCount = 0;
				ExportPngAlpha = false;
			}


		}
		public static Texture2D TakeScreenShot()
		{
			return Screenshot();
		}

		static Texture2D Screenshot()
		{
			if (Camera.main == null)
			{
				Debug.LogWarning("Need a main camera in the scene");
				return null;
			}
			int resWidth = Camera.main.pixelWidth;
			int resHeight = Camera.main.pixelHeight;
			Camera camera = Camera.main;
			RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
			camera.targetTexture = rt;
			Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
			camera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			screenShot.Apply();
			camera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);
			return screenShot;
		}

		public static Texture2D SaveScreenshotToFile(string fileName)
		{
			Texture2D screenShot = Screenshot();
			byte[] bytes = screenShot.EncodeToPNG();
			Debug.Log("Saving " + fileName);

			System.IO.File.WriteAllBytes(fileName, bytes);
			return screenShot;
		}
	}
}
