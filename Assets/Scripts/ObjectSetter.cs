using UnityEngine;
using System.Collections;

public class ObjectSetter : MonoBehaviour {

	public Transform cursorObject;
	public float distance = 1.5f;
	public static GameObject toSet;
	public static bool isSetting;
	private bool canPlace, colliding;

	public Material goodMat,badMat;
	
	void Start() 
	{
		//Screen.showCursor = false;
		Cursor.visible = false;
		isSetting = true;

		gameObject.GetComponent<MeshFilter>().mesh = toSet.GetComponent<MeshFilter>().sharedMesh;
		gameObject.transform.localScale = toSet.transform.localScale;
		//gameObject.GetComponent<MeshCollider>().sharedMesh = toSet.GetComponent<MeshFilter>().sharedMesh;
		if (toSet.GetComponent<MeshCollider>()!=null) {
			MeshCollider mc = gameObject.AddComponent<MeshCollider>();
			mc = toSet.GetComponent<MeshCollider>();
			GetComponent<MeshCollider>().convex = true;
			GetComponent<MeshCollider>().isTrigger = true;
		} else if (toSet.GetComponent<BoxCollider>()!=null) {
			BoxCollider bc = gameObject.AddComponent<BoxCollider>();
			bc = toSet.GetComponent<BoxCollider>();
			GetComponent<BoxCollider>().isTrigger = true;
		} else if (toSet.GetComponent<CapsuleCollider>()!=null) {
			CapsuleCollider cc = gameObject.AddComponent<CapsuleCollider>();
			cc = toSet.GetComponent<CapsuleCollider>();
			GetComponent<CapsuleCollider>().isTrigger = true;
		} else if (toSet.GetComponent<SphereCollider>()!=null) {
			SphereCollider sc = gameObject.AddComponent<SphereCollider>();
			sc = toSet.GetComponent<SphereCollider>();
			GetComponent<SphereCollider>().isTrigger = true;
		}
	}

	void OnTriggerStay (Collider other) {
		colliding = true;
		GetComponent<MeshRenderer>().material = badMat;
	}

	void OnTriggerExit () {
		colliding = false;
		GetComponent<MeshRenderer>().material = goodMat;
	}
	
	void Update() 
	{
		if (Input.GetMouseButton(1)) {
			Cursor.visible = true;
			isSetting = false;
			Destroy(gameObject);
		}
		ObjectFollowCursor();
		if (Input.GetMouseButtonDown (0) && canPlace && !colliding) {
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
			/*if (gameObject.GetComponent<MeshRenderer>().enabled)
				gameObject.GetComponent<MeshRenderer>().enabled = false;
			canPlace = false;*/
		}

		if (Physics.Raycast (mouseRay, out hit, 100f, 1 << 8)) {

		}
		transform.position = new Vector3(hit.point.x,1.1f,hit.point.z);
	}
}
