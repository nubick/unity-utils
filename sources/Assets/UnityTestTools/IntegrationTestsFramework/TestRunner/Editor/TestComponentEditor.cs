using UnityEditor;
using UnityEngine;

namespace UnityTest
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof (TestComponent))]
	public class TestComponentEditor : Editor
	{
		private SerializedProperty testName;
		private SerializedProperty timeout;
		private SerializedProperty ignored;
		private SerializedProperty succeedAssertions;
		private SerializedProperty expectException;
		private SerializedProperty expectedExceptionList;
		private SerializedProperty succeedWhenExceptionIsThrown;

		#region GUI Contens
		private readonly GUIContent guiTestName = new GUIContent ("Test name", "Name of the test (is equal to the GameObject name)");
		private readonly GUIContent guiIncludePlatforms = new GUIContent ("Included platforms", "Platform on which the test should run");
		private readonly GUIContent guiTimeout = new GUIContent("Timeout", "Number of seconds after which the test will timeout");
		private readonly GUIContent guiIgnore= new GUIContent("Ignore", "Ignore the tests in runs");
		private readonly GUIContent guiSuccedOnAssertions= new GUIContent("Succeed on assertions", "Succeed after all assertions are executed");
		private readonly GUIContent guiExpectException= new GUIContent ("Expect exception", "Should the test expect an exception");
		private readonly GUIContent guiExpectExceptionList = new GUIContent ("Expected exception list", "A comma separated list of exception types which will not fail the test when thrown");
		private readonly GUIContent guiSucceedWhenExceptionIsThrown = new GUIContent ("Succeed when exception is thrown", "Should the test succeed when an expected exception is thrown");
		#endregion

		public void OnEnable ()
		{
			timeout = serializedObject.FindProperty ("timeout");
			ignored = serializedObject.FindProperty ("ignored");
			succeedAssertions = serializedObject.FindProperty ("succeedAfterAllAssertionsAreExecuted");
			expectException = serializedObject.FindProperty ("expectException");
			expectedExceptionList = serializedObject.FindProperty ("expectedExceptionList");
			succeedWhenExceptionIsThrown = serializedObject.FindProperty ("succeedWhenExceptionIsThrown");
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			if (!serializedObject.isEditingMultipleObjects)
			{
				var component = (TestComponent) target;
				component.name = EditorGUILayout.TextField (guiTestName, component.name);
				component.includedPlatforms = (TestComponent.IncludedPlatforms)EditorGUILayout.EnumMaskField (guiIncludePlatforms, component.includedPlatforms, EditorStyles.popup);
			}
			EditorGUILayout.PropertyField( timeout, guiTimeout);
			EditorGUILayout.PropertyField( ignored, guiIgnore);
			EditorGUILayout.PropertyField( succeedAssertions, guiSuccedOnAssertions);
			EditorGUILayout.PropertyField (expectException, guiExpectException);
			if (expectException.boolValue)
			{
				EditorGUILayout.PropertyField (expectedExceptionList, guiExpectExceptionList);
				EditorGUILayout.PropertyField (succeedWhenExceptionIsThrown, guiSucceedWhenExceptionIsThrown);
			}
			
			if (serializedObject.ApplyModifiedProperties() || GUI.changed)
			{
				TestManager.InvalidateTestList ();
			}
		}
	}
}
