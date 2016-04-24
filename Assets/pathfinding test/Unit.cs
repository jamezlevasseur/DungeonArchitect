using UnityEngine;
using System.Collections;

public abstract class Unit : Saveable {

	public Transform target;
	protected float unitSpeed = 20f;
	protected Vector3[] path;
	protected int targetIndex;
	protected voidCallback arrivedAtDestinationCallback;

	public void OnPathFound (Vector3[] newPath, bool pathSuccess) {
		if (pathSuccess) {
			path = newPath;
			if (newPath.Length>0) {
				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
		} else {
			print("EMPTY PATH");
		}
	}

	IEnumerator FollowPath () {
		if (path.Length==0) {
			StopCoroutine("FollowPath");
			yield break;
		}
		Vector3 currentWayPoint = path[0];
		while (true) {
			if (transform.position==currentWayPoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					targetIndex = 0;
					path = new Vector3[0];
					if (arrivedAtDestinationCallback!=null)
						arrivedAtDestinationCallback();
					yield break;
				}
				currentWayPoint = path[targetIndex];
			}
			transform.position = Vector3.MoveTowards(transform.position,currentWayPoint,unitSpeed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos () {
		if (path!=null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i==targetIndex) {
					Gizmos.DrawLine(transform.position,path[i]);
				} else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}

}
