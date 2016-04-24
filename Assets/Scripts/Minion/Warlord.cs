using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Warlord : Minion, DungeonLord {

	public static Warlord warlord;

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
	private bool isAssignedToStation;
	private int foodCost;

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
	
	protected override void Start () {
		base.Start ();
		initStats ();
		setUnitText ();
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
		className = "Warlord";
		minionName = "Warlord #"+Random.Range(1,5001);
		level = Minion.STARTING_LEVEL;
	}

	protected override void setUnitText () {
		GUIText miniontext = (GUIText) gameObject.AddComponent<GUIText> ();
		miniontext.text = minionName+" – Level: "+level;

	}

	public void sitOnThrone() {

	}

	public override string getData () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("stats",statsJSON());
		dict.Add("type",GameController.GameObjectType.Warlord);
		return Utils.DictionaryToJSON(dict);
	}

	public override void onSpawn () {

	}
	
	public override void onDeath () {

	}

	public override void releaseFromStation () {
		
	}
	
	public override void assignToStation (Vector3 placeToGo, Structure station) {
		
	}
}
