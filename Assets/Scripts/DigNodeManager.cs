using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

class DigNodeManager : MonoBehaviour, SerializableJSON
{
	public static DigNodeManager digNodeManager;

	public List<DigNode> toDig;
	public DigNode[,] nodes;
	public Dictionary<DigNode,Worker> digJobs;
	public List<Worker> diggers;
	public GameObject node;
	public Vector3 startPoint;

	public const float NODE_RADIUS = 2.5f;
	public const int MAP_RANGE_X = 300;
	public const int MAP_RANGE_Z = 200;
	public const int NODE_SCALE = 5;

	private bool isScanning,amAssigningJobs;
	private AstarGrid astargrid;
	private int gridRangeX,gridRangeZ;
	private List<DigNode> unreachableJobs;
	GameObject rootNode;
	GameObject tileRoot;

	public DigNodeManager() {
		nodes = new DigNode[MAP_RANGE_X,MAP_RANGE_Z];
		diggers = new List<Worker> ();
		toDig = new List<DigNode> ();
		unreachableJobs = new List<DigNode>();
		digJobs = new Dictionary<DigNode, Worker> ();
	}

	void Awake () {
		if (DigNodeManager.digNodeManager == null)
			digNodeManager = this;
		else
			Destroy (gameObject);
		astargrid = GameObject.Find("A*").GetComponent<AstarGrid>();
		gridRangeX = MAP_RANGE_X/NODE_SCALE;
		gridRangeZ = MAP_RANGE_Z/NODE_SCALE;
		startPoint = transform.position;
		rootNode = new GameObject ("DigNode ROOT");
		tileRoot = new GameObject ("tile ROOT");
	}

	public DigNode.NodeType stringToType (string t) {
		DigNode.NodeType ret = DigNode.NodeType.normal;
		if (String.Equals(t, "gold", StringComparison.Ordinal)) {
			ret = DigNode.NodeType.gold;
		} else if (String.Equals(t, "undiggable", StringComparison.Ordinal)) {
			ret = DigNode.NodeType.undiggable;
		}
		return ret;
	}

	public void addLoadedNode (Hashtable dict) {
		string gridPos = (string)dict["gridPosition"];

		Vector3 position = Utils.vector3FromString((string)dict["position"]);
		GameObject newNodeObject = (GameObject)Instantiate(node, position, Quaternion.identity);
		newNodeObject.transform.parent = rootNode.transform;
		DigNode newNode = newNodeObject.GetComponent<DigNode>();
		newNode.goldWorth = int.Parse((string)dict["gold"]);
		newNode.ID = long.Parse((string)dict["id"]);
		newNode.type = stringToType((string)dict["nodeType"]);
		newNode.gridX = int.Parse(gridPos.Split(',')[0]);
		newNode.gridY = int.Parse(gridPos.Split(',')[1]);
		nodes[newNode.gridX,newNode.gridY] = newNode;

		//place tiles
		GameObject newtile = (GameObject) Instantiate(GameController.gamecontroller.GroundTile, position,Quaternion.identity);//new Vector3(startPoint.x+(k*NODE_SCALE),0,startPoint.z+(i*NODE_SCALE)), Quaternion.identity);
		newtile.transform.parent = tileRoot.transform;
	}

	public void makeTiles () {
		Vector3 worldBottomLeft = transform.position - Vector3.right * MAP_RANGE_X / 2 - Vector3.forward * MAP_RANGE_Z / 2;
		for (int x = 0; x < gridRangeX; x++) {
			for (int z = 0; z < gridRangeZ; z++) {
				Vector3 position = worldBottomLeft + Vector3.right * (x * NODE_SCALE + NODE_RADIUS) + Vector3.forward * (z * NODE_SCALE + NODE_RADIUS);
				//place tiles
				GameObject newtile = (GameObject) Instantiate(GameController.gamecontroller.GroundTile, position,Quaternion.identity);//new Vector3(startPoint.x+(k*NODE_SCALE),0,startPoint.z+(i*NODE_SCALE)), Quaternion.identity);
				newtile.transform.parent = tileRoot.transform;
			}
		}
	}

	//x and z dir will be multiplied by 10 for each direction
	public void makeNodeMap (Vector3 startingPos) {
		startPoint = startingPos;
		Vector3 worldBottomLeft = transform.position - Vector3.right * MAP_RANGE_X / 2 - Vector3.forward * MAP_RANGE_Z / 2;
		for (int x = 0; x < gridRangeX; x++) {
			for (int z = 0; z < gridRangeZ; z++) {

				Vector3 position = worldBottomLeft + Vector3.right * (x * NODE_SCALE + NODE_RADIUS) + Vector3.forward * (z * NODE_SCALE + NODE_RADIUS);
				GameObject newNodeObject = (GameObject)Instantiate(node, position, Quaternion.identity);//new Vector3(startingPos.x+(k*NODE_SCALE),0,startingPos.z+(i*NODE_SCALE)), Quaternion.identity);
				newNodeObject.transform.parent = rootNode.transform;
				DigNode newNode = newNodeObject.GetComponent<DigNode>();
				newNode.gridX = x;
				newNode.gridY = z;
				nodes[x,z] = newNode;

				//place tiles
				GameObject newtile = (GameObject) Instantiate(GameController.gamecontroller.GroundTile, position,Quaternion.identity);//new Vector3(startPoint.x+(k*NODE_SCALE),0,startPoint.z+(i*NODE_SCALE)), Quaternion.identity);
				newtile.transform.parent = tileRoot.transform;
			}
		}
	}

	List<DigNode> getNeighbors (DigNode node) {
		List<DigNode> neighbors = new List<DigNode>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x==0 && y==0)
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				if (checkX>=0 && checkX<gridRangeX && checkY>=0 && checkY<gridRangeZ) {
					if (nodes[checkX,checkY]!=null)
						neighbors.Add(nodes[checkX,checkY]);
				}
			}
		}
		return neighbors;
	}

	public bool isReachable (DigNode node) {
		List<DigNode> neighbors = getNeighbors(node);
		if (neighbors.Count==8)
			return false;
		if ((node.gridX==0 || node.gridX+1 == gridRangeX) && (node.gridY==0 || node.gridY+1 == gridRangeZ)) {
			if (neighbors.Count>=3)
				return false;
		} else if (node.gridX==0 || node.gridX+1 == gridRangeX || node.gridY==0 || node.gridY+1 == gridRangeZ) {
			if (neighbors.Count>=5)
				return false;
		}
		return true;
	}

	//TODO optimize
	public void cutOutSpace (Vector3 center, int r) {
		for (int x = 0; x < gridRangeX; x++) {
			for (int z = 0; z < gridRangeZ; z++) {
				if (Vector3.Distance(nodes[x,z].gameObject.transform.position,center)<r)
					nodes[x,z].forceDig();
			}
		}
	}
	
	public void addToDig (DigNode newToDig) {
		toDig.Add (newToDig);
		if (!amAssigningJobs) {
			amAssigningJobs = true;
			StartCoroutine(assignJobs());
		}
	}

	public void updateGrid () {
		if (!isScanning) {
			isScanning = true;
			StartCoroutine(updateGraph());
		}
	}

	IEnumerator updateGraph () {
		astargrid.CreateGrid();
		isScanning = false;
		yield return null;
		checkUnreachableJobs();
	}

	public void cancelJob (DigNode nodeInJob) {
		print ("CANCEL JOB");
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
			print ("UNREACHABLE");
			unreachableJobs.Add(nodeInJob);
		}
	}

	void checkUnreachableJobs () {
		for(int i=0; i<unreachableJobs.Count; i++) {
			if (isReachable(unreachableJobs[i])) {
				addToDig(unreachableJobs[i]);
				unreachableJobs.Remove(unreachableJobs[i]);
				i--;
			}
		}
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
					if (!isReachable(toDig [0])) {
						print ("NOT REACHABLE");
						unreachableJobs.Add(toDig[0]);
						toDig.RemoveAt (0);
					} else if (diggers [i].canDig (toDig [0].gameObject)) {
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

	public string getData () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		string[] jsonNodes = new string[gridRangeX*gridRangeZ];
		int count = 0;
		for (int x = 0; x < gridRangeX; x++) {
			for (int z = 0; z < gridRangeZ; z++) {
				jsonNodes[count] = nodes[x,z].getData();
				count++;
			}
		}
		dict.Add("type","digNodeManager");
		dict.Add("grid",Utils.ArrayToJSON(jsonNodes));
		return Utils.DictionaryToJSON(dict);
	}

}
