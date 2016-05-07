using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

	const int DETECTION_RANGE = 30;
	public static CombatManager instance;
	public static List<Unit> fighters;
	public static List<Unit> good;
	public static List<Unit> neutral;
	public static List<Unit> evil;

	void Awake () {
		if (instance==null) {
			instance = this;
			fighters = new List<Unit>();
			good = new List<Unit>();
			neutral = new List<Unit>();
			evil = new List<Unit>();
		} else {
			Destroy(gameObject);
		}
	}

	float lastUpdate;

	void FixedUpdate () {
		if (Time.time-lastUpdate>1) {
			lastUpdate = Time.time;
			for (int g=0; g<good.Count; g++) {
				if (good[g]!=null) {
					for (int e=0; e<evil.Count; e++) {
						if (evil[e]!=null) {
							if (Vector3.Distance(good[g].gameObject.transform.position,evil[e].gameObject.transform.position)<DETECTION_RANGE) {
								if (good[g].InCombat==false) {
									good[g].engageEnemy(evil[e]);
								}
								if (evil[e].InCombat==false) {
									evil[e].engageEnemy(good[g]);
								}
							}
						}
					}
				}
			}
		}
	}

}
