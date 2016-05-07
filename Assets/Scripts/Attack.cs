using UnityEngine;
using System.Collections;

public abstract class Attack {

	public enum Attempt {Hit,Miss,Crit}
	public abstract string Name {get;}
	public abstract int Range {get;}
	public abstract int Damage {get;}
	public abstract int Speed {get;}
	public abstract int Accuracy {get;}
	public abstract Weapon CurrentWeapon {get;set;}
	public abstract Unit Self {get;set;}
	
	protected abstract void animate();

	public bool attack (Unit target) {
		animate();
		bool crit;
		return target.takeHit(rollForAttack(out crit));
	}

	public virtual int rollForAttack(out bool wasCrit) {
		wasCrit = false;
		Attempt attempt = rollForAccuracy();
		switch (attempt) {
		case Attempt.Miss:
			return 0;
		case Attempt.Hit:
			return rollForDamage();
		case Attempt.Crit:
			wasCrit = true;
			return rollForDamage()*3;
		}
		return 0;
	}
	
	protected virtual int rollForDamage() {
		if (CurrentWeapon==null) {
			return Random.Range(1,Damage*(Self.STR/2)+1);
		}
		return Random.Range(1,Damage*(Self.STR/2)+1) + Random.Range(0,CurrentWeapon.Damage+1);
	}
	
	protected virtual Attempt rollForAccuracy() {
		int chance = Random.Range(1,11);
		if (Accuracy>chance) {
			if (chance==Accuracy)
				return Attempt.Crit;
			else
				return Attempt.Hit;
		}
		return Attempt.Miss;
	}

}



