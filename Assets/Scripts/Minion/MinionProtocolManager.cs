
/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionProtocolManager : MonoBehaviour {

	public const string WORSHIPPER_PROTOCOL = "Worshipper";
	public const string CRAFTER_PROTOCOL = "Crafter";
	public const string WARDEN_PROTOCOL = "Warden";
	public const string PRISONER_PROTOCOL = "Prisoner";
	public const string DUNGEON_LORD_PROTOCOL = "DungeonLord";
	public const string SCHOLAR_PROTOCOL = "Scholar";
	public const string LONER_PROTOCOL = "Loner";
	public const string WORKER_PROTOCOL = "Worker";
	
	public static MinionProtocolManager minionProtocolManager;
	public string[] minionProtocol =  new string[]{WORSHIPPER_PROTOCOL, CRAFTER_PROTOCOL, WARDEN_PROTOCOL, PRISONER_PROTOCOL,
		DUNGEON_LORD_PROTOCOL, SCHOLAR_PROTOCOL, LONER_PROTOCOL, WORKER_PROTOCOL};
	public Dictionary<string, string[]> protocolList;

	void Awake() {
		if (minionProtocolManager == null)
			minionProtocolManager = this;
		else
			Destroy (gameObject);
	}

	void Start() {
		//setup methods
		protocolList = new Dictionary<string, string[]>();
		protocolList.Add(WORSHIPPER_PROTOCOL, new string[]{});
		protocolList.Add(CRAFTER_PROTOCOL, new string[]{});
		protocolList.Add(WARDEN_PROTOCOL, new string[]{});
		protocolList.Add(PRISONER_PROTOCOL, new string[]{});
		protocolList.Add(DUNGEON_LORD_PROTOCOL, new string[]{"sitOnThrone"});
		protocolList.Add(SCHOLAR_PROTOCOL, new string[]{});
		protocolList.Add(LONER_PROTOCOL, new string[]{});
		protocolList.Add(WORKER_PROTOCOL, new string[]{"volunteerToDig","canDig"});
	}
	
	public void protocolValidator (Object validate, string[] protocols) {
		for (int i = 0; i < protocols.Length; i++) {
			string[] methods = protocolList[protocols[i]];
			for (int k = 0; k < methods.Length; k++) {
				if (!HasMethod(validate,methods[k]))
					throw new ProtocolNotFoundException(validate+" does not implement the "+protocols[i]+" protocols.");
			}
		}
	}

	public static bool HasMethod(object objectToCheck, string methodName)
	{
		var type = objectToCheck.GetType();
		return type.GetMethod(methodName) != null;
	}

	//Worshipper methods

	//Crafter methods

	//Warden methods

	//Prisoner methods

	//DungeonLord methods

	//public abstract void sitOnThrone();

	//Scholar methods

	//Loner methods

}
*/