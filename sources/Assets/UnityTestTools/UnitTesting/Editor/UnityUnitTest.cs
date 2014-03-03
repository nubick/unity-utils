using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public abstract class UnityUnitTest
{
	protected List<GameObject> createdObjects = new List<GameObject> ();

	public GameObject CreateGameObject ()
	{
		return CreateGameObject ("");
	}

	public GameObject CreateGameObject ( string name )
	{
		var go = string.IsNullOrEmpty (name) ? new GameObject () : new GameObject (name);
		createdObjects.Add (go);
		return go;
	}

	[TearDown]
	public void TearDown ()
	{
		foreach (var createdObject in createdObjects)
		{
			GameObject.DestroyImmediate (createdObject);
		}
	}
}
