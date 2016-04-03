using UnityEngine;
using System.Collections;

public class Warlord : DungeonLord {
	
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
	public override int FoodCost { get {return 0;} }
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

	public override void sitOnThrone() {

	}

	public override void onSpawn () {

	}
	
	public override void onDeath () {

	}
}
