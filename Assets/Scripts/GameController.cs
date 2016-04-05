using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void voidCallback();

public class GameController : MonoBehaviour {

	public static GameController gamecontroller;

	DigNodeManager digNodeManager;
	public const int TOP_MENU_X = 10;

	public enum SelectableTypes {Structure, Minion, Room, None};
	public enum DungeonLordType {Warlord, GrandNecromancer};

	private bool onMainMenu, onInGameMenu, chooseDungeonLord, onLoadingScreen, playingGame;
	private int loadingPercent;
	private DungeonLordType dungeonLordType;
	private GameObject dgRoot;
	private ButtonGridManager bgm, foreignbgm;
	private SelectableTypes lastSelectedType;
	private voidCallback contextualMenuCallback;
	private AstarGrid astargrid;

	public voidCallback ContextualMenuCallback {set{contextualMenuCallback=value;}}
	public SelectableTypes LastSelectedType{get{return lastSelectedType;}set{lastSelectedType=value;}}
	public ButtonGridManager Foreignbgm {get{return foreignbgm;}set{foreignbgm=value;}}
	public GameObject menuCamera, GroundTile, player, spawner, placer;

	//minions
	public GameObject WarlordPrefab, GrandNecromancerPrefab, GnollPeonPrefab;

	//structures
	public GameObject FarmPrefab, ThronePrefab;

	public GUISkin menuSkin, topMenuSkin, bottomRightMenuSkin, bottomMiddleMenuSkin, bottomMenuBorderSkin, buttonBGSkin;

	public void Awake () {
		Screen.SetResolution(1280, 800, true);
		if (GameController.gamecontroller == null)
			gamecontroller = this;
		else
			Destroy (gameObject);
		onMainMenu = true;
		lastSelectedType = SelectableTypes.None;
		digNodeManager = GetComponent<DigNodeManager>();
		astargrid = GameObject.Find("A*").GetComponent<AstarGrid>();
	}
	
	void Start () {
		contextualMenuCallback = defaultContextualMenu;

		//SETUP BUTTON GRID
		//structure grid
		ButtonGrid structureGrid = new ButtonGrid ();
		structureGrid.insertNewCallback (1,placeFarm,"place farm",GraphicalAssets.graphicalAssets.gnollPeonIcon);

		//minion grid
		ButtonGrid minionGrid = new ButtonGrid ();
		structureGrid.insertNewCallback (1,placeGnollPeon,"place gnoll peon",GraphicalAssets.graphicalAssets.gnollPeonIcon);

		//root grid
		ButtonGrid rootGrid = new ButtonGrid ();
		rootGrid.insertNewGridLink (5,minionGrid, "minions", GraphicalAssets.graphicalAssets.minionIcon);
		rootGrid.insertNewGridLink (5,structureGrid, "structures");
		bgm = new ButtonGridManager (rootGrid);
		bgm.insertButtonGrid(structureGrid);

	}

	void placeFarm() {
		ObjectSetter.toSet = spawner;
		Instantiate (placer);
	}

	void placeGnollPeon() {
		ObjectSetter.toSet = GnollPeonPrefab;
		Instantiate (placer);
		if (DungeonResources.FoodLimit<DungeonResources.Food+GnollPeon.foodCost) {
			ObjectSetter.toSet = GnollPeonPrefab;
			Instantiate (placer);
		} else {
			//alert use to issue
		}
	}
	
	IEnumerator delayedScan() {
		yield return new WaitForSeconds (0.5f);
		loadingPercent = 90;
		//AstarData.active.ScanLoop (null);
		astargrid.CreateGrid();
		loadingPercent = 100;
		onLoadingScreen = false;
		playingGame = true;
		Destroy (menuCamera);
		yield return null;
	}

	public void surrenderContextualMenu () {
		contextualMenuCallback = defaultContextualMenu;
	}

	void defaultContextualMenu () {
		GUI.Box (new Rect (0, 0, 10, 10), "");
	}

	public static void minionContentualMenu (Minion minion) {
		GUI.Label (new Rect (250, 20, 200, 20), minion.MinionName);
		GUI.Label (new Rect (250, 40, 200, 20), "Level "+minion.Level+" "+minion.ClassName);
		GUI.Label (new Rect (500, 60, 200, 20), "STR: "+minion.STR);
		GUI.Label (new Rect (500, 80, 200, 20), "AGI: "+minion.AGI);
		GUI.Label (new Rect (500, 100, 200, 20), "WIS: "+minion.WIS);
		GUI.Label (new Rect (500, 120, 200, 20), "Armor: "+minion.ARM);
		//GUI.Label (new Rect (200, 20, 200, 150), "Status: ");
		GUI.DrawTexture (new Rect (10, 10, 150, 150), minion.MinionPicture);
	}

	IEnumerator initGame () {
		KoolKatDebugger.log ("GO GAME GO");
		loadingPercent = 15;
		digNodeManager.makeNodeMap (transform.position);
		loadingPercent = 35;
		digNodeManager.cutOutSpace (transform.position, 15);
		Instantiate (ThronePrefab, new Vector3(transform.position.x,.5f,transform.position.z), Quaternion.identity);
		Instantiate (player);
		loadingPercent = 50;
		Camera.main.transform.position = new Vector3 (transform.position.x, Camera.main.transform.position.y, transform.position.z-20);
		if (dungeonLordType == DungeonLordType.Warlord) {
			Instantiate(WarlordPrefab,new Vector3(transform.position.x,2,transform.position.z-5),Quaternion.identity);
		}
		loadingPercent = 60;
		StartCoroutine (delayedScan ());

		yield return null;
	}
	
	void OnGUI () {
		int sHeight = Screen.height;
		int sWidth = Screen.width;
		if (onMainMenu) {
			GUI.skin = menuSkin;
			GUI.Label(new Rect(sWidth/2-350,20,750,70),"Dungeon Architect");
			if (GUI.Button(new Rect(sWidth/2-250,200,500,70),"Start")) {
				chooseDungeonLord = true;
				onMainMenu = false;
			}
			if (GUI.Button(new Rect(sWidth/2-250,270,500,70),"Load Game")) {

			}
			if (GUI.Button(new Rect(sWidth/2-250,340,500,70),"Options")) {

			}
			if (GUI.Button(new Rect(sWidth/2-250,410,500,70),"About")) {

			}
		} else if (onInGameMenu) {

		} else if (onLoadingScreen) {
			GUI.skin = menuSkin;
			GUI.Label(new Rect(sWidth/2-250,700,750,70),"LOADING "+loadingPercent+"%");
		} else if (chooseDungeonLord) {
			GUI.skin = menuSkin;
			GUI.Label(new Rect(sWidth/8,20,1200,70),"Choose Your Dungeon Lord");
			GUI.skin = bottomMiddleMenuSkin;
			if (GUI.Button(new Rect(sWidth/7,200,400,400),"Warlord")) {
				dungeonLordType = DungeonLordType.Warlord;
				chooseDungeonLord = false;
				onLoadingScreen = true;
				StartCoroutine(initGame());
			}
			if (GUI.Button(new Rect(sWidth/7*4,200,400,400),"Grand Necromancer")) {

			}
		} else if (playingGame) {
			//TOP MENU
			GUI.skin = topMenuSkin;
			GUI.BeginGroup(new Rect(0,0,sWidth,30));
			GUI.Box(new Rect(0,0,sWidth,30),"");
			if (GUI.Button(new Rect(10,5,90,20),"menu")) {
				
			}
			if (GUI.Button(new Rect(110,5,90,20),"quests")) {
				
			}
			GUI.EndGroup();

			//BOTTOM MIDDLE MENU
			
			GUI.skin = bottomMiddleMenuSkin;
			GUI.BeginGroup(new Rect(0,sHeight/5*4,sWidth,sHeight/4));
			GUI.Box(new Rect(240,0,sWidth,400),"");
			contextualMenuCallback();
			GUI.EndGroup();

			//BOTTOM RIGHT MENU
			GUI.skin = bottomRightMenuSkin;
			GUI.BeginGroup(new Rect(sWidth/5*4,sHeight/4*3,sWidth/5,sHeight/4+2));
			GUI.Box(new Rect(0,10,500,400),"");
			ButtonGridManager currentbgm = foreignbgm==null ? bgm : foreignbgm;
			int buttonOffsetX = 15;
			int buttonOffsetY = 20;
			for (int x=0; x<4; x++) {
				for (int y=0; y<3; y++) {
					GUI.skin = buttonBGSkin;
					GUI.Box(new Rect(x*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetX,
					                 y*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetY,
					                 ButtonGrid.BUTTON_SIZE,
					                 ButtonGrid.BUTTON_SIZE),"");
					GUI.skin = bottomRightMenuSkin;
					int index = (x+1)+(y*4);
					string text;
					Texture icon;
					voidCallback callback = currentbgm.getCurrentGrid().getButtonCallback(index, out text, out icon);
					ButtonGrid grid = currentbgm.getCurrentGrid().getGridLink(index, out text, out icon);
					if (callback!=null) {
						if (GUI.Button(new Rect(x*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetX,
						                        y*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetY,
						                        ButtonGrid.BUTTON_SIZE,
						                        ButtonGrid.BUTTON_SIZE),icon))
						{
							callback();
						}
					} else if (grid!=null) {
						if (GUI.Button(new Rect(x*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetX,
						                        y*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetY,
												ButtonGrid.BUTTON_SIZE,
												ButtonGrid.BUTTON_SIZE),icon))
						{
							currentbgm.setCurrentGrid(grid);
						}
					} else if (index==12 && currentbgm.getCurrentGrid().ParentGrid>-1) {
						if (GUI.Button(new Rect(x*(ButtonGrid.BUTTON_SIZE+10)+10,
							y*(ButtonGrid.BUTTON_SIZE+10)+20,
							ButtonGrid.BUTTON_SIZE,
							ButtonGrid.BUTTON_SIZE),"Cancel"))
						{
							currentbgm.pop();
						}
					}
				}
			}
			GUI.EndGroup();

			//bottom border
			GUI.skin = bottomMenuBorderSkin;
			float d = sHeight/5*4-sHeight/4*3;
			GUI.BeginGroup(new Rect(0,sHeight/4*3,sWidth,sHeight/4));
			GUI.Box(new Rect(-10,d,sWidth/5*4,10),"");
			GUI.Box(new Rect(230,d,10,250),"");
			GUI.Box(new Rect(sWidth/5*4-10,0,10,250),"");
			GUI.Box(new Rect(sWidth/5*4-10,0,sWidth,10),"");
			GUI.EndGroup();

			//RESOURCES MENU, TOP RIGHT
			
			GUI.BeginGroup (new Rect(sWidth - sWidth / 4, 0, sWidth / 4, 20));

			GUI.Label (new Rect (10, 0, 50, 20), "Gold: " + DungeonResources.Gold);
			GUI.Label (new Rect (100, 0, 150, 20), "Food: " + DungeonResources.Food + " / "+DungeonResources.FoodLimit);
			GUI.EndGroup ();
		} 

	}

}

public struct DungeonResources {
	public const int MAX_FOOD = 900;
	public const int BASE_FOOD_LIMIT = 30;
	static int gold;
	static int food;
	static int foodLimit = BASE_FOOD_LIMIT;

	public static int Gold {get{return gold;} set{gold = value;}}
	public static int Food {get{return food;}
		set{
			if (value<=foodLimit) {
				food = value;
			}
		}
	}
	public static int FoodLimit {get{return foodLimit;}
		set{
			if (value+BASE_FOOD_LIMIT<=MAX_FOOD)
				foodLimit = value+BASE_FOOD_LIMIT;
		}}
}

public class ButtonGridManager {

	public const int NUM_GRID_LEVELS = 300;

	private ButtonGrid[] grids;
	private int index, currentGrid;

	public ButtonGridManager (ButtonGrid root) {
		grids = new ButtonGrid[NUM_GRID_LEVELS];
		grids [0] = root;
	}

	public int insertButtonGrid (ButtonGrid grid) {
		index++;
		if (index >= NUM_GRID_LEVELS || grids[index]!=null) {
			for (int i=0; i<NUM_GRID_LEVELS; i++) {
				if (grids[i]==null && i!=0) {
					index = i;
					break;
				}
			}
		}
		grids [index] = grid;
		return index;
	}

	public ButtonGrid removeButtonGrid (int pos) {
		if (pos == 0)
			return null;
		ButtonGrid temp = grids [pos];
		grids [pos] = null;
		return temp;
	}

	public void pop ()  {
		if (grids[currentGrid].ParentGrid > -1) {
			currentGrid = grids[currentGrid].ParentGrid;
		}
	}

	public void setCurrentGrid (ButtonGrid grid) {
		if (grid==null) {
			pop();
		}
		for (int i=0; i< NUM_GRID_LEVELS; i++) {
			if (grid==grids[i]) {
				int parent = currentGrid;
				currentGrid = i;
				grids[i].ParentGrid = parent;
			}
		}
	}

	public ButtonGrid getCurrentGrid () {
		return grids [currentGrid];
	}

	public ButtonGrid getGrid (int pos) {
		return grids [pos];
	}

}

public class ButtonGrid {

	public const int BUTTON_SIZE = 50;
	const int w = 4;
	const int h = 3;

	private Vector2[] buttonGridPositions;
	private voidCallback[] callbacks;
	private ButtonGrid[] gridLinks;
	private string[] buttonText;
	private Texture[] buttonIcons;
	private int amountOfButtons;
	private int parentGrid;

	public int ParentGrid {get{return parentGrid;}set{parentGrid=value;}}

	public ButtonGrid () {
		parentGrid = -1;
		buttonGridPositions = new Vector2[w * h];
		callbacks = new voidCallback[w * h];
		gridLinks = new ButtonGrid[w * h];
		buttonText = new string[w * h];
		buttonIcons = new Texture[w * h];
		for (int x=0; x<4; x++) {
			for (int y=0; y<3; y++) {
				buttonGridPositions[x*y] = new Vector2(x*(BUTTON_SIZE+10)+10,y*(BUTTON_SIZE+10)+10);
			}
		}
	}

	public void insertNewCallback (int pos, voidCallback callback, string text) {
		callbacks [pos-1] = callback;
		buttonText [pos-1] = text;
		buttonIcons [pos-1] = GraphicalAssets.graphicalAssets.defaultIcon;
	}

	public void insertNewCallback (int pos, voidCallback callback, string text, Texture icon) {
		callbacks [pos-1] = callback;
		buttonText [pos-1] = text;
		buttonIcons [pos-1] = icon;
	}

	public void removeCallback (int pos) {
		callbacks [pos-1] = null;
		buttonText [pos-1] = null;
		buttonIcons [pos-1] = null;
	}

	public voidCallback getButtonCallback (int pos, out string text, out Texture icon) {
		text = buttonText[pos-1];
		icon = buttonIcons[pos-1];
		return callbacks[pos-1];
	}

	public void insertNewGridLink (int pos, ButtonGrid link, string text) {
		gridLinks [pos-1] = link;
		buttonText [pos-1] = text;
		buttonIcons [pos-1] = GraphicalAssets.graphicalAssets.defaultIcon;
	}

	public void insertNewGridLink (int pos, ButtonGrid link, string text, Texture icon) {
		gridLinks [pos-1] = link;
		buttonText [pos-1] = text;
		buttonIcons [pos-1] = icon;
	}
	
	public void removeGridLink (int pos) {
		gridLinks [pos-1] = null;
		buttonText [pos-1] = null;
		buttonIcons [pos-1] = null;
	}
	
	public ButtonGrid getGridLink (int pos, out string text, out Texture icon) {
		text = buttonText[pos-1];
		icon = buttonIcons[pos-1];
		return gridLinks[pos-1];
	}

}
