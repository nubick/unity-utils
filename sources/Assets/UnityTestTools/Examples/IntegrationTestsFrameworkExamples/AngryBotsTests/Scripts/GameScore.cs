using UnityEngine;
using System.Collections.Generic;


public class GameScore : MonoBehaviour
{
	static GameScore instance;
	
	
	static GameScore Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (GameScore)FindObjectOfType (typeof (GameScore));
			}
			
			return instance;
		}
	}
	
	
	void OnApplicationQuit ()
	{
		instance = null;
	}
	
	
	public string playerLayerName = "Player", enemyLayerName = "Enemies";
	
	
	int deaths = 0;
	Dictionary<string, int> kills = new Dictionary<string, int> ();
	float startTime = 0.0f;
	
	
	public static int Deaths
	{
		get
		{
			if (Instance == null)
			{
				return 0;
			}
			
			return Instance.deaths;
		}
	}
	
	
	#if !UNITY_FLASH
		public static ICollection<string> KillTypes
		{
			get
			{
				if (Instance == null)
				{
					return new string[0];
				}
				
				return Instance.kills.Keys;
			}
		}
	#endif
	
	
	public static int GetKills (string type)
	{
		if (Instance == null || !Instance.kills.ContainsKey (type))
		{
			return 0;
		}
		
		return Instance.kills[type];
	}
	
	
	public static float GameTime
	{
		get
		{
			if (Instance == null)
			{
				return 0.0f;
			}
			
			return Time.time - Instance.startTime;
		}
	}
	
	
	public static void RegisterDeath (GameObject deadObject)
	{
		if (Instance == null)
		{
			Debug.Log ("Game score not loaded");
			return;
		}
		
		int
			playerLayer = LayerMask.NameToLayer (Instance.playerLayerName),
			enemyLayer = LayerMask.NameToLayer (Instance.enemyLayerName);
			
		if (deadObject.layer == playerLayer)
		{
			Instance.deaths++;
		}
		else if (deadObject.layer == enemyLayer)
		{
			Instance.kills[deadObject.name] = Instance.kills.ContainsKey (deadObject.name) ? Instance.kills[deadObject.name] + 1 : 1;
		}
	}
	
	
	void OnLevelWasLoaded (int level)
	{
		if (startTime == 0.0f)
		{
			startTime = Time.time;
		}
	}
}
