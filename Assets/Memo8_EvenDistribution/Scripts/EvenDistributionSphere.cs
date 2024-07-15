using UnityEngine;

public class EvenDistributionSphere : MonoBehaviour {
	[Range(0, 360)]
	public float fieldOfView = 60;
	public float distance = 1;
	public int pointCount = 300;
	[Range(0, 1)]
	public float turnFraction = (Mathf.Sqrt(5) - 1) / 2;

	private void Reset() {
		fieldOfView = 60;
		distance = 1;
		pointCount = 300;
		turnFraction = (Mathf.Sqrt(5) - 1) / 2;
	}

	private void OnDrawGizmos() {
		Vector3[] points = EvenDistributionUtils.DistributionInSphere(pointCount, fieldOfView, turnFraction);
		Vector3 selfPos = transform.position;
		foreach (var point in points) {
			Vector3 targetPos = selfPos + point * distance;
			Gizmos.DrawLine(selfPos, targetPos);
			Gizmos.DrawSphere(targetPos, 0.01F);
		}
	}
}
