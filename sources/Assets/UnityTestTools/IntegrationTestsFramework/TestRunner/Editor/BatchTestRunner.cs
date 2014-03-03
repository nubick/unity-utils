using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BatchTestRunner : MonoBehaviour
{
	const string testScenesParam = "-testscenes=";

	public static void RunIntegrationTests ()
	{
		Debug.Log ("RunIntegrationTests");
		EditorApplication.NewScene ();
		var go  = new GameObject ("BatchTestRunner");
		go.AddComponent<BatchTestRunner> ();
		DontDestroyOnLoad (go);
		EditorApplication.isPlaying = true;
	}

	public void Awake ()
	{
		var sceneList = GetTestScenesList ();
		if (sceneList.Count == 0)
		{
			Debug.Log ("No scenes on the list");
			EditorApplication.Exit (0);
			return;
		}
		EditorBuildSettings.scenes = sceneList.Select (s => new EditorBuildSettingsScene (s, true)).ToArray ();
	}

	public void Start()
	{
		Application.LoadLevel (1);
	}
	
	private List<string> GetTestScenesList ()
	{
		var sceneList = new List<string> ();
		foreach (var arg in Environment.GetCommandLineArgs ())
		{
			if (arg.ToLower ().StartsWith (testScenesParam))
			{
				var scenesFromParam = arg.Substring (testScenesParam.Length).Split (',');
				foreach (var scene in scenesFromParam)
				{
					var sceneName = scene;
					if (!sceneName.EndsWith (".unity"))
						sceneName += ".unity";
					var foundScenes = Directory.GetFiles (Directory.GetCurrentDirectory (), sceneName, SearchOption.AllDirectories);
					if (foundScenes.Length == 1)
						sceneList.Add (foundScenes[0]);
					else
						Debug.Log (sceneName + " not found or multiple entries found");
				}
			}
		}
		return sceneList;
	}
}
