using System.Collections;
using UnityEngine;

public class DiscoverPlayer : MonoBehaviour {
	public AIMovement movement;
	public EnemyStats stats;
	// enemy viewing vars
	public float viewDist = 25f;
	[Range(0f, 360f)] public float FOV = 55f;
	[Range(0f, 180f)] public float viewHeight = 30f;
	// enemy detection vars
	public LayerMask obstacleMask;
	public LayerMask targetMask;
	public bool targetInView = false;
	public float susCounter;
	[Range(0, 1)] public float susSpeed = 0.1f;
	[Range(0, 1)] public float susDown = 0.01f;
	public float[] susChecks;
	public float lookSpeed = 20f;
	
	void Start() {
		susChecks = new float[4];
		// route area
		susChecks[0] = 0.2f;
		// kinda sus area
		susChecks[1] = 0.3f;
		// sus area
		susChecks[2] = 0.5f;
		// alert area
		susChecks[3] = 1f;
		targetInView = false;
		onUI = false;
	}

	void Update() {
		FindTargetsInView();
		UICheck();
	}

	public Vector3 DirFromAngle(float angle, bool global = false) {
		if (!global) {
			angle += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
	}

	public void FindTargetsInView() {
		Collider[] viewableTargets = Physics.OverlapSphere(transform.position, viewDist, targetMask);
		foreach (Collider target in viewableTargets) {
			Transform t = target.transform;
			Vector3 dir = (t.position - transform.position).normalized;

			float hAngle = Vector3.Angle(transform.up, transform.forward + new Vector3(0, dir.y, 0)) - 90;
			float wAngle = Vector3.Angle(transform.forward, new Vector3(dir.x, 0, dir.z));
			// if in view width angle
			if (wAngle < FOV / 2 && wAngle > -FOV / 2) {
				// if in view height angle
				if (hAngle < viewHeight / 2 && hAngle > -viewHeight / 2) {
					// and no obstacles block it
					float dist = Vector3.Distance(transform.position, t.position);
					RaycastHit hit;
					if (!Physics.Raycast(transform.position, dir, out hit, dist, obstacleMask)) {
						float susMeter = dist;
						susMeter = susMeter.Map(0, viewDist, 0, 1);
						susMeter = 1 - susMeter;
						susCounter += susSpeed * susMeter * Time.deltaTime;
						targetInView = true;
					}
					else targetInView = false;
				}
				else targetInView = false;
			}
			else targetInView = false;
		}
		susCounter -= susDown * Time.deltaTime;
		susCounter = Mathf.Clamp01(susCounter);
	}

	public void DoStates() {
		for (int i = susChecks.Length - 1; i >= 0; i--) {
			if (i == 0) {
				if (movement.onRoute) stats.state = EnemyState.DoRoute;
				else stats.state = EnemyState.Idle;
			}
			else {
				if (susCounter <= susChecks[i] && susCounter > susChecks[i - 1]) {
					stats.state = (EnemyState) i + 1;
					break;
				}
			}
		}
	}

	bool onUI = false;
	public void UICheck() {
		if (susCounter >= susChecks[3] && !onUI) {
			transform.Find("VGFX").SendMessage("OnEnter");
		}
		else if (onUI) {
			transform.Find("VGFX").SendMessage("OnLeave");
		}
	}
}
