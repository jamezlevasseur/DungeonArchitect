using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Structure : Saveable, Selectable, SerializableJSON, GameID {

	protected long id;
	public long ID {get{return id;}}

	protected override void Start () {
		base.Start();
	}

	protected string getStats () {
		Dictionary<object,object> dict = new Dictionary<object, object>();
		dict.Add("id",id);
		dict.Add("position",transform.position.ToString());
		return Utils.DictionaryToJSON(dict);
	}

	protected ButtonGridManager getStructurebgm () {
		ButtonGrid root = new ButtonGrid();
		root.insertNewCallback(8,destroyStructure,"Demolish");
		ButtonGridManager structurebgm = new ButtonGridManager(root);
		return structurebgm;
	}

	public virtual void assertbgm () {
		GameController.gamecontroller.Foreignbgm = getStructurebgm();
		GameController.gamecontroller.LastSelectedType = GameController.SelectableTypes.Structure;
	}

	protected void destroyStructure () {
		GameController.gamecontroller.Foreignbgm = null;
		Destroy(gameObject);
	}

	public virtual void wasUnselected () {
		GameController.gamecontroller.surrenderContextualMenu ();
	}

	public virtual void wasSelected () {
		assertbgm();
	}

	protected void OnMouseDown () {
		wasSelected();
	}
	
	public abstract void minionLeftStation (Minion minion);

}

public class ClientHandler <T>
{
	public T[] clients;
	int clientLimit;
	int index, count;

	public int Count {
		get {
			return count;
		}
	}

	public ClientHandler (int _clientLimit) {
		clientLimit = _clientLimit;
		clients = new T[_clientLimit];	
		count = 0;
	}

	public bool addClient (T client) {
		if (count>=clientLimit)
			return false;
		if (clients[index]==null) {
			clients[index] = client;
		} else {
			for (int i=0; i<clients.Length; i++) {
				if (clients[i]==null) {
					clients[i] = client;
					index = i+1;
					count++;
					return true;
				}
			}
			return false;
		}
		count++;
		return true;
	}

	public bool RemoveClient (T client) {
		for (int i=0; i<clients.Length; i++) {
			if (clients[i].Equals(client)) {
				clients[i] = default(T);
				index = i;
				count--;
				return true;
			}
		}
		return false;
	}

	public bool hasMaxClients () {
		return (count>=clientLimit);
	}

}