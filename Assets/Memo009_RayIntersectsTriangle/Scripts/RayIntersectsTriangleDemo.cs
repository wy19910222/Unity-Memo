/*
 * @Author: wangyun
 * @CreateTime: 2024-07-16 22:09:30 128
 * @LastEditor: wangyun
 * @EditTime: 2024-07-16 22:09:30 133
 */

using System;
using UnityEngine;

public class RayIntersectsTriangleDemo : MonoBehaviour {
	public Transform rayCaster;
	public Transform[] trianglePoints;
	
	public Color rayColor = Color.cyan;
	public float rayOriginPointRadius = 0.1F;
	public Color triangleColor = Color.white;
	public Color intersectPointColor = Color.magenta;
	public float intersectPointRadius = 0.1F;
	
	private void OnDrawGizmos() {
		if (!rayCaster || trianglePoints.Length < 3 || Array.Exists(trianglePoints, point => !point)) {
			return;
		}
		
		Vector3 rayOrigin = rayCaster.position;
		Vector3 rayDirection = rayCaster.forward;
		Vector3[] triangle = Array.ConvertAll(trianglePoints, point => point.position);
		
		Gizmos.color = rayColor;
		Gizmos.DrawSphere(rayOrigin, rayOriginPointRadius);
		Gizmos.DrawRay(rayOrigin, rayDirection * int.MaxValue);
		
		Gizmos.color = triangleColor;
		Gizmos.DrawLine(triangle[0], triangle[1]);
		Gizmos.DrawLine(triangle[1], triangle[2]);
		Gizmos.DrawLine(triangle[2], triangle[0]);
		
		if (IntersectDetection.IsRayIntersectsTriangle(rayOrigin, rayDirection, triangle, out float distance)) {
			Gizmos.color = intersectPointColor;
			Gizmos.DrawSphere(rayOrigin + rayDirection * distance, intersectPointRadius);
		}
	}
}
