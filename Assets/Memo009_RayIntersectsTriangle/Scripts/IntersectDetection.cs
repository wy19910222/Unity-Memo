/*
 * @Author: wangyun
 * @CreateTime: 2024-07-16 22:04:21 573
 * @LastEditor: wangyun
 * @EditTime: 2024-07-16 22:04:21 582
 */

using UnityEngine;

public static class IntersectDetection {
	/// <summary>
	/// 参考：如何判断三维空间中射线是否与三角形相交 https://zhuanlan.zhihu.com/p/687077146
	/// 记射线起点为O，方向为D，三角形顶点为V1、V2、V3，交点P = O + tD = (1 - u - v)V1 + uV2 + vV3。其中t为射线起点到交点的距离，(1 - u - v)、u、v为交点的质心坐标。
	/// 记(V2 - V1)为E1、(V3 - V1)为E2，由 O + tD = (1 - u - v)V1 + uV2 + vV3 得到 O - V1 = -tD + u(V2 - V1) + v(V3 - V1) = -tD + uE1 + vE2。
	/// ┏ -D.x    E1.x    E2.x ┓┏ t ┓   ┏ (O - V1).x ┓              
	/// ┃ -D.y    E1.y    E2.y ┃┃ u ┃ = ┃ (O - V1).y ┃，为了方便，记为 Ax = c ，记 Ai 为 c 取代了 A 的第 i 列得到的新矩阵。
	/// ┗ -D.z    E1.z    E2.z ┛┗ t ┛   ┗ (O - V1).z ┛                
	/// 若det(A)不为零，则射线与三角面不平行，根据克莱默法则可得：
	/// t = det(A1)/det(A) = (O-v1)×E1·E2 / (D×E2·E1)
	/// u = det(A2)/det(A) = D×E2·(O-v1) / (D×E2·E1)
	/// v = det(A3)/det(A) = (O-v1)×E1·D / (D×E2·E1)
	/// 若 t ≥ 0 && u ≥ 0 && v ≥ 0 && u + v ≤ 1 ，则有交点。
	/// </summary>
	/// <param name="rayOrigin">射线起点</param>
	/// <param name="rayDirection">射线方向单位向量</param>
	/// <param name="triangle">三角形的三个顶点坐标</param>
	/// <param name="distance">返回射线起点到交点的距离</param>
	/// <returns>是否相交</returns>
	public static bool IsRayIntersectsTriangle(Vector3 rayOrigin, Vector3 rayDirection, Vector3[] triangle, out float distance) {
		distance = 0;

		Vector3 edge1 = triangle[1] - triangle[0];
		Vector3 edge2 = triangle[2] - triangle[0];
		Vector3 rayCrossE2 = Vector3.Cross(rayDirection, edge2);	// D×E2
		float detA = Vector3.Dot(rayCrossE2, edge1);
		if (detA > -Mathf.Epsilon && detA < Mathf.Epsilon) {
			// 射线与三角面平行
			return false;
		}
		float invertedDetA = 1F / detA;
		Vector3 oToV1 = rayOrigin - triangle[0];	// O-v1
		float u = Vector3.Dot(rayCrossE2, oToV1) * invertedDetA;
		if (u < 0 || u > 1) {
			return false;
		}
		Vector3 oToV1CrossE1 = Vector3.Cross(oToV1, edge1);	// (O-v1)×E1
		float v = Vector3.Dot(oToV1CrossE1, rayDirection) * invertedDetA;
		if (v < 0 || u + v > 1) {
			return false;
		}
		distance = Vector3.Dot(oToV1CrossE1, edge2) * invertedDetA;
		return distance >= 0;
	}
}
