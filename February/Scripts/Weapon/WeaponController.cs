using UnityEngine;

public class WeaponController : MonoBehaviour {
	virtual public void Attack() {
		Debug.Log("We attacked with " + name);
	}

	virtual public void OnSwitch(GunProperties props) {
		Debug.Log("We switched gun!");
	}

	// not implemented
	virtual public void OnSwitch() {
		Debug.Log("We switched melee");
	}
}
