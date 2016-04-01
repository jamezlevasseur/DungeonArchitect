using UnityEngine;
using System.Collections;

public abstract class Selectable : MonoBehaviour {

	public abstract void wasSelected();
	public abstract void wasUnselected();
}
