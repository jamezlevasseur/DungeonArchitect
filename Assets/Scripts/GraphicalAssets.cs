using UnityEngine;
using System.Collections;

public class GraphicalAssets : MonoBehaviour {

	public static GraphicalAssets graphicalAssets;
	public Texture defaultIcon;
	public Texture minionIcon;
	public Texture portalIcon;
	public Texture cubeMinionIcon;
	public Texture capsuleMinionIcon;
	public Texture gnollPeonIcon;

	void Awake () {
		if (GraphicalAssets.graphicalAssets == null)
			GraphicalAssets.graphicalAssets = this;
		else
			Destroy (gameObject);
	}

}
