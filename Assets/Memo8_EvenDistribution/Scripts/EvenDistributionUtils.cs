using UnityEngine;

public static class EvenDistributionUtils {
	public static Vector3[] DistributionInCircle(int pointCount, float dstPow, float turnFraction, Vector3[] result = null) {
		result ??= new Vector3[pointCount];
		for (int i = 0; i < pointCount; i++) {
			float t = i / (pointCount - 1F);
			float dst = Mathf.Lerp(0, 1, Mathf.Pow(t, dstPow));
			float angle = 2 * Mathf.PI * turnFraction * i;
			float x = dst * Mathf.Cos(angle);
			float y = dst * Mathf.Sin(angle);
			result[i] = new Vector3(x, y, 0);
		}
		return result;
	}

	public static Vector3[] DistributionInSphere(int pointCount, float fieldOfView, float turnFraction, Vector3[] result = null) {
		result ??= new Vector3[pointCount];
		float minZ = Mathf.Cos(fieldOfView * 0.5F * Mathf.Deg2Rad);
		for (int i = 0; i < pointCount; ++i) {
			float t = i / (pointCount - 1F);
			float z = Mathf.Lerp(minZ, 1, t);
			float dst = Mathf.Sqrt(1 - z * z);
			float az = 2 * Mathf.PI * turnFraction * i;
			float x = dst * Mathf.Cos(az);
			float y = dst * Mathf.Sin(az);
			result[i] = new Vector3(x, y, z);
		}
		return result;
	}
}
