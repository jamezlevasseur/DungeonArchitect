using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionController : MonoBehaviour {
	
	/** Mask for the raycast placement */
	public LayerMask groundMask;
	public LayerMask minionMask;
	public static MinionController minionController;

	bool isSelecting = false;
	Vector3 mousePosition1;
	
	public Transform target;

	public List<Minion> allUnits;
	public List<Minion> unselectedUnits;
	public List<Minion> selectedUnits;
	
	/** Determines if the target position should be updated every frame or only on double-click */
	private bool onlyOnDoubleClick;
	
	Camera cam;

	void Awake () {
		if (minionController == null) {
			minionController = this;
		} else {
			Destroy(gameObject);
		}
		onlyOnDoubleClick = true;
	}

	public void Start () {
		//Cache the Main Camera
		cam = Camera.main;

		if (target == null)
			target = GameObject.Find ("Target").transform;
		allUnits = new List<Minion> ();
		selectedUnits = new List<Minion> ();
		unselectedUnits = new List<Minion> ();
	}
	
	public void OnGUI () {
		if (onlyOnDoubleClick && cam != null && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2) {
			UpdateTargetPosition ();
		} else if (onlyOnDoubleClick && cam != null && Event.current.type == EventType.MouseDown && Event.current.button == 1 && !ObjectSetter.isSetting) {
			Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (!Physics.Raycast (mouseRay, 100f, 1<<10)) {
				deselectUnits();
			}
		}
		if( isSelecting )
		{
			// Create a rect from both mouse positions
			Rect rect = Utils.GetScreenRect( mousePosition1, Input.mousePosition );
			Utils.DrawScreenRect( rect, new Color( 0.8f, 0.8f, 0.95f, 0.25f ) );
			Utils.DrawScreenRectBorder( rect, 2, new Color( 0.8f, 0.8f, 0.95f ) );
		}
	}

	// Update is called once per frame
	void Update () {
		if (!onlyOnDoubleClick && cam != null) {
			UpdateTargetPosition ();
		}
		// If we press the left mouse button, save mouse location and begin selection
		if( Input.GetMouseButtonDown( 0 ) )
		{
			isSelecting = true;
			mousePosition1 = Input.mousePosition;
		}
		// If we let go of the left mouse button, end selection
		if (Input.GetMouseButtonUp (0)) {
			for (int i=0; i<unselectedUnits.Count; i++) {
				try {
					if (IsWithinSelectionBounds(unselectedUnits[i].gameObject)) {
						selectedUnits.Add(unselectedUnits[i]);
						unselectedUnits[i].wasSelected();
						unselectedUnits.RemoveAt(i);
						i--;
					}
				} catch (MissingReferenceException e) {
					print (e.Message);
				}
			}
			isSelecting = false;
		}
	}

	public void deselectUnits () {
		for (int i=0; i<selectedUnits.Count; i++) {
			selectedUnits [i].wasUnselected ();
			unselectedUnits.Add(selectedUnits[i]);
		}
		selectedUnits.Clear ();
		if (GameController.gamecontroller.LastSelectedType==GameController.SelectableTypes.Minion) {
			GameController.gamecontroller.Foreignbgm = null;
		}
	}

	public void addSelectedUnit (Minion unit) {
		unselectedUnits.Remove (unit);
		selectedUnits.Add (unit);
	}

	public void addUnselectedUnit (Minion unit) {
		selectedUnits.Remove (unit);
		unselectedUnits.Add (unit);
	}

	public void addNewUnit (Minion unit) {
		allUnits.Add (unit);
	}

	public void UpdateTargetPosition () {
		//Fire a ray through the scene at the mouse position and place the target where it hits
		RaycastHit hit;
		if (Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, groundMask) && hit.point != target.position) {
			target.position = hit.point;
			for (int i=0; i<selectedUnits.Count; i++) {
				selectedUnits[i].goTo(target.position);
				print ("SENDING GO TO");
			}
		}
	}

	public bool IsWithinSelectionBounds( GameObject gameObject )
	{
		if( !isSelecting )
			return false;
		
		Camera camera = Camera.main;
		Bounds viewportBounds =
			Utils.GetViewportBounds( camera, mousePosition1, Input.mousePosition );

		return viewportBounds.Contains(
			camera.WorldToViewportPoint( gameObject.transform.position ) );
	}
	
}
