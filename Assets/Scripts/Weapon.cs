using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {

	public abstract string Name {get;}
	public abstract int Range {get;}
	public abstract int Damage {get;}
	public abstract int Speed {get;}

	public abstract void animate ();
	
}