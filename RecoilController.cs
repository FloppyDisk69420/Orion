using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour {
	protected InputManager input;
	public bool fire = false;
	public GunStats stats;
	protected Vector3 returnRotation;
	public float recoilBase = 1f;
	protected Vector3 addedRotation;
	
	protected void Start() {
		SetNewReturnRotation();
		addedRotation = new Vector3();
	}

	public void SetNewReturnRotation() {
		returnRotation = transform.localEulerAngles;
	}
	
	virtual public void AddRecoil() {
		float recoilPower = recoilBase * stats.recoil;
		addedRotation.x -= recoilPower;
		addedRotation.y += Random.Range(-recoilPower, recoilPower);
		addedRotation.z += Random.Range(-recoilPower, recoilPower);
	}

	public void UpdateRecoil() {
		float speed = 1f;
		if (addedRotation.magnitude > recoilBase * stats.recoil / 10) {
			speed = 6f;
		}
		else {
			speed = 3f;
		}

		transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(returnRotation + addedRotation), Time.deltaTime * speed);
		addedRotation = Vector3.Lerp(addedRotation, Vector3.zero, Time.deltaTime * speed);
	}

	protected void Update() {
		if (fire) {
			AddRecoil();
			fire = false;
		}
	}
}
