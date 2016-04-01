using UnityEngine;
using System.Collections;

public class Structure : Selectable {

	protected ButtonGridManager getStructurebgm () {
		ButtonGrid root = new ButtonGrid();
		root.insertNewCallback(9,destroyStructure,"Demolish");
		ButtonGridManager structurebgm = new ButtonGridManager(root);
		return structurebgm;
	}

	protected void destroyStructure () {
		GameController.gamecontroller.Foreignbgm = null;
		Destroy(gameObject);
	}

	public override void wasUnselected () {
		
	}

	public override void wasSelected () {
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Structure;
	}


}
