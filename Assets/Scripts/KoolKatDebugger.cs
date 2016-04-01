using UnityEngine;
using System.Collections;

//FOR WHEN U NEED TO DEBUG RIGHT MEOW

public class KoolKatDebugger : MonoBehaviour {

	public static KoolKatDebugger kkd;
	private bool debugRightMeow;

	void Awake () {
		if (KoolKatDebugger.kkd == null)
			kkd = this;
		else
			Destroy (gameObject);
	}

	public static void debug (bool rightMeow) {
		KoolKatDebugger.kkd.debugRightMeow = rightMeow;
	}

	public static void log (object message) {
		if (KoolKatDebugger.kkd.debugRightMeow)
			Debug.Log ("Meow, "+message);
	}

}
