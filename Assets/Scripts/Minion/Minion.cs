using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Minion : Unit, Selectable, SerializableJSON, GameID {
	
	public const int STARTING_LEVEL = 1;
	public enum MinionStatDescriptor {Weak, Intermediate, Strong};
	public Material selectedMat;

	//The waypoint we are currently moving towards
	protected int currentWaypoint = 0;
	protected Vector3 lastLoc;
	protected CharacterController controller;
	protected Material unselectedMat;
	protected float lastWander;
	protected bool canWander;
	protected voidCallback reachedDestinationCallback,pathingFailedCallback;

	/*
	str, agi, wis, arm, and spd should all exist in a 1-10 range
	hp should be anywhere from 100 (very low) to 500 (low) to 1000 (intermediate) to 10,000 (boss health)
	 */
	public abstract int FoodCost { get;set; }
	public abstract int STR { get;set; }
	public abstract int AGI { get;set; }
	public abstract int WIS { get;set; }
	public abstract int SPD { get;set; }
	public abstract int TotalHP { get;set; }
	public abstract int HP { get;set; }
	public abstract int ARM { get;set; }
	public abstract int Level { get;set; }
	public abstract int EXP { get;set; }
	public abstract string ClassName { get;set; }
	public abstract string MinionName { get;set; }
	public abstract Texture MinionPicture { get;set; }
	public abstract bool IsAssignedToStation { get;set; }
	public long ID {get{return id;}}

	protected override void Start () {
		base.Start();
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

	public virtual void goTo (Vector3 targetPosition) {
		if (!IsAssignedToStation)
			PathRequestManager.RequestPath (transform.position,targetPosition, pathFound);
	}

	public void pathFound(Vector3[] newPath, bool pathSuccess) {
		if (pathSuccess) {
			print ("FOUND PATH");
			base.path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		} else {
			Debug.Log("FAILED TO FIND PATH");
		}
	}

	protected void wander () {
		/*
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
		}*/
	}

	protected void setupToWander() {
		currentWaypoint = 0;
		canWander = true;
		lastWander = Time.time+10;
		//path = null;
	}

	protected string statsJSON () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("id",id);
		dict.Add("str",STR);
		dict.Add("agi",AGI);
		dict.Add("wis",WIS);
		dict.Add("spd",SPD);
		dict.Add("hp",HP);
		dict.Add("totalhp",TotalHP);
		dict.Add("arm",ARM);
		dict.Add("level",Level);
		dict.Add("exp",EXP);
		dict.Add("className",ClassName);
		dict.Add("minionName",MinionName);
		dict.Add("foodCost",FoodCost);
		dict.Add("isAssignedToStation",IsAssignedToStation);
		dict.Add("position",transform.position.ToString());
		dict.Add("rotation",transform.localRotation.ToString());
		return Utils.DictionaryToJSON(dict);
	}

	public override void syncStats (Hashtable stats) {
		transform.position = Utils.vector3FromString((string)stats["position"]);
		id = long.Parse((string)stats["id"]);
		STR = int.Parse((string)stats["str"]);
		AGI = int.Parse((string)stats["agi"]);
		WIS = int.Parse((string)stats["wis"]);
		SPD = int.Parse((string)stats["spd"]);
		TotalHP = int.Parse((string)stats["totalhp"]);
		ARM = int.Parse((string)stats["arm"]);
		EXP = int.Parse((string)stats["exp"]);
		Level = int.Parse((string)stats["level"]);
		FoodCost = int.Parse((string)stats["foodCost"]);
		ClassName = (string)stats["className"];
		MinionName = (string)stats["minionName"];
		IsAssignedToStation = bool.Parse((string)stats["isAssignedToStation"]);
	}

	protected void destroyMinion () {
		GameController.gamecontroller.Foreignbgm = null;
		Destroy(gameObject);
	}

	protected ButtonGridManager getMinionbgm () {
		ButtonGrid root = new ButtonGrid();
		root.insertNewCallback(8,destroyMinion,"Kill");
		ButtonGridManager structurebgm = new ButtonGridManager(root);
		return structurebgm;
	}

	public virtual void wasUnselected () {
		gameObject.GetComponent<MeshRenderer> ().material = unselectedMat;
		GameController.gamecontroller.surrenderContextualMenu ();
	}

	public virtual void assertbgm () {
		GameController.gamecontroller.Foreignbgm = getMinionbgm();
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Minion;
	}

	public virtual void wasSelected () {
		MinionController.minionController.deselectUnits ();
		MinionController.minionController.addSelectedUnit (this);
		gameObject.GetComponent<MeshRenderer> ().material = selectedMat;
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Minion;
		GameController.gamecontroller.ContextualMenuCallback = minionContextualMenuCallback;
		assertbgm();
	}

	void minionContextualMenuCallback () {
		GameController.minionContentualMenu (this);
	}

	protected void OnMouseDown () {
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
	
	public abstract void releaseFromStation ();
	public abstract void assignToStation (Vector3 placeToGo, Structure station);
	protected abstract void initStats ();
	protected abstract void setUnitText ();
	//add food to food cost
	public abstract void onSpawn ();
	//subtract food from food cost
	public abstract void onDeath ();

}
