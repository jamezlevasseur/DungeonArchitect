using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StructureController : MonoBehaviour {

	public static StructureController structureController;

	public Transform target;

	private List<Structure> allUnits;
	private List<Structure> unselectedUnits;
	private List<Structure> selectedUnits;


	void Awake () {
		if (structureController == null) {
			structureController = this;
		} else {
			Destroy(gameObject);
		}
	}

	public void Start () {
		if (target == null)
			target = GameObject.Find ("Target").transform;
		allUnits = new List<Structure> ();
		selectedUnits = new List<Structure> ();
		unselectedUnits = new List<Structure> ();
	}

	public void OnGUI () {
		if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
			deselectUnits();
		}
	}

	void deselectUnits () {
		for (int i=0; i<selectedUnits.Count; i++) {
			selectedUnits [i].wasUnselected ();
		}
		selectedUnits.Clear ();
		if (GameController.gamecontroller.LastSelectedType==GameController.SelectableTypes.Structure) {
			GameController.gamecontroller.Foreignbgm = null;
		}
	}

	public void addSelectedUnit (Structure unit) {
		selectedUnits.Add (unit);
	}

	public void addUnselectedUnit (Structure unit) {
		unselectedUnits.Add (unit);
	}

	public void addNewUnit (Structure unit) {
		allUnits.Add (unit);
	}
		
}
