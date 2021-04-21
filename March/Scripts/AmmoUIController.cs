using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUIController : MonoBehaviour {
	public GunProperties props;
	public TMP_Text magazine;
	public TMP_Text capacity;
	public Image icon;
	public Sprite[] icons;

	void Update() {
		UpdateUI();
	}

	void UpdateUI() {
		if (props) {
			// checks if the are the same
			if (magazine.text != props.controller.rounds.ToString()) {
				// if not update it
				magazine.text = props.controller.rounds.ToString();
			}
			if (capacity.text != props.stats.ammo.ToString()) {
				capacity.text = props.stats.ammo.ToString();
			}

			// check if the icon matches the current
			if (icon.sprite != icons[(int)props.stats.type]) {
				// if not then update it
				icon.sprite = icons[(int)props.stats.type];
				// makes visible
				icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
			}
		}
		else {
			magazine.text = "-";
			capacity.text = "-";
			icon.sprite = null;
			// make transparent
			icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
		}
	}
}
