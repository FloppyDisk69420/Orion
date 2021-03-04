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
		Debug.Log("Turned UI On");
	}

	public void OnLeave() {
		foreach (GameObject element in elementList) {
			element.SetActive(false);
		}
		Debug.Log("Turned UI Off");
	}
}
