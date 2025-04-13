using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SceneToolkit.Editor {
    public class SceneToolkit : EditorWindow {
        private Vector2 _scrollPos;
        private string[] _scenePaths;
        private HashSet<string> _favoriteScenes = new();

        private const string FavoritesKey = "SceneToolkit_Favorites";

        [MenuItem("Tools/Scene Toolkit")]
        public static void Open() {
            var window = GetWindow<SceneToolkit>("Scene Toolkit");
            window.Show();
        }

        private void OnEnable() {
            LoadFavorites();
            RefreshSceneList();
        }

        private void LoadFavorites() {
            var saved = EditorPrefs.GetString(FavoritesKey, "");
            _favoriteScenes = new HashSet<string>(saved.Split(';').Where(s => !string.IsNullOrEmpty(s)));
        }

        private void SaveFavorites() {
            var saveString = string.Join(";", _favoriteScenes);
            EditorPrefs.SetString(FavoritesKey, saveString);
        }

        private void RefreshSceneList() {
            _scenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(AssetDatabase.IsOpenForEdit)
                .OrderByDescending(path => _favoriteScenes.Contains(path)) // favourites to top
                .ThenBy(Path.GetFileNameWithoutExtension)
                .ToArray();
        }

        private void OnGUI() {
            if (GUILayout.Button("Refresh Scenes")) {
                RefreshSceneList();
            }

            GUILayout.Space(5);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach (var scenePath in _scenePaths) {
                EditorGUILayout.BeginHorizontal();

                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                bool isFavorite = _favoriteScenes.Contains(scenePath);

             
                GUIContent starIcon = EditorGUIUtility.IconContent(isFavorite ? "d_Favorite" : "d_FavoriteOverlay");
                if (GUILayout.Button(starIcon, GUILayout.Width(24), GUILayout.Height(18))) {
                    if (isFavorite)
                        _favoriteScenes.Remove(scenePath);
                    else
                        _favoriteScenes.Add(scenePath);

                    SaveFavorites();
                    RefreshSceneList();
                    break;
                }

                if (GUILayout.Button(sceneName)) {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
