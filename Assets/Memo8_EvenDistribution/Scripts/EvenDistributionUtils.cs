using UnityEngine;

public static class EvenDistributionUtils {
	public static Vector3[] DistributionInCircle(int pointCount, float dstPow, float turnFraction) {
		Vector3[] points = new Vector3[Mathf.Max(pointCount, 0)];
		DistributionInCircle(points, dstPow, turnFraction);
		return points;
	}
	public static void DistributionInCircle(Vector3[] points, float dstPow, float turnFraction) {
		int pointCount = points.Length;
		switch (pointCount) {
			case > 1:
				for (int i = 0; i < pointCount; i++) {
					float t = i / (pointCount - 1F);
					float dst = Mathf.Lerp(0, 1, Mathf.Pow(t, dstPow));	// 以幂函数分布点与圆心的距离
					float angle = 2 * Mathf.PI * turnFraction * i;	// 计算螺旋角度
					float x = dst * Mathf.Cos(angle);	// 计算x轴坐标
					float y = dst * Mathf.Sin(angle);	// 计算y轴坐标
					points[i] = new Vector3(x, y, 0);
				}
				break;
			case 1:
				points[0] = Vector3.zero;
				break;
		}
	}

	public static Vector3[] DistributionInSphere(int pointCount, float fieldOfView, float turnFraction) {
		Vector3[] points = new Vector3[Mathf.Max(pointCount, 0)];
		DistributionInCircle(points, fieldOfView, turnFraction);
		return points;
	}

	public static void DistributionInSphere(Vector3[] points, float fieldOfView, float turnFraction) {
		int pointCount = points.Length;
		switch (pointCount) {
			case > 1:
				float minZ = Mathf.Cos(fieldOfView * 0.5F * Mathf.Deg2Rad);
				for (int i = 0; i < pointCount; i++) {
					float t = i / (pointCount - 1F);
					float z = Mathf.Lerp(1, minZ, t);	// 线性分布z轴
					float dst = Mathf.Sqrt(1 - z * z);	// 通过z轴坐标计算点与z轴的距离
					float angle = 2 * Mathf.PI * turnFraction * i;	// 计算螺旋角度(z轴)
					float x = dst * Mathf.Cos(angle);	// 计算x轴坐标
					float y = dst * Mathf.Sin(angle);	// 计算y轴坐标
					points[i] = new Vector3(x, y, z);
				}
				break;
			case 1:
				points[0] = Vector3.forward;
				break;
		}
	}
}
