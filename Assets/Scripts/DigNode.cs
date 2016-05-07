using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DigNode : Saveable, SerializableJSON, GameID {

	public enum NodeType {normal,gold,undiggable}
	public NodeType type;
	public Material normalMat, goldMat, undiggableMat,selectedNormalMat, selectedGoldMat;
	public int gridX, gridY;
	public int goldWorth;
	private Material selectedMat, unselectedMat;
	private bool isSelected;
	public long ID {get{return id;}set{id=value;}}

	protected override void Start () {
		base.Start();
		if (id==0)
			id = GameController.getID();
		int nodeType = Random.Range (1, 101);
		if (nodeType <= 10) {
			type = NodeType.undiggable;
			gameObject.GetComponent<MeshRenderer>().material = undiggableMat;
		} else if (nodeType <= 40) {
			type = NodeType.gold;
			gameObject.GetComponent<MeshRenderer>().material = goldMat;
			selectedMat = selectedGoldMat;
			unselectedMat = goldMat;
			goldWorth = Random.Range (1,21);
		} else {
			type = NodeType.normal;
			gameObject.GetComponent<MeshRenderer>().material = normalMat;
			selectedMat = selectedNormalMat;
			unselectedMat = normalMat;
		}
	}

	void OnMouseDown () {
		if (type == NodeType.undiggable && ObjectSetter.isSetting)
			return;
		isSelected = !isSelected;
		if (isSelected) {
			DigNodeManager.digNodeManager.addToDig(this);
			gameObject.GetComponent<MeshRenderer>().material = selectedMat;
		} else {
			DigNodeManager.digNodeManager.cancelJob(this);
			gameObject.GetComponent<MeshRenderer>().material = unselectedMat;
		}
	}
	
	public void dig () {
		if (type == NodeType.undiggable)
			return;
		DigNodeManager.digNodeManager.digJobs.Remove (this);
		Saveable.all.Remove(this);
		Destroy (gameObject);
	}

	public void forceDig () {
		Destroy (gameObject);
	}

	public override string getData () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("type",GameController.GameObjectType.DigNode);
		dict.Add("nodeType",type);
		dict.Add("position",transform.position.ToString());
		dict.Add("gridPosition",gridX+","+gridY);
		dict.Add("gold",goldWorth);
		dict.Add("id",id);
		return Utils.DictionaryToJSON(dict);
	}

	public override void syncStats (Hashtable stats)
	{
		//dig node manager handles this
		throw new System.NotImplementedException ();
	}

}
