using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovementJoystick))]
public class MovementJoystickEditor : JoystickEditor
{
    private SerializedProperty mState;

    private SerializedProperty strengthCurve;
    private SerializedProperty threshold;

    private SerializedProperty backgroundState, thresholdState, handleState, mainNavCG, mysteryBox, activityFeed;

    protected override void OnEnable()
    {
        base.OnEnable();

        mState = serializedObject.FindProperty("m_state");

        strengthCurve = serializedObject.FindProperty("strengthCurve");
        threshold = serializedObject.FindProperty("threshold");

        backgroundState = serializedObject.FindProperty("backgroundState");
        thresholdState = serializedObject.FindProperty("thresholdState");
        handleState = serializedObject.FindProperty("handleState");

        mainNavCG = serializedObject.FindProperty("mainNavCG");
        mysteryBox = serializedObject.FindProperty("mysteryBox");
        activityFeed = serializedObject.FindProperty("activityFeed");
    }

    protected override void DrawValues()
    {
        base.DrawValues();

        EditorGUILayout.PropertyField(mState);

        EditorGUILayout.PropertyField(strengthCurve, new GUIContent("Strength Curve", "Adjust the intensity of input over distance from center."));
        EditorGUILayout.PropertyField(threshold, new GUIContent("Threshold", "Run threshold visual."));

        EditorGUILayout.PropertyField(backgroundState, true);
        EditorGUILayout.PropertyField(thresholdState, true);
        EditorGUILayout.PropertyField(handleState, true);

        EditorGUILayout.PropertyField(mainNavCG, true);
        EditorGUILayout.PropertyField(mysteryBox, true);
        EditorGUILayout.PropertyField(activityFeed, true);

    }

}