using UnityEngine;
using System.Collections;

public abstract class Minion : Unit {
	
	public const int STARTING_LEVEL = 1;
	public enum MinionStatDescriptor {Weak, Intermediate, Strong};
	public Material selectedMat;
	//The AI's speed per second
	//public float speed = 100;
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;

	//The waypoint we are currently moving towards
	protected int currentWaypoint = 0;
	protected int shouldBeMovingCount;
	protected Vector3 lastLoc;
	protected CharacterController controller;
	protected Material unselectedMat;
	protected float lastWander;
	protected bool canWander, shouldBeMoving;
	protected voidCallback reachedDestinationCallback,pathingFailedCallback;

	/*
	str, agi, wis, arm, and spd should all exist in a 1-10 range
	hp should be anywhere from 100 (very low) to 500 (low) to 1000 (intermediate) to 10,000 (boss health)
	 */
	public abstract int STR { get; }
	public abstract int AGI { get; }
	public abstract int WIS { get; }
	public abstract int SPD { get; }
	public abstract int TotalHP { get; }
	public abstract int HP { get; }
	public abstract int ARM { get; }
	public abstract int Level { get; }
	public abstract int EXP { get; }
	public abstract string ClassName { get; }
	public abstract string MinionName { get; }
	public abstract Texture MinionPicture { get; }

	protected virtual void Start () {
		unselectedMat = gameObject.GetComponent<MeshRenderer>().material;
		controller = GetComponent<CharacterController>();
		MinionController.minionController.addUnselectedUnit (this);
		MinionController.minionController.addNewUnit (this);
		canWander = true;
		reachedDestinationCallback = setupToWander;
		pathingFailedCallback = setupToWander;
		//Start a new path to the targetPosition, return the result to the OnPathComplete function
	}

	protected virtual void Update () {
		if (Time.time - lastWander > 5 && canWander) {
			//50% chance to wander
			if (Random.Range(1,101)>20) {
				wander ();
				lastWander = Time.time;
			} else {
				//else wait a few seconds before trying to wander again
				lastWander = Time.time;
			}
		}
		//minionPathing ();
	}

	public void goTo (Vector3 targetPosition) {
		canWander = false;
		//seeker.StartPath (transform.position,targetPosition, OnPathComplete);
	}

	protected void wander () {
		return;
		int dir = Random.Range(1,5);
		int low = 4;
		int high = 8;
		Vector3 forScale = new Vector3 (Random.Range(low,high),0,Random.Range(low,high));
		switch (dir) {
		case 1:
			//forward
			PathRequestManager.RequestPath (transform.position,transform.position+Vector3.Scale(Vector3.forward, forScale), base.OnPathFound);
			break;
		case 2:
			//backward
			PathRequestManager.RequestPath (transform.position,transform.position+Vector3.Scale(Vector3.back, forScale), base.OnPathFound);
			break;
		case 3:
			//left
			PathRequestManager.RequestPath (transform.position,transform.position+Vector3.Scale(Vector3.left, forScale), base.OnPathFound);
			break;
		case 4:
			//right
			PathRequestManager.RequestPath (transform.position,transform.position+Vector3.Scale(Vector3.right, forScale), base.OnPathFound);
			break;
		default:
			break;
		}
	}

	protected void setupToWander() {
		currentWaypoint = 0;
		canWander = true;
		lastWander = Time.time+10;
		//path = null;
	}

	protected void minionPathing () {/*
		if (path == null) {
			//We have no path to move after yet
			return;
		}
		if (currentWaypoint >= path.vectorPath.Count) {
			Debug.Log ("End Of Path Reached");
			shouldBeMoving = false;
			shouldBeMovingCount = 0;
			lastLoc = Vector3.zero;
			reachedDestinationCallback();
			return;
		}
		if (shouldBeMoving) {
			if (lastLoc==Vector3.zero) {
				lastLoc = transform.position;
			} else {
				if (lastLoc == transform.position)
					shouldBeMovingCount++;
				if (shouldBeMovingCount>25) {
					Debug.Log("REALLY SHOULD HAVE MOVED BY NOW");
				}
			}
		}
		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir *= speed * Time.deltaTime;
		controller.SimpleMove (dir);
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}*/
	}

	public override void wasUnselected () {
		gameObject.GetComponent<MeshRenderer> ().material = unselectedMat;
		GameController.gamecontroller.surrenderContextualMenu ();
	}

	public override void wasSelected () {
		gameObject.GetComponent<MeshRenderer> ().material = selectedMat;
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Minion;
		GameController.gamecontroller.ContextualMenuCallback = minionContextualMenuCallback;
	}

	void minionContextualMenuCallback () {
		GameController.minionContentualMenu (this);
	}

	protected void OnMouseDown () {
		MinionController.minionController.deselectUnits ();
		MinionController.minionController.addSelectedUnit (this);
		wasSelected ();
	}

	public static int getStartingStat (MinionStatDescriptor type) {
		switch (type) {
		case MinionStatDescriptor.Weak:
			return Random.Range(1,4);
		case MinionStatDescriptor.Intermediate:
			return Random.Range(4,7);
		case MinionStatDescriptor.Strong:
			return Random.Range(7,10);
		default:
			return Random.Range(4,7);
		}
	}

	public static int getStartingHP (MinionStatDescriptor type) {
		switch (type) {
		case MinionStatDescriptor.Weak:
			return Random.Range(100,400);
		case MinionStatDescriptor.Intermediate:
			return Random.Range(400,700);
		case MinionStatDescriptor.Strong:
			return Random.Range(700,1000);
		default:
			return Random.Range(400,700);
		}
	}

	protected abstract void initStats ();
	protected abstract void setUnitText ();

}