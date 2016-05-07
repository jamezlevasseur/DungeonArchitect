using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Saveable : MonoBehaviour, SerializableJSON, IComparable {

	public static List<Saveable> all;
	public long id;

	void Awake () {
		if (all==null)
			all = new List<Saveable>();
	}

	// Use this for initialization
	protected virtual void Start () {
		all.Add(this);
		if (id==0)
			id = GameController.getID();
	}

	protected virtual void OnDestroy () {
		all.Remove(this);
	}

	public abstract string getData () ;
	public abstract void syncStats (Hashtable stats);

	public static Saveable getForID (long id) {
		IComparable[] array = all.ToArray();
		ISaveable s = new ISaveable();
		s.id = id;
		return (Saveable) Utils.binSearch((IComparable)s,(IComparable[])array);
	}

	public int CompareTo (object obj) {
		Saveable sav = (Saveable) obj;
		if (sav.id>id) {
			return 1;
		} else if (sav.id<id) {
			return -1;
		} else {
			return 0;
		}
	}
	
}

//just for searching
class ISaveable : Saveable {
	public override string getData ()
	{
		throw new NotImplementedException ();
	}
	public override void syncStats (Hashtable stats)
	{
		throw new NotImplementedException ();
	}
}
