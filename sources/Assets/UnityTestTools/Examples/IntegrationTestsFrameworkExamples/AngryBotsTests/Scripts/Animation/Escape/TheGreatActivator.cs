using UnityEngine;
using System.Collections;

public class TheGreatActivator : MonoBehaviour
{
	public GameObject[] targets;
	public float delay = 2.5f;


	IEnumerator Start ()
	{
		yield return new WaitForSeconds (delay);

		foreach (GameObject target in targets)
		{
			target.SetActive (true);
		}
	}
}
