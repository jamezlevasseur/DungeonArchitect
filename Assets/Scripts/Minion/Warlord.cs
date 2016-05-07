using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Warlord : Minion, DungeonLord {

	public override int FoodCost { get {return 0;} set {} }

	public static Warlord warlord;
	
	//private
	
	protected override void Start () {
		base.Start ();
		initStats ();
		if (WeaponObject!=null) {
			myWeapon = WeaponObject.GetComponent<Weapon>();
		}
		base.reachedDestinationCallback = doNothing;
		unitFaction = Faction.Evil;
		defaultAttack = new MeleeAttack(myWeapon, this);
		onSpawn();
	}

	protected override void initStats () {
		str = getStartingStat (UnitStatDescriptor.Strong);
		agi = getStartingStat (UnitStatDescriptor.Strong);
		wis = getStartingStat (UnitStatDescriptor.Strong);
		spd = getStartingStat (UnitStatDescriptor.Strong);
		arm = getStartingStat (UnitStatDescriptor.Strong);
		totalhp = getStartingHP (UnitStatDescriptor.Strong)*4;
		hp = totalhp;
		className = "Warlord";
		unitName = "Warlord #"+Random.Range(1,5001);
		level = Minion.STARTING_LEVEL;
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
		CombatManager.evil.Add(this);
	}
	
	public override void onDeath () {
		GameController.gamecontroller.runGameOver();
	}

	public override void releaseFromStation () {
		
	}
	
	public override void assignToStation (Vector3 placeToGo, Structure station) {
		
	}

	//combat stuff
	
	public override bool attack() {
		print ("WARLORD ATTACK");
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
