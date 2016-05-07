using UnityEngine;
using System.Collections;

public class MeleeAttack : Attack {
	
	private string attackName = "Melee Attack";
	private int range = 7;
	private int damage = 100;
	private int speed = 3;
	//out of 10, 0 = always miss, 10 = always hit
	private int accuracy = 8;
	private int animStage = 0;
	private int animMod = 1;
	private Weapon currentWeapon;
	private Unit self;
	
	public override string Name {get {return attackName;} }
	public override int Range {
		get {
			//return the bottleneck range

			if (CurrentWeapon==null) {
				return range;
			}
			return CurrentWeapon.Range < range ? CurrentWeapon.Range : range;

		} 
	}
	public override int Damage {get {return damage;} }
	public override int Speed {get {return speed;} }
	public override int Accuracy {get {return accuracy;}}
	public override Weapon CurrentWeapon {get {return currentWeapon;} set{currentWeapon = value;}}
	public override Unit Self {get {return self;} set{self = value;}}
	
	public MeleeAttack (Weapon _currentWeapon, Unit _self) {
		currentWeapon = _currentWeapon;
		self = _self;
	}
	
	protected override void animate() {

		if (currentWeapon!=null) {
			Debug.Log("ROTATE");
			switch(animStage) {
			case 0:
				animMod = 1;
				animStage+=animMod;
				currentWeapon.gameObject.transform.Rotate (new Vector3(0,0,30));
				break;
			case 1:
				animStage+=animMod;
				currentWeapon.gameObject.transform.Rotate (new Vector3(0,0,30));
				break;
			case 2:
				animStage+=animMod;
				currentWeapon.gameObject.transform.Rotate (new Vector3(0,0,-30));
				break;
			case 3:
				animMod = -1;
				animStage+=animMod;
				currentWeapon.gameObject.transform.Rotate (new Vector3(0,0,-30));
				break;
			}
		} else {
			//animation without weapon
		}
	}
	
}