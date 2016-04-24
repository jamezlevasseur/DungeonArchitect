using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GnollPeon : Minion, Worker {

	public static int foodCost = 2;

	//stats
	private int str;
	private int agi;
	private int wis;
	private int spd;
	private int hp;
	private int totalhp;
	private int arm;
	private int level;
	private int exp;
	private string className;
	private string minionName;
	
	//public stats
	public override int FoodCost { get {return foodCost;} set {foodCost=value;} }
	public override int STR { get {return str;} set {str=value;} }
	public override int AGI { get {return agi;} set {agi=value;} }
	public override int WIS { get {return wis;} set {wis=value;} }
	public override int SPD { get {return spd;} set {spd=value;} }
	public override int HP { get {return hp;} set {hp=value;} }
	public override int TotalHP { get {return totalhp;} set {totalhp=value;} }
	public override int ARM { get {return arm;} set {arm=value;} }
	public override int Level { get {return level;} set {level=value;} }
	public override int EXP { get {return exp;} set {exp=value;} }
	public override string ClassName { get {return className;} set {className=value;} }
	public override string MinionName { get {return minionName;} set {minionName=value;} }
	public override Texture MinionPicture { get {return minionPicture;} set {minionPicture=value;} }
	public override bool IsAssignedToStation { get{return isAssignedToStation;} set {isAssignedToStation=value;} }
	
	//public
	public Texture minionPicture;
	
	//private
	private bool isWorking,isDigging,isAssignedToStation;
	private GameObject targetDigNode;
	private Structure station;

	protected override void Start () {
		base.Start ();
		onSpawn ();
		initStats ();
		setUnitText ();
		volunteerToDig ();
		base.reachedDestinationCallback = doNothing;
	}

	void doNothing () {

	}
	
	protected override void Update () {
		base.Update ();
	}

	void OnMouseOver () {
		if (Input.GetMouseButtonDown(1)) {

		}
	}
	
	protected override void initStats () {
		str = Minion.getStartingStat (Minion.MinionStatDescriptor.Strong);
		agi = Minion.getStartingStat (Minion.MinionStatDescriptor.Strong);
		wis = Minion.getStartingStat (Minion.MinionStatDescriptor.Strong);
		spd = Minion.getStartingStat (Minion.MinionStatDescriptor.Strong);
		arm = Minion.getStartingStat (Minion.MinionStatDescriptor.Strong);
		totalhp = Minion.getStartingHP (Minion.MinionStatDescriptor.Strong);
		hp = totalhp;
		className = "Gnoll Peon";
		minionName = "Gnoll Peon #"+Random.Range(1,5001);
		level = Minion.STARTING_LEVEL;
	}
	
	protected override void setUnitText () {
		GUIText miniontext = (GUIText) gameObject.AddComponent<GUIText> ();
		miniontext.text = minionName+" – Level: "+level;
		
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
		gnollPeonGoTo (targetDigNode.transform.position);
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
		//base.setupToWander ();
	}

	public void gnollPeonGoTo (Vector3 targetPosition) {
		canWander = false;
		PathRequestManager.RequestPath (transform.position,targetPosition, digPathFound);
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

}
