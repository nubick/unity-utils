using UnityEngine;
using System.Collections;


public class GameOverGUI : MonoBehaviour
{
	const float
		kMenuWidth = 200.0f,
		kMenuHeight = 241.0f,
		kMenuHeaderHeight = 26.0f,
		kButtonWidth = 175.0f,
		kButtonHeight = 30.0f;
	const float
		kDeathCostFactor = 100.0f,
		kBuzzerKillPrize = 100.0f,
		kSpiderKillPrize = 50.0f,
		kMechKillPrize = 500.0f;


	public float delay = 3.0f, fadeSpeed = 1.0f;
	public Texture2D menuBackground, scoreHeader, restartButton;


	Texture2D background;
	#if !UNITY_FLASH
		float backgroundFade = 0.0f, guiFade = 0.0f;
		int recordedTime;
	#else
		float backgroundFade = 0.0f, guiFade = 1.0f;
	#endif
	int deaths, buzzerKills, spiderKills, mechKills, points;
	bool restarting = false;


	IEnumerator Start ()
	{
		#if !UNITY_FLASH
			const float kBackgroundTarget = 0.5f, kGUITarget = 1.0f;
		#endif

		CalculateScore ();

		#if !UNITY_FLASH
			Color color = Color.black;
		#else
			Color color = new Color (0.0f, 0.0f, 0.0f, 0.5f);
		#endif
		background = new Texture2D (2, 2);
		background.SetPixels (new Color[] {color, color, color, color});
		background.Apply ();

		yield return new WaitForSeconds (delay);

		#if !UNITY_FLASH
			do
			{
				backgroundFade = Mathf.Clamp (backgroundFade + Time.deltaTime * fadeSpeed, 0.0f, kBackgroundTarget);
				yield return null;
			}
			while (backgroundFade < kBackgroundTarget);

			do
			{
				guiFade = Mathf.Clamp (guiFade + Time.deltaTime * fadeSpeed, 0.0f, kGUITarget);
				yield return null;
			}
			while (guiFade < kGUITarget);
		#endif

		Screen.showCursor = true;
		Screen.lockCursor = false;
	}


	void CalculateScore ()
	{
		#if !UNITY_FLASH
			recordedTime = (int)GameScore.GameTime;
		#endif

		deaths = GameScore.Deaths;
		buzzerKills = GameScore.GetKills ("KamikazeBuzzer");
		spiderKills = GameScore.GetKills ("EnemySpider");
		mechKills = GameScore.GetKills ("EnemyMech") + GameScore.GetKills ("ConfusedEnemyMech");

		points = (int)(buzzerKills * kBuzzerKillPrize + spiderKills * kSpiderKillPrize + mechKills * kMechKillPrize);

		if (deaths != 0)
		{
			points /= (int)(deaths * kDeathCostFactor);
		}
	}


	void OnGUI ()
	{
		Color preColor = GUI.color;

		if (Event.current.type == EventType.repaint)
		{
			GUI.color = new Color (preColor.r, preColor.g, preColor.b, backgroundFade);
			GUI.DrawTexture (new Rect (0.0f, 0.0f, Screen.width, Screen.height), background);
		}

		GUI.color = new Color (preColor.r, preColor.g, preColor.b, guiFade);

		Rect menuRect = new Rect (
			(Screen.width - kMenuWidth) * 0.5f,
			(Screen.height - kMenuHeight) * 0.5f,
			kMenuWidth,
			kMenuHeight
		);

		GUI.DrawTexture (menuRect, menuBackground);
		GUI.DrawTexture (new Rect (menuRect.x, menuRect.y, scoreHeader.width, scoreHeader.height), scoreHeader);

		GUILayout.BeginArea (menuRect);
			GUILayout.Space (kMenuHeaderHeight);

			ScoreLine ("Buzzers disabled", buzzerKills);
			ScoreLine ("Spiders wrecked", spiderKills);
			ScoreLine ("Mechs destroyed", mechKills);

			GUILayout.Space (10.0f);

			ScoreLine ("Deaths", deaths);

			#if !UNITY_FLASH
				ScoreLine ("Time", string.Format ("{0}m {1}s", (int)recordedTime / 60, (int)recordedTime % 60));
			#endif

			GUILayout.Space (10.0f);

			ScoreLine ("Points", points);

			GUILayout.FlexibleSpace ();

			if (restarting)
			{
				GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
			}
			else if (MenuButton (restartButton))
			{
				StartCoroutine (DoRestart ());
			}

			GUILayout.FlexibleSpace ();

		GUILayout.EndArea ();

		GUI.color = preColor;
	}


	IEnumerator DoRestart ()
	{
		yield return null;
		restarting = true;
		yield return null;
		DemoControl.Restart ();
	}


	void ScoreLine (string label, object value)
	{
		GUILayout.BeginHorizontal ();
			GUILayout.Label (label);
			GUILayout.FlexibleSpace ();
			GUILayout.Label (value.ToString ());
		GUILayout.EndHorizontal ();
	}


	bool MenuButton (Texture2D icon)
	{
		bool wasPressed = false;

		GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();

			Rect rect = GUILayoutUtility.GetRect (kButtonWidth, kButtonHeight, GUILayout.Width (kButtonWidth), GUILayout.Height (kButtonHeight));

			switch (Event.current.type)
			{
				case EventType.MouseUp:
					if (rect.Contains (Event.current.mousePosition))
					{
						wasPressed = true;
					}
				break;
				case EventType.Repaint:
					GUI.DrawTexture (rect, icon);
				break;
			}

			GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

		return wasPressed;
	}
}
