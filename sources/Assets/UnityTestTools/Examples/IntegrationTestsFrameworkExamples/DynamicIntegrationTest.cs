using System;
using UnityEngine;

[IntegrationTest.DynamicTest("ExampleIntegrationTests")]
//[IntegrationTest.Ignore]
[IntegrationTest.ExpectExceptions (false, typeof (ArgumentException))]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout(1)]
[IntegrationTest.ExcludePlatform(RuntimePlatform.Android, RuntimePlatform.LinuxPlayer)]
public class DynamicIntegrationTest : MonoBehaviour 
{
	void Start()
	{
		IntegrationTest.Pass(gameObject);
	}
}
