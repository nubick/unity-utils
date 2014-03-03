using UnityEngine;
using System.Collections;


public class DelayedDeactivation : MonoBehaviour
{
	public float delay = 2.0f;


	IEnumerator Start ()
	{
		yield return new WaitForSeconds (delay);
		gameObject.SetActive (false);
	}
}
