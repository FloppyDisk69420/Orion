using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Route))]
public class RouteEditor : Editor {
	public override void OnInspectorGUI() {
		Route route = (Route)target;
		for (int i = 0; i < route.checkpoints.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			route.checkpoints[i] = EditorGUILayout.Vector3Field("Checkpoint " + (i + 1), route.checkpoints[i]);
			route.times[i] = EditorGUILayout.FloatField("    Wait Time", route.times[i]);
			EditorGUILayout.EndVertical();
			if (GUILayout.Button("-", GUILayout.MaxWidth(20), GUILayout.MinHeight(40f))) {
				route.RemoveCheckpoint(i);
			}
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add")) {
			route.AddCheckpoint();
		}
	}

	[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void DrawGizmosSelected(Route route, GizmoType gizmoType) {
		if (route.checkpoints.Count > 0) {
			Gizmos.color = Color.red;
			for (int i = 0; i < route.checkpoints.Count; i++) {
				Gizmos.DrawSphere(route.checkpoints[i], 0.125f);

				// draws route
				if (i != route.checkpoints.Count - 1) {
					Gizmos.DrawLine(route.checkpoints[i], route.checkpoints[i + 1]);
				}
				else {
					Gizmos.DrawLine(route.checkpoints[i], route.checkpoints[0]);
				}
			}
		}
    }

	void OnSceneGUI() {
		Route t = (Route)target;
		// Set the colour of the next handle to be drawn
		Handles.color = Color.magenta;
		for (int i = 0; i < t.checkpoints.Count; i++) {
			EditorGUI.BeginChangeCheck();
			Vector3 lookTarget = Handles.PositionHandle(t.checkpoints[i], Quaternion.identity);

			if( EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(target, "Changed Look Target");
				t.checkpoints[i] = lookTarget;
			}
		}
	}
}
