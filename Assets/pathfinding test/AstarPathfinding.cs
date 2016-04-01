using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class AstarPathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	AstarGrid grid;

	void Awake () {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<AstarGrid> ();
	}

	public void StartFindPath (Vector3 startPosition, Vector3 targetPosition) {

		
		AstarNode targetNode = grid.nodeFromWorldPosition (targetPosition);
		if (!targetNode.walkable) {
			scanRadius = 4;
			//alreadySearched = new List<AstarNode>();
			nearbyNodes = new Heap<AstarNode>(scanRadius*8);
			goalNode = targetNode;
			searchCode++;
			if (searchCode>10)
				searchCode = 0;
			//AstarNode nearbyNode = getClosestWalkableNeighbor(targetNode,targetNode,0,4);
			getClosestWalkableNeighbor_(targetNode, 0);
			if (nearbyNodes.Count==0)
				print ("EMPTY");
			AstarNode nearbyNode = nearbyNodes.RemoveFirst();
			targetNode = nearbyNode==null ? targetNode : nearbyNode;
		}
		Utils.makeMarker(targetNode.worldPosition.x,targetNode.worldPosition.z);
		StartCoroutine(FindPath(startPosition, targetNode.worldPosition));
	}

	AstarNode closestNode = null;
	List<AstarNode> alreadySearched;
	AstarNode goalNode;
	int scanRadius,searchCode;
	Heap<AstarNode> nearbyNodes;
	void getClosestWalkableNeighbor_ (AstarNode node, int scanDepth) {
		if (scanDepth<scanRadius) {
			foreach (AstarNode neighbor in grid.getNeighbors(node)) {
				if (node.searchCode!=searchCode && neighbor.walkable) {
					node.searchCode = searchCode;
					neighbor.gCost = goalNode.gCost + getDistance(goalNode, neighbor);
					neighbor.hCost = getDistance(neighbor,goalNode);
					nearbyNodes.Add(neighbor);
					getClosestWalkableNeighbor_ (neighbor,scanDepth+1);
				} else {
					node.searchCode = searchCode;
					getClosestWalkableNeighbor_ (neighbor,scanDepth+1);
				}
			}
		}
	}

	IEnumerator FindPath (Vector3 startPos, Vector3 targetPos) {
		Stopwatch sw = new Stopwatch();
		sw.Start();

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		AstarNode startNode = grid.nodeFromWorldPosition (startPos);
		AstarNode targetNode = grid.nodeFromWorldPosition (targetPos);

		if (startNode.walkable && targetNode.walkable) {
			Heap<AstarNode> openset = new Heap<AstarNode> (grid.MaxSize);
			HashSet<AstarNode> closedSet = new HashSet<AstarNode> ();
			openset.Add (startNode);

			while (openset.Count > 0) {
				AstarNode currentNode = openset.RemoveFirst();
				closedSet.Add(currentNode);

				if (currentNode == targetNode) {
					sw.Stop();
					print("Path Found: " + sw.ElapsedMilliseconds + "ms");
					pathSuccess = true;
					break;
				}

				foreach (AstarNode neighbor in grid.getNeighbors(currentNode)) {
					if (!neighbor.walkable || closedSet.Contains(neighbor))
						continue;
					int newMovementCostToNeighbor = currentNode.gCost + getDistance(currentNode, neighbor);
					if (newMovementCostToNeighbor < neighbor.gCost || !openset.Contains(neighbor)) {
						neighbor.gCost = newMovementCostToNeighbor;
						neighbor.hCost = getDistance(neighbor,targetNode);
						neighbor.parent = currentNode;

						if (!openset.Contains(neighbor))
							openset.Add(neighbor);
						else
							openset.UpdateItem(neighbor);
					}

				}
			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = retracePath(startNode, targetNode);
		}
		requestManager.FinishProcessingPath(waypoints,pathSuccess);
	}

	Vector3[] retracePath (AstarNode startNode, AstarNode endNode) {
		List<AstarNode> path = new List<AstarNode>();
		AstarNode currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	Vector3[] SimplifyPath (List<AstarNode> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		for (int i=1; i<path.Count; i++) {
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
			if (directionNew!= directionOld) {
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	int getDistance (AstarNode nodeA, AstarNode nodeB) {
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (distX>distY)
			return 14*distY + 10 * (distX-distY);
		return 14*distX + 10 * (distY-distX);
	}
}
