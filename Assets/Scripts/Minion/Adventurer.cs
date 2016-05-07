using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Adventurer : Enemy {

	protected override void Start () {
		base.Start ();
		onSpawn ();
		initStats ();
		base.reachedDestinationCallback = doNothing;
	}
	
	protected override void initStats () {
		unitFaction = Faction.Good;
		myWeapon = WeaponObject.GetComponent<Weapon>();
		defaultAttack = new MeleeAttack(myWeapon, this);
		str = getStartingStat (UnitStatDescriptor.Strong);
		agi = getStartingStat (UnitStatDescriptor.Strong);
		wis = getStartingStat (UnitStatDescriptor.Strong);
		spd = getStartingStat (UnitStatDescriptor.Strong);
		arm = getStartingStat (UnitStatDescriptor.Strong);
		totalhp = getStartingHP (UnitStatDescriptor.Strong);
		hp = totalhp;
		className = "Adventurer";
		unitName = "Adventurer #"+Random.Range(1,5001);
		level = Enemy.STARTING_LEVEL;
	}

	public override void onSpawn () {
		CombatManager.good.Add(this);
	}

	public override void onDeath () {
		base.onDeath();
	}

	public override bool attack() {
		print ("ADV ATTACK");
		if (defaultAttack.attack(engagedEnemy)) {
			return true;
		}
		return false;
	}

	public override string getData () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("type",GameController.GameObjectType.Adventurer);
		dict.Add("stats",statsJSON());
		return Utils.DictionaryToJSON(dict);
	}

}
