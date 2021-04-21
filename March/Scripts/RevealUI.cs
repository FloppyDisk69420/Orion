using System.Collections.Generic;
using UnityEngine;

public class RevealUI : MonoBehaviour {
	public List<GameObject> elementList = new List<GameObject>();

	void Start() {
		OnLeave();
	}
	
	public void OnEnter() {
		foreach (GameObject element in elementList) {
			element.SetActive(true);
		}
	}

	public void OnLeave() {
		foreach (GameObject element in elementList) {
			element.SetActive(false);
		}
	}
}
