using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DiscoverPlayer))]
public class DiscoverPlayerEditor : Editor {
	void OnSceneGUI() {
		DiscoverPlayer dp = (DiscoverPlayer)target;
		Transform look = dp.transform.Find("Look");
		Handles.color = Color.white;
		
		// view angles
		Quaternion downRot = Quaternion.AngleAxis(dp.viewHeight / 2, dp.transform.right);
		Quaternion upRot = Quaternion.AngleAxis(-dp.viewHeight / 2, dp.transform.right);
		Vector3 angleA = dp.DirFromAngle(-dp.FOV / 2) * dp.viewDist;
		Vector3 angleB = dp.DirFromAngle(dp.FOV / 2) * dp.viewDist;
		// ray vars
		Vector3 r1 = look.position + upRot * angleA;
		Vector3 r2 = look.position + downRot * angleA;
		Vector3 r3 = look.position + upRot * angleB;
		Vector3 r4 = look.position + downRot * angleB;
		// rays
		Handles.DrawLine(look.position, r1);
		Handles.DrawLine(look.position, r2);
		Handles.DrawLine(look.position, r3);
		Handles.DrawLine(look.position, r4);
		// borders
		Handles.DrawLine(r1, r2);
		Handles.DrawLine(r1, r3);
		Handles.DrawLine(r3, r4);
		Handles.DrawLine(r4, r2);
	}
}
