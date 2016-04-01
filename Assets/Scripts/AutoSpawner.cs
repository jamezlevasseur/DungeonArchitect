using UnityEngine;
using System.Collections;

public class AutoSpawner : MonoBehaviour {

	float startTime, lastSpawn;
	public GameObject[] thingsToSpawn;
	public Transform transformToSpawn;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (lastSpawn == 0 || Time.time - lastSpawn > 50) {
			foreach (GameObject thingToSpawn in thingsToSpawn) {
				Instantiate (thingToSpawn, new Vector3(transform.position.x,0,transform.position.z), Quaternion.identity);
				lastSpawn = Time.time;
			}
			if (transformToSpawn!=null)
				Instantiate (transformToSpawn, new Vector3(transform.position.x,0,transform.position.z), Quaternion.identity);
		}
		if (Time.time - startTime > 30)
			Destroy (gameObject);
	}
}
