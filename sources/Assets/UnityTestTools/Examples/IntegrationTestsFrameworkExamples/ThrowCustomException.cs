using System;
using UnityEngine;

public class ThrowCustomException : MonoBehaviour
{
	public void Start ()
	{
		throw new CustomException ();
	}

	private class CustomException : Exception
	{
	}
}
