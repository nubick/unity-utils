using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
	public class CallMethodScriptTester : MonoBehaviour
	{
		public Text TextComponent;

		private void Log(string message)
		{
			Debug.Log(message);
			TextComponent.text = message;
		}

		public void MethodA()
		{
			Log("MethodA called. No Parameters.");
		}

		public void MethodStr1(string strParameter)
		{
			Log("MethodStr1 called, str: " + strParameter);
		}

		private void MethodStr2(string strParameter1, string strParameter2)
		{
			Log("MethodStr2 called, str1: " + strParameter1 + ", str2: " + strParameter2);
		}

		public void MethodStr3(string strParameter1, string strParameter2, string strParameter3)
		{
			Log("MethodStr3 called, str1: " + strParameter1 + ", str2: " + strParameter2 + ", str3: " + strParameter3);
		}

		public void MethodInt1(int intParameter)
		{
			Log("MethodInt1 called, int: " + intParameter);
		}

		private void MethodInt2(int intParameter1, int intParameter2)
		{
			Log("MethodInt2 called, int1: " + intParameter1 + ", int2: " + intParameter2);
		}

		public void MethodInt3(int intParameter1, int intParameter2, int intParameter3)
		{
			Log("MethodInt3 called, int1: " + intParameter1 + ", int2: " + intParameter2 + ", int3: " + intParameter3);
		}

		public void MethodFloat1(float floatParameter)
		{
			Log("MethodFloat1 called, float: " + floatParameter);
		}

		private void MethodFloat2(float floatParameter1, float floatParameter2)
		{
			Log("MethodFloat2 called, float1: " + floatParameter1 + ", float2: " + floatParameter2);
		}

		public void MethodFloat3(float floatParameter1, float floatParameter2, float floatParameter3)
		{
			Log("MethodFloat3 called, float1: " + floatParameter1 + ", float2: " + floatParameter2 + ", float3: " + floatParameter3);
		}

		public void MethodMix(string strParameter, int intParameter, float floatParameter)
		{
			Log("MethodMix called, str: " + strParameter + ", int: " + intParameter + ", float: " + floatParameter);
		}

		public int MethodRetInt(string strParameter, float floatParameter)
		{
			Log("MethodRetInt, str: " + strParameter + ", float: " + floatParameter);
			return 123;
		}

		public void MethodColor(Color color)
		{
			Log("MethodColor, color: " + color);
		}

		public void MethodObject(CallMethodScriptTester tester)
		{
			Log("MethodObject, tester: " + tester.name);
		}

		public void MethodObject2(Sprite sprite)
		{
			Log("MethodObject2, sprite: " + sprite.name);
		}

		public IEnumerator CoroutineWithoutParameters()
		{
			Log("Coroutine step 1. Please wait.");
			yield return new WaitForSeconds(3f);
			Log("Coroutine step 2. Please wait.");
			yield return new WaitForSeconds(3f);
			Log("Coroutine step 3. The latest one. It is finished now");
		}

		public IEnumerator CoroutineWithDelayParameter(float delay)
		{
			Log("Coroutine step 1. Delay: " + delay);
			yield return new WaitForSeconds(delay);
			Log("Coroutine is finished after delay.");
		}

		public void MethodBool1(bool isDone)
		{
			Log($"MethodBool1: {isDone}.");
		}

		public void MethodBool2(string strParameter, bool isCool)
		{
			Log($"MethodBool2: {strParameter}, {isCool}.");
		}
	}
}