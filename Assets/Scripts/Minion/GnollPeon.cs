using UnityEngine;
using System.Collections;

public class GnollPeon : Worker {

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
	public override int FoodCost { get {return foodCost;} }
	public override int STR { get {return str;} }
	public override int AGI { get {return agi;} }
	public override int WIS { get {return wis;} }
	public override int SPD { get {return spd;} }
	public override int HP { get {return hp;} }
	public override int TotalHP { get {return totalhp;} }
	public override int ARM { get {return arm;} }
	public override int Level { get {return level;} }
	public override int EXP { get {return exp;} }
	public override string ClassName { get {return className;} }
	public override string MinionName { get {return minionName;} }
	public override Texture MinionPicture { get {return minionPicture;} }
	
	//public
	public Texture minionPicture;
	
	//private
	private bool isWorking,isDigging;
	private GameObject targetDigNode;

	protected override void Start () {
		base.Start ();
		onSpawn ();
		initStats ();
		setUnitText ();
		volunteerToDig ();
		base.reachedDestinationCallback = doNothing;
		//base.pathingFailedCallback = 
	}

	void doNothing () {

	}
	
	protected override void Update () {
		base.Update ();

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
	
	public override void volunteerToDig () {
		DigNodeManager.digNodeManager.diggers.Add (this);
	}

	public override bool canDig (GameObject nodeToDig) {
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

	public override void finishedDig() {

	}

	public override void cancelCurrentDigJob() {
		targetDigNode = null;
		setupForNextDig ();
		isDigging = false;
	}

	public override void setupForNextDig() {
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
		yield return new WaitForSeconds (.5f);
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
		}
	}

	public override void onSpawn () {
		DungeonResources.Food+=foodCost;
	}

	public override void onDeath () {
		DungeonResources.Food-=foodCost;
	}

}
