using UnityEngine;

public static class IntegrationTest
{
	public const string passMessage = "IntegrationTest Pass";
	public const string failMessage = "IntegrationTest Fail";

	public static void Pass(GameObject go)
	{
		go = FindTopGameObject(go);
		LogResult(go, passMessage);
	}

	public static void Fail (GameObject go, string reason)
	{
		Fail (go);
		Debug.Log (reason);
	}

	public static void Fail (GameObject go)
	{
		go = FindTopGameObject (go);
		LogResult(go, failMessage);
	}

	private static void LogResult (GameObject go, string message)
	{
		Debug.Log(message + " (" + FindTopGameObject(go).name + ")",
					go);
	}

	private static GameObject FindTopGameObject(GameObject go)
	{
		while (go.transform.parent != null)
			go = go.transform.parent.gameObject;
		return go;
	}
}
