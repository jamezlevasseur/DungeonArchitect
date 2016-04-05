using UnityEngine;
using System.Collections;

public class Portal : Structure {

	ButtonGridManager portalbgm;

	public GameObject capsuleMinion;
	public GameObject cubeMinion;

	// Use this for initialization
	void Start () {
		portalbgm = base.getStructurebgm();
		portalbgm.getCurrentGrid().insertNewCallback(1,summonCapsuleMinion,"Capsule");
		portalbgm.getCurrentGrid().insertNewCallback(2,summonCubeMinion,"Cube");
	}
	
	void summonCapsuleMinion () {
		Instantiate (capsuleMinion, new Vector3(transform.position.x,capsuleMinion.transform.position.y,transform.position.z), Quaternion.identity);
	}

	void summonCubeMinion () {
		Instantiate (cubeMinion, new Vector3(transform.position.x,cubeMinion.transform.position.y,transform.position.z), Quaternion.identity);
	}
		
	void OnMouseDown () {
		GameController.gamecontroller.Foreignbgm = portalbgm;
		wasSelected();
	}

	public override void wasSelected () {
		base.wasSelected();
	}
}
