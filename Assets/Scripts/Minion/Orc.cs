using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Orc : Minion {

	public static int foodCost = 4;

	public static List<Orc> Orcs;

	public override int FoodCost { get {return foodCost;} set {foodCost=value;} }

	protected override void Start () {
		base.Start ();
		onSpawn ();
		initStats ();
		if (WeaponObject!=null) {
			myWeapon = WeaponObject.GetComponent<Weapon>();
		}
		base.reachedDestinationCallback = doNothing;
		unitFaction = Faction.Evil;
		defaultAttack = new MeleeAttack(myWeapon, this);
		if (Orcs==null)
			Orcs = new List<Orc>();
		Orcs.Add(this);
	}
	
	protected override void initStats () {
		str = getStartingStat (UnitStatDescriptor.Strong);
		agi = getStartingStat (UnitStatDescriptor.Strong);
		wis = getStartingStat (UnitStatDescriptor.Strong);
		spd = getStartingStat (UnitStatDescriptor.Strong);
		arm = getStartingStat (UnitStatDescriptor.Strong);
		totalhp = getStartingHP (UnitStatDescriptor.Strong);
		hp = totalhp;
		className = "Orc";
		unitName = "Orc #"+Random.Range(1,5001);
		level = Minion.STARTING_LEVEL;
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
	
	public override void onSpawn () {
		DungeonResources.Food+=foodCost;
		CombatManager.evil.Add(this);
	}
	
	public override void onDeath () {
		DungeonResources.Food-=foodCost;
		Orcs.Remove(this);
	}
	
	public override string getData () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("type",GameController.GameObjectType.Orc);
		dict.Add("stats",statsJSON());
		return Utils.DictionaryToJSON(dict);
	}
	
	public override void releaseFromStation () {
		isAssignedToStation = false;
		station.minionLeftStation(this);
		transform.position = GameController.safeSpawnPoint;
	}
	
	public override void assignToStation (Vector3 placeToGo, Structure _station) {
		print ("ASSIGN TO NEW THING");
		transform.position = placeToGo;
		station = _station;
		isAssignedToStation = true;
	}
	
	//combat stuff
	
	public override bool attack() {
		print ("ORC ATTACK");
		if (defaultAttack.attack(engagedEnemy)) {
			Enemy deadEnemy = (Enemy) engagedEnemy;
			exp+=(deadEnemy.expReward*2);
			if(exp>100) {
				int temp = exp-100;
				levelup();
				exp = temp;
			}
			print ("NEW EXP: "+exp);
			return true;
		}
		return false;
	}


}
