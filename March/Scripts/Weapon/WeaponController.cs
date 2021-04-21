using UnityEngine;

public class WeaponController : MonoBehaviour {
	virtual public void Attack() {
		Debug.Log("We attacked with " + name);
	}
}
