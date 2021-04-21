using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreateWeaponPickup))]
public class CreateWeaponPickupEditor : Editor {
	SerializedProperty weaponProp;
	SerializedProperty pickupEffectProp;
	SerializedProperty maskProp;

	void OnEnable() {
		weaponProp = serializedObject.FindProperty("weapon");
		pickupEffectProp = serializedObject.FindProperty("pickupEffect");
		maskProp = serializedObject.FindProperty("mask");
	}

	public override void OnInspectorGUI() {
		EditorGUILayout.PropertyField(weaponProp, new GUIContent("Weapon"));
		EditorGUILayout.PropertyField(pickupEffectProp, new GUIContent("Pickup Effect"));
		EditorGUILayout.PropertyField(maskProp, new GUIContent("Mask"));

		serializedObject.ApplyModifiedProperties();

		CreateWeaponPickup cwp = target as CreateWeaponPickup;
		if (cwp.weapon) {
			if (GUILayout.Button("Create Weapon Pickup")) {
				cwp.CreatePickup();
			}
		}
	}
}
