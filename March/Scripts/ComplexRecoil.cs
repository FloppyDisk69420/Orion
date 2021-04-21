using UnityEngine;

public class ComplexRecoil : RecoilController {
	[Header("Advanced Settings")]
    public float recoilVertBase = 2f;
	public float recoilHorizBase = 1f;
	[Space()]
	public bool affectX = true;
	public bool affectY = true;
	public bool affectZ = true;
	[Space()]
	[Range(0, 1)] public float switchPercent = 0.8f;
	public float maxAddedRot = 12f;
	public float snapSpeed = 3f;
	public float returnSpeed = 0.2f;
	
	override public void AddRecoil() {
		float recoilPower = recoilBase * stats.recoil;
		if (affectX) addedRotation.x -= recoilPower * recoilVertBase;
		if (affectY) addedRotation.y += Random.Range(-recoilPower * recoilHorizBase, recoilPower * recoilHorizBase);
		if (affectZ) addedRotation.z += Random.Range(-recoilPower * recoilHorizBase, recoilPower * recoilHorizBase);
	}

	override public void UpdateRecoil() {
		float speed = 0f;
		if (addedRotation.magnitude >= maxAddedRot * switchPercent) speed = snapSpeed;
		else speed = returnSpeed;

		transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(addedRotation + returnRotation), Time.deltaTime * speed);

		addedRotation = Vector3.Lerp(addedRotation, Vector3.zero, Time.deltaTime * speed);

		addedRotation = Vector3.ClampMagnitude(addedRotation, maxAddedRot);
	}
}
