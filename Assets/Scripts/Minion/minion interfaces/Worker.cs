using UnityEngine;
using System.Collections;

public interface Worker {

	//dig methods
	void volunteerToDig();
	void resignFromDig();
	bool canDig (GameObject nodeToDig);
	void finishedDig();
	void setupForNextDig();
	void cancelCurrentDigJob();

}
