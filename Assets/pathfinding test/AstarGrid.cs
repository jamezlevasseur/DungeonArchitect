using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AstarGrid : MonoBehaviour {

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public float nodeRadius;
	public Transform player;
	Vector2 gridWorldSize;
	AstarNode[,] grid;
	float nodeDiameter;
	int gridSizeX,gridSizeY;

	void Awake () {
		gridWorldSize = new Vector2(DigNodeManager.MAP_RANGE_X,DigNodeManager.MAP_RANGE_Z);
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y/nodeDiameter);
		//CreateGrid ();
	}

	public int MaxSize {
		get {
			return gridSizeX*gridSizeY;
		}
	}

	public void CreateGrid () {
		grid = new AstarNode[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius-.1f,unwalkableMask));
				grid[x,y] = new AstarNode(walkable,worldPoint,x,y);
			}
		}
	}

	public List<AstarNode> getNeighbors (AstarNode node) {
		List<AstarNode> neighbors = new List<AstarNode>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x==0 && y==0)
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				if (checkX>=0 && checkX<gridSizeX && checkY>=0 && checkY<gridSizeY) {
					neighbors.Add(grid[checkX,checkY]);
				}
			}
		}
		return neighbors;
	}

	public AstarNode nodeFromWorldPosition (Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt( (gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt( (gridSizeY - 1) * percentY);
		return grid [x, y];
	}
	
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (grid != null && displayGridGizmos) {
			//AstarNode playerNode = nodeFromWorldPosition(player.position);
			Vector3 cube = Vector3.one * (nodeDiameter-.1f);
			cube.y = 1;
			foreach (AstarNode n  in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				/*if (playerNode == n) {
					Gizmos.color = Color.cyan;
				}*/
				Gizmos.DrawCube(n.worldPosition, cube);
			}
		}
	}












}
