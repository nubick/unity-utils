using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.Editor
{
	public class MissingReferencesDetector
	{
		[MenuItem("Window/Utils/Check Missing References")]
		public static void CheckMissingReferences()
		{
			new MissingReferencesDetector().Check();
		}

		private void Check()
		{
			CheckScenes();
			CheckAssets();
			Debug.Log("Missing References Detection finish");
		}
		
		private void CheckScenes()
		{
			List<Scene> scenes = LoadAllScenes();
			foreach (Scene scene in scenes)
			{
				List<GameObject> sceneGameObjects = GetSceneGameObjects(scene);
				Debug.Log($"Scene: {scene.name}, game objects: {sceneGameObjects.Count}");
				CheckGameObjects(sceneGameObjects);
			}
		}
		
		private List<GameObject> GetSceneGameObjects(Scene scene)
		{
			List<GameObject> sceneGameObjects = new List<GameObject>();
			foreach (GameObject rootGameObject in scene.GetRootGameObjects())
				FetchHierarchy(rootGameObject.transform, sceneGameObjects);
			return sceneGameObjects;
		}

		private void FetchHierarchy(Transform rootTransform, List<GameObject> listToFill)
		{
			listToFill.Add(rootTransform.gameObject);
			for (int i = 0; i < rootTransform.childCount; i++)
				FetchHierarchy(rootTransform.GetChild(i), listToFill);
		}
		
		private List<Scene> LoadAllScenes()
		{
			foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
				EditorSceneManager.OpenScene(buildScene.path, OpenSceneMode.Additive);
			
			List<Scene> scenes = new List<Scene>();
			for (int index = 0; index < SceneManager.sceneCount; index++)
				scenes.Add(SceneManager.GetSceneAt(index));
			return scenes;
		}

		private void CheckGameObjects(List<GameObject> gameObjects)
		{
			gameObjects.ForEach(CheckGameObject);
		}
		
		private void CheckGameObject(GameObject gameObject)
		{
			Component[] allComponents = gameObject.GetComponents<Component>();
			foreach (Component component in allComponents)
			{
				if (component == null)
					Debug.LogError($"Missing: Component in game object '{GetPath(gameObject.transform)}'");
				else
					CheckComponent(component, gameObject);
			}
		}

		private void CheckComponent(Component component, GameObject ownerGameObject)
		{
			SerializedObject serializedObject = new SerializedObject(component);
			SerializedProperty property = serializedObject.GetIterator();
			while (property.NextVisible(enterChildren: true))
			{
				if (property.propertyType == SerializedPropertyType.ObjectReference)
				{
					if (property.objectReferenceValue == null && property.objectReferenceInstanceIDValue > 0)
					{
						string propertyName = ObjectNames.NicifyVariableName(property.name);
						Debug.LogError($"Missing: property '{propertyName}', component: '{component.GetType().Name}', game object: {GetPath(ownerGameObject.transform)}");
					}
				}
			}
		}

		private string GetPath(Transform transform)
		{
			Transform parent = transform.parent;
			return parent == null ? transform.name : $"{GetPath(parent)}/{transform.name}";
		}

		private void CheckAssets()
		{
			List<GameObject> assetsGameObjects = GetAssetsGameObjects();
			Debug.Log($"Check assets, game objects: {assetsGameObjects.Count}");
			CheckGameObjects(assetsGameObjects);
		}

		private List<GameObject> GetAssetsGameObjects()
		{
			List<GameObject> gameObjects = new List<GameObject>();
			string[] paths = AssetDatabase.GetAllAssetPaths();
			foreach (string path in paths)
			{
				if(!path.StartsWith("Assets/"))
					continue;
				
				GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);

				if (gameObject == null)
					continue;
				
				FetchHierarchy(gameObject.transform, gameObjects);
			}
			return gameObjects;
		}
	}
}