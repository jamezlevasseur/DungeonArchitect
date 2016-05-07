using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GnollPeon : Minion, Worker {

	public static int foodCost = 2;

	public override int FoodCost { get {return foodCost;} set {foodCost=value;} }

	//private
	private bool isWorking,isDigging;
	private GameObject targetDigNode;


	protected override void Start () {
		base.Start ();
		onSpawn ();
		initStats ();
		volunteerToDig ();
		base.reachedDestinationCallback = doNothing;
	}

	protected override void initStats () {
		unitFaction = Faction.Neutral;
		myWeapon = WeaponObject.GetComponent<Weapon>();
		defaultAttack = new MeleeAttack(myWeapon, this);
		str = getStartingStat (UnitStatDescriptor.Strong);
		agi = getStartingStat (UnitStatDescriptor.Strong);
		wis = getStartingStat (UnitStatDescriptor.Strong);
		spd = getStartingStat (UnitStatDescriptor.Strong);
		arm = getStartingStat (UnitStatDescriptor.Strong);
		totalhp = getStartingHP (UnitStatDescriptor.Strong);
		hp = totalhp;
		className = "Gnoll Peon";
		unitName = "Gnoll Peon #"+Random.Range(1,5001);
		level = Minion.STARTING_LEVEL;
	}
	
	public void volunteerToDig () {
		DigNodeManager.digNodeManager.diggers.Add (this);
	}

	public void resignFromDig () {
		DigNodeManager.digNodeManager.diggers.Remove (this);
	}

	public bool canDig (GameObject nodeToDig) {
		if (isWorking)
			return false;
		Debug.Assert (nodeToDig != null);
		targetDigNode = nodeToDig;
		StartCoroutine (goToDigNode());
		isWorking = true;
		return true;
	}

	IEnumerator goToDigNode () {
		goTo (targetDigNode.transform.position);
		base.arrivedAtDestinationCallback = arrivedAtDigNode;
		yield return null;
	}

	public void finishedDig() {

	}

	public override void assertbgm ()
	{
		ButtonGridManager bgm = base.getMinionbgm();
		ButtonGrid root = bgm.getGrid(0);
		if (IsAssignedToStation) {
			root.insertNewCallback(5,releaseFromStation,"quit work");
		}
		GameController.gamecontroller.Foreignbgm = bgm;
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Minion;
	}

	public void cancelCurrentDigJob() {
		targetDigNode = null;
		setupForNextDig ();
		isDigging = false;
	}

	public void setupForNextDig() {
		targetDigNode = null;
		isWorking = false;
	}

	void arrivedAtDigNode () {
		StopCoroutine("digThatNode");
		StartCoroutine("digThatNode");
	}

	IEnumerator digThatNode () {
		if (!isWorking) {
			setupForNextDig();
			yield return null;
		}
		try {
			DigNode node = targetDigNode.GetComponent<DigNode>();
			DungeonResources.Gold+=node.goldWorth;
			node.dig();
		} catch (System.NullReferenceException e) {
			Debug.Log(e.Message);
		}
		yield return null;
		DigNodeManager.digNodeManager.updateGrid();
		yield return new WaitForSeconds (2f);
		setupForNextDig();
		isDigging = false;
		yield return null;
	}

	public void digPathFound(Vector3[] newPath, bool pathSuccess) {
		print ("DIGPATHFOUND");
		if (pathSuccess) {
			base.path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		} else {
			Debug.Log("FAILED JOB");
			DigNodeManager.digNodeManager.failedJob(targetDigNode.GetComponent<DigNode>());
			setupForNextDig();
			isDigging = false;
		}
	}

	public override void onSpawn () {
		DungeonResources.Food+=foodCost;
	}

	public override void onDeath () {
		DungeonResources.Food-=foodCost;
	}
	
	public override string getData () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("type",GameController.GameObjectType.GnollPeon);
		dict.Add("stats",statsJSON());
		return Utils.DictionaryToJSON(dict);
	}

	public override void releaseFromStation () {
		isAssignedToStation = false;
		volunteerToDig();
		station.minionLeftStation(this);
		transform.position = GameController.safeSpawnPoint;
	}

	public override void assignToStation (Vector3 placeToGo, Structure _station) {
		print ("ASSIGN TO NEW THING");
		resignFromDig();
		cancelCurrentDigJob();
		transform.position = placeToGo;
		station = _station;
		isAssignedToStation = true;
	}

	//combat stuff

	public override bool attack() {return false;}

}
