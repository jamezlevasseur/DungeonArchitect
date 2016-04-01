using UnityEngine;
using System.Collections;

public class MC_controller : MonoBehaviour {

	float speed = 5.0f;
	float scrollSpeed = 35.0f;
	const int LOWEST_HEIGHT = 30;
	const int HIGHEST_HEIGHT = 50;

	// Update is called once per frame
	void Update () {
		float translationV = Input.GetAxis("Vertical") * speed;
		float translationH = Input.GetAxis("Horizontal") * speed;
		float height = Input.GetAxis ("Mouse ScrollWheel") * scrollSpeed;
		if (transform.position.y < LOWEST_HEIGHT && height>0) {
			height = 0;
		} else if (transform.position.y-height<=LOWEST_HEIGHT) {
			height = transform.position.y-LOWEST_HEIGHT;
		} else if (transform.position.y > HIGHEST_HEIGHT) {
			//height = HIGHEST_HEIGHT;
			height = transform.position.y-HIGHEST_HEIGHT;
		}
		if (height > 0) {
			KoolKatDebugger.log ("change: " + height);
			KoolKatDebugger.log ("height: " + transform.position.y);
		}
		transform.Translate(translationH,translationV,height);
		if (Input.GetKeyDown (KeyCode.Q)) {
			transform.Rotate(new Vector3(1,90,0));
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			transform.Rotate(new Vector3(0,-90,0));
		}
	}
}
