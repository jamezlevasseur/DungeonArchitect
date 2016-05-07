using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farm : Structure {

	const int MAX_CORN = 4;
	const int POINTS_PER_CORN = 5;

	public static List<Farm> farms;

	Vector3[] cornCords = new Vector3[] {new Vector3(.2f,1,.2f), new Vector3(-.2f,1,.2f), new Vector3(.2f,1,-.2f), new Vector3(-.2f,1,-.2f) };
	int corn;

	public int foodOnFarm;
	public ClientHandler<Worker> clientHandler;
	public bool hasFarmer;
	public GameObject Corn;

	void Awake () {
		if (farms==null)
			farms = new List<Farm>();
	}

	protected override void Start () {
		base.Start();
		farms.Add(this);
		clientHandler = new ClientHandler<Worker>(1);
		StartCoroutine("foodTick");
	}

	protected override void OnDestroy () {
		farms.Remove(this);
	}

	IEnumerator foodTick () {
		while (true) {
			yield return new WaitForSeconds(30);
			if (clientHandler.Count==1) {
				if (corn<MAX_CORN)
					corn++;
			} else {
				if (corn>0)
					corn--;
			}
			foodOnFarm = corn * POINTS_PER_CORN;
			DungeonResources.calculateFoodLimit();
			updateCornRender();
		}
	}

	void updateCornRender () {
		for (int i=0; i<transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}
		for (int i=0; i<corn; i++) {
			GameObject acorn = (GameObject)Instantiate(Corn, transform.position - cornCords[i], Quaternion.identity);
			acorn.transform.parent = transform;
			acorn.transform.localPosition = cornCords[i];
		}
	}

	public override string getData () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("id",id);
		dict.Add("type",GameController.GameObjectType.Farm);
		dict.Add("foodOnFarm",foodOnFarm);
		dict.Add("hasFarmer",hasFarmer);
		dict.Add("position",transform.position.ToString());
		if (clientHandler.Count>0) {
			string[] workers = new string[clientHandler.Count];
			for (int i=0; i<workers.Length; i++) {
				Minion m = (Minion)clientHandler.clients[i];
				workers[i] = m.getData();
			}
			dict.Add("clients",workers);
		}
		return Utils.DictionaryToJSON(dict);
	}

	public override void syncStats (Hashtable stats) {
		id = long.Parse((string)stats["id"]);
		transform.position = Utils.vector3FromString((string)stats["position"]);
		hasFarmer = bool.Parse((string)stats["hasFarmer"]);
		foodOnFarm = int.Parse((string)stats["foodOnFarm"]);
		if (hasFarmer && stats.ContainsKey("clients")) {
			Hashtable clients = (Hashtable)stats["clients"];
			foreach (Hashtable c in clients) {
				Hashtable clientStats = (Hashtable)c["stats"];
				Minion m = (Minion) Saveable.getForID(long.Parse((string)clientStats["id"]));
				clientHandler.addClient(m as Worker);
				m.assignToStation(new Vector3(transform.position.x,2,transform.position.z), this);
			}
		}

	}

	public override void minionLeftStation (Minion minion) {
		clientHandler.RemoveClient((Worker)minion);
	}

	void OnMouseOver () {
		if (Input.GetMouseButtonDown(1)) {
			foreach (Minion minion in MinionController.minionController.selectedUnits) {
				if (minion is Worker) {
					if (!clientHandler.hasMaxClients()) {
						clientHandler.addClient(minion as Worker);
					    minion.assignToStation(new Vector3(transform.position.x,2,transform.position.z), this);
					}
				}
			}
		}
	}

}
