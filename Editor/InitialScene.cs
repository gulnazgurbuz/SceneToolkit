using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SceneToolkit.Editor {
	
	[InitializeOnLoad]
	public class InitialScene : SettingsProvider {
		private static string ProjectPath {
			get {
				var assetsPath = Application.dataPath;
				return Directory.GetParent(assetsPath)?.FullName;
			}
		}

		private static string PrefsKey => ProjectPath + ".InitialScene";

		private static string PrefsKeyEnabled => PrefsKey + ".enabled";

		private static bool Enabled => EditorPrefs.GetBool(PrefsKeyEnabled, true);

		static InitialScene() {
			EditorBuildSettings.sceneListChanged += UpdatePlayModeStartScene;
			UpdatePlayModeStartScene();
		}

		[SettingsProvider]
		public static SettingsProvider AutoSceneProvider() {
			var provider = new InitialScene("Project/SceneToolkit/Auto Scene", SettingsScope.Project);
			return provider;
		}
		public InitialScene(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

		
		private static void UpdatePlayModeStartScene() {
			SceneAsset sceneAsset = null;

			if (Enabled) {
				string value = EditorPrefs.GetString(PrefsKey, "none");
				if (value == "auto") {
					foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
						if (scene.enabled) {
							sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
							break;
						}
					}
				}
				else if (value != "none") {
					sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(value);
				}
			}

			EditorSceneManager.playModeStartScene = sceneAsset;
		}

		public override void OnGUI(string searchContext) {
			base.OnGUI(searchContext);
			
			var prefsValue = EditorPrefs.GetString(PrefsKey, "none");

			// Build scene list
			var guids = AssetDatabase.FindAssets("t:Scene");
			var paths = new string[guids.Length];
			for (int i = 0; i < guids.Length; i++) {
				paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
			}

			Array.Sort(paths, string.Compare);

			// Finding selected index
			var selectedIndex = 0;
			if (prefsValue == "auto") {
				selectedIndex = 1;
			}
			else {
				int arrayIndex = Array.IndexOf(paths, prefsValue);
				if (arrayIndex >= 0) {
					selectedIndex = arrayIndex + 2;
				}
			}

			var menuEntries = new string[paths.Length + 2];
			menuEntries[0] = "None";
			menuEntries[1] = "Initial";
			Array.Copy(paths, 0, menuEntries, 2, paths.Length);

			EditorGUI.BeginChangeCheck();

			var enabled = Enabled;
			enabled = EditorGUILayout.Toggle("Enable InitialScene", enabled);
			EditorGUILayout.Space();

			selectedIndex = EditorGUILayout.Popup("Scene to load on Play", selectedIndex, menuEntries);

			if (EditorGUI.EndChangeCheck()) {
				prefsValue = selectedIndex switch {
					0 => "none",
					1 => "auto",
					_ => menuEntries[selectedIndex]
				};

				EditorPrefs.SetString(PrefsKey, prefsValue);
				EditorPrefs.SetBool(PrefsKeyEnabled, enabled);
				UpdatePlayModeStartScene();
			}

			var helpBoxMessage = selectedIndex switch {
				0 => "The scenes currently loaded in the editor will be maintained when entering Play mode.\n\nThis is the default Unity behaviour.",
				1 => "The first enabled scene in the Build Settings box will be loaded when entering Play mode. If no such scene exists, falls back to 'None'.",
				_ =>$"The scene '{prefsValue}' will be loaded when entering Play mode. If the scene does not exist anymore, falls back to 'None'."
			};

			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(helpBoxMessage, MessageType.Info, true);
		}
	}
}