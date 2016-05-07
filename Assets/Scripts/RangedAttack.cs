using UnityEngine;
using System.Collections;

public class RangedAttack : Attack {
	
	private string attackName = "Ranged Attack";
	private int range = 13;
	private int damage = 5;
	private int speed = 3;
	//out of 10, 0 = always miss, 10 = always hit
	private int accuracy = 8;
	private Weapon currentWeapon;
	private Unit self;
	
	public override string Name {get {return attackName;} }
	public override int Range {
		get {
			//return the bottleneck range
			if (CurrentWeapon==null)
				return range;
			else 
				return CurrentWeapon.Range < this.Range ? CurrentWeapon.Range : this.Range;
		} 
	}
	public override int Damage {get {return damage;} }
	public override int Speed {get {return speed;} }
	public override int Accuracy {get {return accuracy;}}
	public override Weapon CurrentWeapon {get {return currentWeapon;} set{currentWeapon = value;}}
	public override Unit Self {get {return self;} set{self = value;}}
	
	public RangedAttack (Weapon _currentWeapon, Unit _self) {
		currentWeapon = _currentWeapon;
		self = _self;
	}
	
	protected override void animate() {
		if (currentWeapon!=null) {
			//animate weapon
		} else {
			//animation without weapon
		}
	}
	
}
