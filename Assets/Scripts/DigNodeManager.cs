using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

class DigNodeManager : MonoBehaviour
{
	public static DigNodeManager digNodeManager;
	public List<DigNode> nodes,toDig;
	public Dictionary<DigNode,Worker> digJobs;
	public List<Worker> diggers;
	public GameObject node;
	public Vector3 startPoint, middlePoint;

	private bool isScanning;
	private AstarGrid astargrid;

	public const int NODE_SCALE = 5;
	public const float NODE_RADIUS = 2.5f;
	public const int MAP_RANGE_X = 200;
	public const int MAP_RANGE_Z = 100;


	private bool amAssigningJobs;

	public DigNodeManager() {
		nodes = new List<DigNode> ();
		diggers = new List<Worker> ();
		toDig = new List<DigNode> ();
		digJobs = new Dictionary<DigNode, Worker> ();
	}

	void Awake () {
		if (DigNodeManager.digNodeManager == null)
			digNodeManager = this;
		else
			Destroy (gameObject);
		astargrid = GameObject.Find("A*").GetComponent<AstarGrid>();
	}

	//x and z dir will be multiplied by 10 for each direction
	public void makeNodeMap (Vector3 startingPos) {
		int xLimit = MAP_RANGE_X/NODE_SCALE;
		int zLimit = MAP_RANGE_Z/NODE_SCALE;
		startPoint = startingPos;
		GameObject rootNode = new GameObject ("DigNode ROOT");
		GameObject tileRoot = new GameObject ("tile ROOT");
		Vector3 worldBottomLeft = transform.position - Vector3.right * MAP_RANGE_X / 2 - Vector3.forward * MAP_RANGE_Z / 2;
		for (int x = 0; x < xLimit; x++) {
			for (int z = 0; z < zLimit; z++) {
				Vector3 position = worldBottomLeft + Vector3.right * (x * NODE_SCALE + NODE_RADIUS) + Vector3.forward * (z * NODE_SCALE + NODE_RADIUS);
				GameObject newNode = (GameObject)Instantiate(node, position, Quaternion.identity);//new Vector3(startingPos.x+(k*NODE_SCALE),0,startingPos.z+(i*NODE_SCALE)), Quaternion.identity);
				newNode.transform.parent = rootNode.transform;
				nodes.Add(newNode.GetComponent<DigNode>());

				//place tiles
				GameObject newtile = (GameObject) Instantiate(GameController.gamecontroller.GroundTile, position,Quaternion.identity);//new Vector3(startPoint.x+(k*NODE_SCALE),0,startPoint.z+(i*NODE_SCALE)), Quaternion.identity);
				newtile.transform.parent = tileRoot.transform;
			}
		}
	}


	public Vector3 cutOutSpace (Vector3 center, int r) {
		for (int i = 0; i < nodes.Count; i++) {
			if (Vector3.Distance(nodes[i].gameObject.transform.position,center)<=r) {
				nodes[i].forceDig();
			}
		}
		return center;
	}

	public void addToDig (DigNode newToDig) {
		toDig.Add (newToDig);
		if (!amAssigningJobs) {
			KoolKatDebugger.log ("ADD NEW TO DIG");
			amAssigningJobs = true;
			StartCoroutine(assignJobs());
		}
	}

	public void updateGraphAfterDig () {
		if (!isScanning) {
			KoolKatDebugger.log("UPDATE AFTER DIG");
			isScanning = true;
			StartCoroutine(updateGraph());
		}
	}

	public void cancelJob (DigNode nodeInJob) {
		Debug.Log ("CANCEL JOB");
		if (toDig.Contains(nodeInJob)) {
			toDig.Remove(nodeInJob);
		}
		if (digJobs.ContainsKey (nodeInJob)) {
			digJobs [nodeInJob].cancelCurrentDigJob ();
			digJobs.Remove (nodeInJob);
		}
	}

	public void failedJob (DigNode nodeInJob) {
		if (digJobs.ContainsKey (nodeInJob)) {
			digJobs [nodeInJob].cancelCurrentDigJob ();
			digJobs.Remove (nodeInJob);
			addToDig(nodeInJob);
		}
	}

	IEnumerator updateGraph () {
		yield return new WaitForSeconds (1);
		astargrid.CreateGrid();
		isScanning = false;
		yield return null;
	}

	IEnumerator assignJobs () {
		bool keepLooping = true;
		while (keepLooping) {
			KoolKatDebugger.log ("BEGIN NEW TO DIG, DIGGERS: "+diggers.Count);

			for (int i=0; i<diggers.Count; i++) {
				if (toDig.Count==0) {
					amAssigningJobs = false;
					keepLooping = false;
					break;
				}
				try {
				if (diggers [i].canDig (toDig [0].gameObject)) {
						digJobs.Add(toDig[0], diggers[i]);
						toDig.RemoveAt (0);
					}
				} catch (System.ArgumentOutOfRangeException e) {
					Debug.Log( e.Message );
					keepLooping = false;
					break;
				}
			}
			yield return new WaitForSeconds(2);
		}
	}

}
