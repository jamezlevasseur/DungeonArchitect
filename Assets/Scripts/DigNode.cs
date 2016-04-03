﻿using UnityEngine;
using System.Collections;

public class DigNode : MonoBehaviour {

	public enum NodeType {normal,gold,undiggable}
	public NodeType type;
	public Material normalMat, goldMat, undiggableMat,selectedNormalMat, selectedGoldMat;
	public int gridX, gridY;
	public int goldWorth;
	private Material selectedMat, unselectedMat;
	private bool isSelected;

	void Start () {
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
		if (type == NodeType.undiggable)
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

	IEnumerator selfDestruct () {
		yield return null;
		DigNodeManager.digNodeManager.digJobs.Remove (this);
		Destroy (gameObject);
	}

	public void dig () {
		if (type == NodeType.undiggable)
			return;
		DigNodeManager.digNodeManager.digJobs.Remove (this);
		Destroy (gameObject);
	}

	public void forceDig () {
		Destroy (gameObject);
	}

}
