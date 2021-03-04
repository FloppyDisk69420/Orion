using UnityEngine;

public static class AddonMath {
	public static float Map(this float num, float min, float max, float scaleMin, float scaleMax) {
		if (num >= max)  {
			return scaleMax;
		}
		else if (num <= min) {
			return scaleMin;
		}
    	float newNum = (((num - min) * (scaleMax - scaleMin)) / (max - min)) + scaleMin;
		return newNum;
	}
}