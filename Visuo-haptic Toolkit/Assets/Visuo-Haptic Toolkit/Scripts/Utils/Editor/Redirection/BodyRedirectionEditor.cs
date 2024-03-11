using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using VHToolkit.Redirection.WorldRedirection;
using VHToolkit.Redirection.PseudoHaptics;

namespace VHToolkit.Redirection.BodyRedirection {
	/// <summary>
	/// Custom editor for the body redirection scene. Allows to show the Geslain2022Polynom parameters only if it is selected.
	/// </summary>
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty techniqueInstance;

		SerializedProperty physicalLimbs;
		SerializedProperty physicalHead;
		SerializedProperty virtualHead;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;
		SerializedProperty origin;
		SerializedProperty redirect;
		SerializedProperty parameters;
		SerializedObject parametersObject;

		readonly HashSet<string> bufferTechniques = new() { nameof(Han2018InterpolatedReach), nameof(Azmandian2016Body), nameof(Geslain2022Polynom), nameof(Cheng2017Sparse) };
		readonly HashSet<string> noThresholdTechniques = new() { nameof(Poupyrev1996GoGo), nameof(Lecuyer2000Swamp), nameof(Samad2019Weight) };


		private void OnEnable() {

			technique = serializedObject.FindProperty("_technique");
			techniqueInstance = serializedObject.FindProperty("techniqueInstance");

			physicalLimbs = serializedObject.FindProperty("scene.limbs");
			physicalHead = serializedObject.FindProperty("scene.physicalHead");
			virtualHead = serializedObject.FindProperty("scene.virtualHead");
			physicalTarget = serializedObject.FindProperty("scene.physicalTarget");
			virtualTarget = serializedObject.FindProperty("scene.virtualTarget");
			origin = serializedObject.FindProperty("scene.origin");
			redirect = serializedObject.FindProperty("redirect");

			parameters = serializedObject.FindProperty("scene.parameters");
			parametersObject = new SerializedObject(parameters.objectReferenceValue);
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((BodyRedirection)target), typeof(BodyRedirection), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("User Parameters", EditorStyles.largeLabel);

			// Scene
			EditorGUILayout.PropertyField(physicalLimbs, new GUIContent("Physical Limbs"));

			// EditorGUILayout.PropertyField(virtualLimbs, new GUIContent("Virtual Limbs"));
			if (technique.enumNames[technique.enumValueIndex] == nameof(Azmandian2016Hybrid)) {
				EditorGUILayout.PropertyField(physicalHead, new GUIContent("Physical Head"));
				EditorGUILayout.PropertyField(virtualHead, new GUIContent("Virtual Head"));
			}
			else if (technique.enumNames[technique.enumValueIndex] == nameof(Poupyrev1996GoGo)) {
				EditorGUILayout.PropertyField(physicalHead, new GUIContent("Physical Head"));
			}


			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Technique Parameters", EditorStyles.largeLabel);

			EditorGUILayout.PropertyField(technique, new GUIContent("Redirection technique"));

			EditorGUILayout.PropertyField(redirect, new GUIContent("Activate Redirection"));
			EditorGUILayout.PropertyField(parameters, new GUIContent("Numerical Parameters"));

			// If no parameters Scriptable object, update object and don't render the rest of the view
			if (parameters.objectReferenceValue == null) {
				serializedObject.ApplyModifiedProperties();
				return;
			}

			parametersObject = new SerializedObject(parameters.objectReferenceValue);
			parametersObject.Update();

			EditorGUILayout.PropertyField(physicalTarget, new GUIContent("Physical Target"));
			EditorGUILayout.PropertyField(virtualTarget, new GUIContent("Virtual Target"));
			EditorGUILayout.PropertyField(origin, new GUIContent("Origin"));

			// Hides redirectionLateness and controlpoint fields if the technique is not Geslain2022Polynom
			if (technique.enumNames[technique.enumValueIndex] == nameof(Geslain2022Polynom)) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("redirectionLateness"), new GUIContent("Redirection Lateness (a2)"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("controlPoint"), new GUIContent("ControlPoint"));
			} else if (technique.enumNames[technique.enumValueIndex] == nameof(Poupyrev1996GoGo)) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GoGoCoefficient"), new GUIContent("Coefficient"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GoGoActivationDistance"), new GUIContent("Activation Distance"));
			}

			if (bufferTechniques.Contains(technique.enumNames[technique.enumValueIndex])) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("RedirectionBuffer"), new GUIContent("Redirection Buffer"));
			}

			if (noThresholdTechniques.Contains(technique.enumNames[technique.enumValueIndex])) {
				if (technique.enumNames[technique.enumValueIndex] == nameof(Lecuyer2000Swamp)) {
					EditorGUILayout.PropertyField(parametersObject.FindProperty("SwampSquareLength"), new GUIContent("Square Side Length"));
					EditorGUILayout.PropertyField(parametersObject.FindProperty("SwampCDRatio"), new GUIContent("C/D Ratio"));
				}
			} else {
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("Threshold Parameters", EditorStyles.largeLabel);
				EditorGUILayout.PropertyField(parametersObject.FindProperty("HorizontalAngles"), new GUIContent("Max Horizontal Angles"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("VerticalAngles"), new GUIContent("Max Vertical Angles"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("Gain"), new GUIContent("Max Depth Gain"));
			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}