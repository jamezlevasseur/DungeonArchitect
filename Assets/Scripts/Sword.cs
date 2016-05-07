using UnityEngine;
using System.Collections;

public class Sword : Weapon {
	
	private string weaponName = "Sword";
	private int range = 3;
	private int damage = 300;
	private int speed = 4;
	
	public override string Name {get {return weaponName;}}
	public override int Range {get {return range;}}
	public override int Damage {get {return damage;}}
	public override int Speed {get {return speed;}}
	
	public override void animate () {
		
	}
}

