using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Minion : Unit, Selectable {

	protected Structure station;
	protected bool isAssignedToStation;

	public virtual int FoodCost { get; set; }
	public virtual bool IsAssignedToStation { get{return isAssignedToStation;} set {isAssignedToStation=value;} }

	protected override void Start () {
		base.Start();
		unselectedMat = gameObject.GetComponent<MeshRenderer>().material;
		MinionController.minionController.addUnselectedUnit (this);
		MinionController.minionController.addNewUnit (this);
		reachedDestinationCallback = doNothing;
		pathingFailedCallback = doNothing;
		//Start a new path to the targetPosition, return the result to the OnPathComplete function
	}

	public override void goTo (Vector3 targetPosition) {
		try {
			if (!IsAssignedToStation)
				PathRequestManager.RequestPath (transform.position,targetPosition, pathFound);
		} catch (MissingReferenceException e) {
			print (e.Message);
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
		dict.Add("exp",EXP);
		dict.Add("className",ClassName);
		dict.Add("minionName",UnitName);
		dict.Add("foodCost",FoodCost);
		dict.Add("isAssignedToStation",IsAssignedToStation);
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
		EXP = int.Parse((string)stats["exp"]);
		Level = int.Parse((string)stats["level"]);
		FoodCost = int.Parse((string)stats["foodCost"]);
		ClassName = (string)stats["className"];
		unitName = (string)stats["minionName"];
		IsAssignedToStation = bool.Parse((string)stats["isAssignedToStation"]);
	}

	protected void destroyMinion () {
		GameController.gamecontroller.Foreignbgm = null;
		Destroy(gameObject);
	}

	protected ButtonGridManager getMinionbgm () {
		ButtonGrid root = new ButtonGrid();
		root.insertNewCallback(8,destroyMinion,"Kill");
		ButtonGridManager structurebgm = new ButtonGridManager(root);
		return structurebgm;
	}

	public virtual void wasUnselected () {
		gameObject.GetComponent<MeshRenderer> ().material = unselectedMat;
		GameController.gamecontroller.surrenderContextualMenu ();
	}

	public virtual void assertbgm () {
		GameController.gamecontroller.Foreignbgm = getMinionbgm();
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Minion;
	}

	public virtual void wasSelected () {
		MinionController.minionController.deselectUnits ();
		MinionController.minionController.addSelectedUnit (this);
		gameObject.GetComponent<MeshRenderer> ().material = selectedMat;
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Minion;
		GameController.gamecontroller.ContextualMenuCallback = minionContextualMenuCallback;
		assertbgm();
	}

	void minionContextualMenuCallback () {
		GameController.minionContentualMenu (this);
	}

	protected void OnMouseDown () {
		wasSelected ();
	}

	protected void levelup () {
		level+=1;
		exp = 0;
		totalhp+=100;
		hp = totalhp;
		increaseRandomStat();
		StartCoroutine("levelUpParticleEffect");
	}

	protected void increaseRandomStat () {
		//make random later
		if (str<10)
			str+=1;
	}

	IEnumerator levelUpParticleEffect () {
		GameObject pe = (GameObject) Instantiate(GraphicalAssets.graphicalAssets.levelupParticleEffect,Vector3.zero,Quaternion.identity);
		pe.transform.parent = transform;
		pe.transform.localPosition = Vector3.zero;
		yield return new WaitForSeconds(3.0f);
		Destroy(pe);
	}

	public override void onDeath ()
	{
		base.onDeath ();
		MinionController.minionController.selectedUnits.Remove(this);
		MinionController.minionController.allUnits.Remove(this);
		MinionController.minionController.unselectedUnits.Remove(this);
	}

	public abstract void releaseFromStation ();
	public abstract void assignToStation (Vector3 placeToGo, Structure station);
	protected abstract void initStats ();

}
