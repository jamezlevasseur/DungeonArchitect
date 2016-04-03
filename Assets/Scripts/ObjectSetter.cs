using UnityEngine;
using System.Collections;

public class ObjectSetter : MonoBehaviour {

	public Transform cursorObject;
	public float distance = 1.5f;
	public static GameObject toSet;
	public static bool isSetting;
	private bool canPlace;
	
	void Start() 
	{
		//Screen.showCursor = false;
		Cursor.visible = false;
		isSetting = true;
	}
	
	void Update() 
	{
		if (Input.GetMouseButton(1)) {
			Cursor.visible = true;
			isSetting = false;
			Destroy(gameObject);
		}
		ObjectFollowCursor();
		if (Input.GetMouseButtonDown (0) && canPlace) {
			Vector3 pos = transform.position;
			pos.y = 1;
			Instantiate(toSet, pos, Quaternion.identity);
			Cursor.visible = true;
			isSetting = false;
			Destroy(gameObject);
		}

	}
	
	void ObjectFollowCursor() 
	{
		RaycastHit hit;
		Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		int mask = 1 << 5 | 1 << 9;

		if (!Physics.Raycast (mouseRay, 100f, mask)) {
			if (!gameObject.GetComponent<MeshRenderer>().enabled)
				gameObject.GetComponent<MeshRenderer>().enabled = true;
			canPlace = true;
		} else {
			if (gameObject.GetComponent<MeshRenderer>().enabled)
				gameObject.GetComponent<MeshRenderer>().enabled = false;
			canPlace = false;
		}

		if (Physics.Raycast (mouseRay, out hit, 100f, 1 << 8)) {

		}

		transform.position = hit.point;
	}
}
