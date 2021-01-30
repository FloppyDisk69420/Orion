using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexRecoil : RecoilController {
	[Header("Advanced Settings")]
    public float recoilVertBase = 2f;
	public float recoilHorizBase = 1f;

	public float lossPercent = .1f;
	public bool affectX = true;
	public bool affectY = true;
	public bool affectZ = true;

	override public void AddRecoil() {
		float recoilPower = recoilBase * stats.recoil;
		if (affectX) addedRotation.x -= recoilPower * recoilVertBase;
		if (affectY) addedRotation.y += Random.Range(-recoilPower * recoilHorizBase, recoilPower * recoilHorizBase);
		if (affectZ) addedRotation.z += Random.Range(-recoilPower * recoilHorizBase, recoilPower * recoilHorizBase);

		returnRotation += lossPercent * addedRotation;
	}


}
