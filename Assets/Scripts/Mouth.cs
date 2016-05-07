using UnityEngine;
using System.Collections;

public class Mouth : MonoBehaviour {

	public static bool mouthIsFound = true;

	void OnMouseOver () {
		if (Input.GetMouseButtonDown(0)) {
			print ("CLICK MOUTH");
			if (!mouthIsFound) {
				print("INNER MOUTH");
				checkPathToMouth();
			} else {
				GameController.gamecontroller.launchRaidScreen();
			}
		}
	}

	void checkPathToMouth () {
		PathRequestManager.RequestPath(Vector3.zero,transform.position,OnPathFound);
	}

	public void OnPathFound (Vector3[] newPath, bool pathSuccess) {
		print("MOUTH SUCCESS "+pathSuccess);
		if (pathSuccess) {
			mouthIsFound = true;
			GameController.gamecontroller.launchRaidScreen();
		}
	}


}
