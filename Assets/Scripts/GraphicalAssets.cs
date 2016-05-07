using UnityEngine;
using System.Collections;

public class GraphicalAssets : MonoBehaviour {

	public static GraphicalAssets graphicalAssets;
	public Texture defaultIcon;
	public Texture minionIcon;
	public Texture structureIcon;
	public Texture raidMap;
	public Material damagedMaterial;
	public GameObject levelupParticleEffect;

	void Awake () {
		if (GraphicalAssets.graphicalAssets == null)
			GraphicalAssets.graphicalAssets = this;
		else
			Destroy (gameObject);
	}

}
