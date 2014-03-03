using UnityEngine;
using System.Collections;

public class InitLevelManager : MonoBehaviour
{
	public Texture2D splash;
	
	
	private Texture2D background;
	private bool loading = true;
	
	
	void Start ()
	{
		background = new Texture2D (2, 2);
		background.SetPixels (new Color[] {Color.black, Color.black, Color.black, Color.black});
		background.Apply ();
		
		DontDestroyOnLoad (gameObject);
		Application.LoadLevel (Application.loadedLevel + 1);
	}
	
	
	void OnLevelWasLoaded (int level)
	{
		loading = false;
	}
	
	
	void OnGUI ()
	{
		if (!loading)
		{
			return;
		}
		
		float splashWidth = splash.width, splashHeight = splash.height;
		
		if (splashWidth > Screen.width)
		{
			float scale = Screen.width / splashWidth;
			splashWidth *= scale;
			splashHeight *= scale;
		}
		
		GUI.DrawTexture (new Rect (0.0f, 0.0f, Screen.width, Screen.height), background);
		
		GUI.DrawTexture (
			new Rect (
				(Screen.width - splashWidth) * 0.5f,
				(Screen.height - splashHeight) * 0.5f,
				splashWidth,
				splashHeight
			),
			splash
		);
	}
}
