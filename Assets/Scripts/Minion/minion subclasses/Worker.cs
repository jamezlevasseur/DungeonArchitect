using UnityEngine;
using System.Collections;

public abstract class Worker : Minion {

	public abstract void volunteerToDig();
	public abstract bool canDig (GameObject nodeToDig);
	public abstract void finishedDig();
	public abstract void setupForNextDig();
	public abstract void cancelCurrentDigJob();

}
