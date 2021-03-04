using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic {
	public float thickness = 1f;
	public List<Vector2> points = new List<Vector2>();
	public Color32 vertexColor;
	protected override void OnPopulateMesh(VertexHelper vh) {
		vh.Clear();

		for (int i = 1; i < points.Count; i++) {
			Vector2 n1, n2, n3, n4;
			SetVertices(points[i - 1], points[i], out n1, out n2, out n3, out n4);
			vh.AddVert(n1, color, new Vector2(1, 1));
		}
	}

	void SetVertices(Vector2 v1, Vector2 v2, out Vector2 n1, out Vector2 n2, out Vector2 n3, out Vector2 n4) {
		n1 = v1 + Vector2.down * thickness;
		n2 = v1 + Vector2.up * thickness;

		n3 = v2 + Vector2.down * thickness;
		n4 = v2 + Vector2.up * thickness;
		
		float angle = Vector2.Angle(v1, v2);
		
		
	}
}
