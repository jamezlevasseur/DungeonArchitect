using UnityEngine;
using System.Collections;

public class AstarNode: IHeapItem<AstarNode> {

	public bool walkable;
	public Vector3 worldPosition;
	public int gCost, hCost, gridX, gridY, searchCode;
	public AstarNode parent;
	int heapIndex;


	public AstarNode(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY) {
		walkable = _walkable;
		worldPosition = _worldPosition;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo (AstarNode nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare==0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}

}
