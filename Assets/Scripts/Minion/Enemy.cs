using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : Unit {

	public int expReward = 30;
	public float stallingCheck;
	public Vector3 stallLoc = Vector3.zero;

	protected override void Start () {
		base.Start();
		unselectedMat = gameObject.GetComponent<MeshRenderer>().material;
		reachedDestinationCallback = doNothing;
		pathingFailedCallback = doNothing;
		//Start a new path to the targetPosition, return the result to the OnPathComplete function
	}

	protected virtual void Update () {
		if (Time.time-stallingCheck>3) {
			if (path==null)
				path = new Vector3[0];
			if (stallLoc==transform.position && inCombat == false && targetIndex==0 && path.Length==0) {
				PathRequestManager.RequestPath(transform.position, Vector3.zero, OnPathFound);
			}
			stallLoc = transform.position;
			stallingCheck = Time.time;
		}
	}

	protected string statsJSON () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("id",id);
		dict.Add("str",STR);
		dict.Add("agi",AGI);
		dict.Add("wis",WIS);
		dict.Add("spd",SPD);
		dict.Add("hp",HP);
		dict.Add("totalhp",TotalHP);
		dict.Add("arm",ARM);
		dict.Add("level",Level);
		dict.Add("className",ClassName);
		dict.Add("unitnName",UnitName);
		dict.Add("position",transform.position.ToString());
		dict.Add("rotation",transform.localRotation.ToString());
		return Utils.DictionaryToJSON(dict);
	}
	
	public override void syncStats (Hashtable stats) {
		transform.position = Utils.vector3FromString((string)stats["position"]);
		id = long.Parse((string)stats["id"]);
		STR = int.Parse((string)stats["str"]);
		AGI = int.Parse((string)stats["agi"]);
		WIS = int.Parse((string)stats["wis"]);
		SPD = int.Parse((string)stats["spd"]);
		TotalHP = int.Parse((string)stats["totalhp"]);
		ARM = int.Parse((string)stats["arm"]);
		Level = int.Parse((string)stats["level"]);
		ClassName = (string)stats["className"];
		UnitName = (string)stats["unitName"];
	}

	public virtual void wasUnselected () {

	}
	
	public virtual void wasSelected () {

	}

	protected abstract void initStats ();

}
