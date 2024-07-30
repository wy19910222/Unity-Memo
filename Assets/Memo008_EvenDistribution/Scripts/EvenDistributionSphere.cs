using UnityEngine;

public class EvenDistributionSphere : MonoBehaviour {
	[Range(0, 360)]
	public float fieldOfView = 60;
	public float distance = 1;
	public int pointCount = 300;
	[Range(0, 1)]
	public float turnFraction = (Mathf.Sqrt(5) - 1) / 2;

	public bool drawLine = true;
	public Color lineColor = new Color(1, 1, 1, 0.1F);
	public bool drawSphere = true;
	public Color sphereColor = Color.white;

	private Vector3[] points;

	private void Reset() {
		fieldOfView = 60;
		distance = 1;
		pointCount = 300;
		turnFraction = (Mathf.Sqrt(5) - 1) / 2;
		
		drawLine = true;
		lineColor = new Color(1, 1, 1, 0.1F);
		drawSphere = true;
		sphereColor = Color.white;
	}

	private void OnDrawGizmos() {
		if (points == null || points.Length != pointCount) {
			points = new Vector3[pointCount];
		}
		EvenDistributionUtils.DistributionInSphere(points, fieldOfView, turnFraction);
		
		Vector3 selfPos = transform.position;
		foreach (var point in points) {
			Vector3 targetPos = selfPos + point * distance;
			if (drawLine) {
				Gizmos.color = lineColor;
				Gizmos.DrawLine(selfPos, targetPos);
			}
			if (drawSphere) {
				Gizmos.color = sphereColor;
				Gizmos.DrawSphere(targetPos, 0.01F);
			}
		}
	}
}
