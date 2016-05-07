using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Json;

public delegate void voidCallback();

public class GameController : MonoBehaviour {

	public static GameController gamecontroller;

	private long idCount;
	DigNodeManager digNodeManager;
	public const int TOP_MENU_X = 10;
	public const int MOUTH_X_MOD = 50;

	public enum SelectableTypes {Structure, Minion, Room, None};
	public enum DungeonLordType {Warlord, GrandNecromancer};
	public enum GameObjectType {DigNode,GnollPeon,Warlord,Farm,Orc,Adventurer};

	private bool onMainMenu, onInGameMenu, chooseDungeonLord, onLoadingScreen, playingGame, onRaidScreen, showRaidReward, gameOver;
	private int loadingPercent,raidGold,infamy, gameOverTimer;
	private float infamyCheck;
	private DungeonLordType dungeonLordType;
	private GameObject dgRoot;
	private ButtonGridManager bgm, foreignbgm;
	private SelectableTypes lastSelectedType;
	private voidCallback contextualMenuCallback;
	private AstarGrid astargrid;

	public static Vector3 safeSpawnPoint = new Vector3(0,.5f,5);
	public voidCallback ContextualMenuCallback {set{contextualMenuCallback=value;}}
	public SelectableTypes LastSelectedType{get{return lastSelectedType;}set{lastSelectedType=value;}}
	public ButtonGridManager Foreignbgm {get{return foreignbgm;}set{foreignbgm=value;}}
	public GameObject menuCamera, GroundTile, player, placer;

	//minions
	public GameObject WarlordPrefab, GrandNecromancerPrefab, GnollPeonPrefab, OrcPrefab, AdventurerPrefab, MouthPrefab;

	//structures
	public GameObject FarmPrefab, ThronePrefab;

	public GUISkin menuSkin, topMenuSkin, bottomRightMenuSkin, bottomMiddleMenuSkin, bottomMenuBorderSkin, buttonBGSkin, mapSkin;

	public void Awake () {

		if (GameController.gamecontroller == null) {
			gamecontroller = this;
		} else {
			Destroy (gameObject);
		}
		idCount = 0;
		Screen.SetResolution(1280, 800, true);
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
		structureGrid.insertNewCallback (1,placeFarm,"place farm");

		//minion grid
		ButtonGrid minionGrid = new ButtonGrid ();
		minionGrid.insertNewCallback (1,placeGnollPeon,"place gnoll peon");
		minionGrid.insertNewCallback (2,placeOrc,"place orc");
		//minionGrid.insertNewCallback (4,placeAdventurer,"place adventurer");

		//root grid
		ButtonGrid rootGrid = new ButtonGrid ();
		rootGrid.insertNewGridLink (5, minionGrid, "minions", GraphicalAssets.graphicalAssets.minionIcon);
		rootGrid.insertNewGridLink (6, structureGrid, "structures", GraphicalAssets.graphicalAssets.structureIcon);
		bgm = new ButtonGridManager (rootGrid);
		bgm.insertButtonGrid(structureGrid);
		bgm.insertButtonGrid(minionGrid);
		DungeonResources.Gold = 15;
	}

	void Update () {
		if (Time.time-infamyCheck>10) {
			print (infamyCheck);
			infamyCheck = Time.time;
			int chance = UnityEngine.Random.Range(1,11);
			if (infamy>80) {
				if (chance<9) {
					spawnDungeonWave(4);
				}
				infamy-=10;
			} else if (infamy>60) {
				if (chance<7) {
					spawnDungeonWave(4);
				}
				infamy-=5;
			} else if (infamy>40) {
				if (chance<5) {
					spawnDungeonWave(4);
				}
				infamy-=5;
			} else if (infamy>20) {
				if (chance<3) {
					spawnDungeonWave(4);
				}
				infamy-=5;
			}
		}
	}

	void spawnDungeonWave (int amount) {
		for (int i=0;i<amount; i++) {
			Instantiate(AdventurerPrefab,new Vector3(transform.position.x+MOUTH_X_MOD-10,.5f,transform.position.z), Quaternion.identity);
		}
	}

	void placeAdventurer () {
		ObjectSetter.toSet = AdventurerPrefab;
		Instantiate (placer);
	}

	void placeFarm() {
		ObjectSetter.toSet = FarmPrefab;
		Instantiate (placer);
	}

	void placeOrc() {
		if (DungeonResources.FoodLimit>DungeonResources.Food+Orc.foodCost && DungeonResources.Gold>=5) {
			ObjectSetter.toSet = OrcPrefab;
			Instantiate (placer);
			DungeonResources.Gold-=5;
		} else {
			//alert use to issue
		}

	}

	void placeGnollPeon() {
		if (DungeonResources.FoodLimit>DungeonResources.Food+GnollPeon.foodCost && DungeonResources.Gold>=2) {
			ObjectSetter.toSet = GnollPeonPrefab;
			Instantiate (placer);
			DungeonResources.Gold-=2;
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

	public void launchRaidScreen() {
		onRaidScreen = true;
	}

	public static void minionContentualMenu (Minion minion) {
		GUI.Label (new Rect (250, 20, 200, 20), minion.UnitName);
		GUI.Label (new Rect (250, 40, 200, 20), "Level "+minion.Level+" "+minion.ClassName);
		GUI.Label (new Rect (250, 60, 200, 20), "Health: "+minion.HP+"/"+minion.TotalHP);
		GUI.Label (new Rect (250, 80, 200, 20), "Exp: "+minion.EXP+"/100");
		GUI.Label (new Rect (500, 60, 200, 20), "STR: "+minion.STR);
		GUI.Label (new Rect (500, 80, 200, 20), "AGI: "+minion.AGI);
		GUI.Label (new Rect (500, 100, 200, 20), "WIS: "+minion.WIS);
		GUI.Label (new Rect (500, 120, 200, 20), "Armor: "+minion.ARM);
		//GUI.DrawTexture (new Rect (10, 10, 150, 150), minion.UnitPicture);
	}

	IEnumerator initGame () {
		KoolKatDebugger.log ("GO GAME GO");
		loadingPercent = 15;
		digNodeManager.makeNodeMap (transform.position);
		loadingPercent = 35;
		digNodeManager.cutOutSpace (transform.position, 15);
		digNodeManager.cutOutSpace (new Vector3(transform.position.x+MOUTH_X_MOD,.5f,transform.position.z), 15);
		Instantiate (ThronePrefab, new Vector3(transform.position.x,.5f,transform.position.z), Quaternion.identity);
		Instantiate (MouthPrefab, new Vector3(transform.position.x+MOUTH_X_MOD,.5f,transform.position.z), Quaternion.identity);
		Instantiate (player);
		loadingPercent = 50;
		Camera.main.transform.position = new Vector3 (transform.position.x, Camera.main.transform.position.y, transform.position.z-20);
		if (dungeonLordType == DungeonLordType.Warlord) {
			Instantiate(WarlordPrefab,new Vector3(transform.position.x,1,transform.position.z-5),Quaternion.identity);
		}
		loadingPercent = 60;
		StartCoroutine (delayedScan ());

		yield return null;
	}

	void saveGame () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		//game settings
		dict.Add("idCount",idCount);
		//for each obj in game
		string[] all_s = new string[Saveable.all.Count];
		print("all save length: "+Saveable.all.Count);
		for (int i = 0; i < Saveable.all.Count; i++) {
			all_s[i] = Saveable.all[i].getData();
		}
		dict.Add("objects",all_s);
		File.WriteAllText(Application.persistentDataPath+"/dungeonarchitect.sav", Utils.DictionaryToJSON(dict));
	}

	IEnumerator loadGame () {
		Instantiate (player);
		if (File.Exists(Application.persistentDataPath+"/dungeonarchitect.sav")) {
			Hashtable loadJSON = (Hashtable) JSON.JsonDecode(File.ReadAllText(Application.persistentDataPath+"/dungeonarchitect.sav"));
			print (loadJSON);
			ArrayList objects = (ArrayList) loadJSON["objects"];
			Hashtable doLast = new Hashtable();
			foreach (Hashtable obj in objects) {
				string type = (string)obj["type"];
				switch (type) {
				case "DigNode":
					digNodeManager.addLoadedNode(obj);
					break;
				case "Warlord":
					Hashtable statsWarlord = (Hashtable) obj["stats"];
					GameObject warlord = (GameObject)Instantiate(WarlordPrefab,Vector3.zero,Quaternion.identity);
					warlord.GetComponent<Warlord>().syncStats(statsWarlord);
					break;
				case "GnollPeon":
					Hashtable statsGnollPeon = (Hashtable) obj["stats"];
					GameObject gnollPeon = (GameObject)Instantiate(GnollPeonPrefab,Vector3.zero,Quaternion.identity);
					gnollPeon.GetComponent<GnollPeon>().syncStats(statsGnollPeon);
					break;
				case "Farm":
					if (bool.Parse((string)obj["hasFarmer"]) && obj.ContainsKey("clients")) {
						doLast.Add(""+doLast.Count,obj);
						break;
					}
					GameObject farm = (GameObject)Instantiate(FarmPrefab,Vector3.zero,Quaternion.identity);
					farm.GetComponent<Farm>().syncStats(obj);
					break;
				}
			}
			//do some structures last to ensure minions assigned to them are instantiated
			foreach (Hashtable obj in doLast) {
				string type = (string)obj["type"];
				switch (type) {
				case "Farm":
					GameObject farm = (GameObject)Instantiate(FarmPrefab,Vector3.zero,Quaternion.identity);
					farm.GetComponent<Farm>().syncStats(obj);
					break;
				}
			}
			idCount = long.Parse((string)loadJSON["idCount"]);
		}
		digNodeManager.makeTiles();

		Camera.main.transform.position = new Vector3 (transform.position.x, Camera.main.transform.position.y, transform.position.z-20);
		StartCoroutine (delayedScan ());
		yield return null;
	}

	public static long getID () {
		gamecontroller.idCount++;
		return gamecontroller.idCount;
	}
	
	void OnGUI () {
		int sHeight = Screen.height;
		int sWidth = Screen.width;
		if (gameOver) {
			GUI.skin = menuSkin;
			GUI.Box(new Rect(0,0,sWidth,sHeight),"");
			GUI.Label(new Rect(sWidth/2-150,400,450,70),"Game Over");
			if (Time.time-gameOverTimer>4) {
				Application.LoadLevel(0);
			}
		} else if (onMainMenu) {
			GUI.skin = menuSkin;
			GUI.Label(new Rect(sWidth/2-350,20,750,70),"Dungeon Architect");
			if (GUI.Button(new Rect(sWidth/2-250,200,500,70),"Start")) {
				chooseDungeonLord = true;
				onMainMenu = false;
			}
			if (GUI.Button(new Rect(sWidth/2-250,270,500,70),"Load Game")) {
				onMainMenu = false;
				StartCoroutine(loadGame());
			}
			if (GUI.Button(new Rect(sWidth/2-250,340,500,70),"Options")) {

			}
			if (GUI.Button(new Rect(sWidth/2-250,410,500,70),"About")) {

			}
		} else if (onInGameMenu) {
			Time.timeScale = 0;
			GUI.skin = menuSkin;
			GUI.BeginGroup(new Rect(sWidth/2-sWidth/8,sHeight/4,sWidth/4,sHeight/4*2));
			GUI.Box(new Rect(0,0,sWidth/4,sHeight/4*2),"");
			if (GUI.Button(new Rect(sWidth/128,70,sWidth/4,50),"Save Game")) {
				saveGame();
				onInGameMenu = false;
				Time.timeScale = 1;
			}
			if (GUI.Button(new Rect(sWidth/128,140,sWidth/4,50),"Quit")) {
				Application.Quit();
				onInGameMenu = false;
				Time.timeScale = 1;
			}
			if (GUI.Button(new Rect(sWidth/128,210,sWidth/4,50),"Close")) {
				onInGameMenu = false;
				Time.timeScale = 1;
			}
			GUI.EndGroup();

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
		} else if (onRaidScreen) {
			GUI.skin = mapSkin;
			GUI.BeginGroup(new Rect(100,50,1024,700));
			GUI.Box(new Rect(0,0,1024,700),GraphicalAssets.graphicalAssets.raidMap);
			GUI.Label(new Rect(230,430,200,50), "Easy");
			if (GUI.Button(new Rect(230,350,50,50), "")) {
				raidGold = doRaid (Orc.Orcs.Count, 1);
				showRaidReward = true;
				onRaidScreen = false;
			}
			GUI.Label(new Rect(390,80,200,50), " Medium");
			if (GUI.Button(new Rect(400,0,50,50), "")) {
				raidGold = doRaid (Orc.Orcs.Count, 2);
				showRaidReward = true;
				onRaidScreen = false;
			}
			GUI.Label(new Rect(50,370,200,50), "Hard");
			if (GUI.Button(new Rect(50,400,50,50), "")) {
				raidGold = doRaid (Orc.Orcs.Count, 3);
				showRaidReward = true;
				onRaidScreen = false;
			}
			GUI.Label(new Rect(800,400,200,50), "Very Hard");
			if (GUI.Button(new Rect(800,350,50,50), "")) {
				raidGold = doRaid (Orc.Orcs.Count, 4);
				showRaidReward = true;
				onRaidScreen = false;
			}
			if (GUI.Button(new Rect(800,600,100,50), "close")) {
				onRaidScreen = false;
			}
			GUI.skin = menuSkin;
			if (Orc.Orcs==null)
				GUI.Label(new Rect(50,600,900,200),"Raiding Orcs: 0");
			else
				GUI.Label(new Rect(50,600,900,200),"Raiding Orcs: "+Orc.Orcs.Count);
			GUI.EndGroup();
		} else if (showRaidReward) {
			GUI.skin = menuSkin;
			GUI.BeginGroup(new Rect(sWidth/2-sWidth/8,sHeight/4,sWidth/3+50,sHeight/4*2));
			GUI.Box(new Rect(0,0,sWidth/3,sHeight/4*2),"");
			if (raidGold==0) {
				GUI.Label(new Rect(0,50,sWidth/3+50,170),"Raid Failed!");
			} else {
				GUI.Label(new Rect(50,50,sWidth/3+50,170),"Raided "+raidGold+" gold!");
			}
			if(GUI.Button(new Rect(250,300,200,100),"close")) {
				DungeonResources.Gold+=raidGold;
				raidGold = 0;
				showRaidReward=false;
			}
			GUI.EndGroup();
		} else if (playingGame) {
			//TOP MENU
			GUI.skin = topMenuSkin;
			GUI.BeginGroup(new Rect(0,0,sWidth,30));
			GUI.Box(new Rect(0,0,sWidth,30),"");
			if (GUI.Button(new Rect(10,5,90,20),"menu")) {
				onInGameMenu = !onInGameMenu;
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
			bool showToolTip = false;
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
						                        ButtonGrid.BUTTON_SIZE),new GUIContent(icon,text)))
						{
							callback();
						}
						showToolTip = true;
					} else if (grid!=null) {
						if (GUI.Button(new Rect(x*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetX,
						                        y*(ButtonGrid.BUTTON_SIZE+10)+buttonOffsetY,
												ButtonGrid.BUTTON_SIZE,
												ButtonGrid.BUTTON_SIZE),new GUIContent(icon,text)))
						{
							currentbgm.setCurrentGrid(grid);
						}
						showToolTip = true;
					} else if (index==12 && currentbgm.getCurrentGrid().ParentGrid>-1) {
						if (GUI.Button(new Rect(x*(ButtonGrid.BUTTON_SIZE+10)+10,
							y*(ButtonGrid.BUTTON_SIZE+10)+20,
							ButtonGrid.BUTTON_SIZE,
						    ButtonGrid.BUTTON_SIZE),new GUIContent("Cancel",text)))
						{
							currentbgm.pop();
						}
						showToolTip = true;

					}
				}
			}

			if (showToolTip == true) {
				Rect toolRect = new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,120,60);
				GUI.Label(toolRect,GUI.tooltip);
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
			GUI.Label (new Rect (200, 0, 200, 20), "Infamy: " + infamy);
			GUI.Label (new Rect (10, 0, 200, 20), "Gold: " + DungeonResources.Gold);
			GUI.Label (new Rect (100, 0, 150, 20), "Food: " + DungeonResources.Food + " / " + DungeonResources.FoodLimit);
			GUI.EndGroup ();
		} 

	}

	public void runGameOver () {
		gameOver = true;
		gameOverTimer = (int)Time.time;
	}

	int doRaid (int orcs, int targetStrength) {
		print (orcs);
		int chance = 0;
		switch (targetStrength) {
		case 1:
			chance = UnityEngine.Random.Range(0,3);
			infamy+=10;
			if (orcs>=chance) {
				return UnityEngine.Random.Range(5,30);
			}
			break;
		case 2:
			chance = UnityEngine.Random.Range(3,6);
			infamy+=20;
			if (orcs>=chance) {
				return UnityEngine.Random.Range(25,50);
			}
			break;
		case 3:
			chance = UnityEngine.Random.Range(6,9);
			infamy+=30;
			if (orcs>=chance) {
				return UnityEngine.Random.Range(45,60);
			}
			break;
		case 4:
			chance = UnityEngine.Random.Range(9,12);
			infamy+=40;
			if (orcs>=chance) {
				return UnityEngine.Random.Range(55,80);
			}
			break;
		}
		return 0;
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

	public static int FoodLimit {
		get{return foodLimit;}
		set {
			if (value+BASE_FOOD_LIMIT<=MAX_FOOD) {
				foodLimit = value+BASE_FOOD_LIMIT;
			}
		}
	}

	public static void calculateFoodLimit () {
		int total = 0;
		foreach (Farm farm in Farm.farms) {
			total += farm.foodOnFarm;
		}
		FoodLimit = total;
	}
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
