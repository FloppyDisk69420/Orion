using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour {
	public GunStats stats;
	public bool fire = false;
	public float recoilBase = 1f;
	public float speed = 3f;
	public float returnTime = 2f;
	public float minSpeed = 0.1f;

	protected Vector3 returnRotation;
	protected Vector3 addedRotation;
	
	protected void Start() {
		addedRotation = new Vector3();
		returnRotation = transform.localEulerAngles;
	}
	
	virtual public void AddRecoil() {
		float recoilPower = recoilBase * stats.recoil;
		addedRotation.x -= recoilPower;
		addedRotation.y += Random.Range(-recoilPower, recoilPower);
		addedRotation.z += Random.Range(-recoilPower, recoilPower);
	}

	virtual public void UpdateRecoil() {
		float returnFactor = addedRotation.normalized.magnitude.Map(0, 1, minSpeed, returnTime);
		
		transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(addedRotation + returnRotation), Time.deltaTime * speed * returnFactor);

		addedRotation = Vector3.Lerp(addedRotation, Vector3.zero, Time.deltaTime * speed * returnFactor);
	}

	protected void Update() {
		if (fire) {
			AddRecoil();
			fire = false;
		}
	}
}
