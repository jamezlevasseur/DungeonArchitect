using UnityEngine;
using System.Collections;

public abstract class Unit : Saveable, GameID {

	public const int STARTING_LEVEL = 1;

	public enum Faction {Good,Neutral,Evil};
	public enum UnitStatDescriptor {Weak, Intermediate, Strong};

	public Transform target;
	public Unit engagedEnemy;
	public GameObject WeaponObject;
	public Material selectedMat;
	public Texture unitPicture;
	
	protected float unitSpeed = 20f;
	protected Vector3[] path;
	protected int targetIndex;
	protected voidCallback arrivedAtDestinationCallback;
	protected int currentWaypoint = 0;
	protected Material unselectedMat;
	protected voidCallback reachedDestinationCallback,pathingFailedCallback;
	protected Vector3 lastLoc;

	//stats
	protected int str;
	protected int agi;
	protected int wis;
	protected int spd;
	protected int hp;
	protected int totalhp;
	protected int arm;
	protected int level;
	protected int exp;
	protected string className;
	protected string unitName;
	protected Faction unitFaction = Faction.Neutral;
	protected bool inCombat;
	protected Weapon myWeapon;
	protected Attack defaultAttack;
	
	/*
	str, agi, wis, arm, and spd should all exist in a 1-10 range
	hp should be anywhere from 100 (very low) to 500 (low) to 1000 (intermediate) to 10,000 (boss health)
	 */
	//public stats
	public virtual int STR { get {return str;} set {str=value;} }
	public virtual int AGI { get {return agi;} set {agi=value;} }
	public virtual int WIS { get {return wis;} set {wis=value;} }
	public virtual int SPD { get {return spd;} set {spd=value;} }
	public virtual int HP { get {return hp;} set {hp=value;} }
	public virtual int TotalHP { get {return totalhp;} set {totalhp=value;} }
	public virtual int ARM { get {return arm;} set {arm=value;} }
	public virtual int Level { get {return level;} set {level=value;} }
	public virtual int EXP { get {return exp;} set {exp=value;} }
	public virtual string ClassName { get {return className;} set {className=value;} }
	public virtual string UnitName { get {return unitName;} set {unitName=value;} }
	public virtual Texture UnitPicture { get {return unitPicture;} set {unitPicture=value;} }
	public virtual Faction UnitFaction {get {return unitFaction;}}
	public virtual bool InCombat {get {return inCombat;} set {inCombat = value;}}
	public virtual Weapon MyWeapon {get {return myWeapon;}}
	public virtual Attack DefaultAttack {get {return defaultAttack;}}

	public long ID {get{return id;}}

	protected void doNothing () {}
	
	protected override void Start () {
		base.Start();
		CombatManager.fighters.Add(this);
	}

	//stats
	public int getStartingStat (UnitStatDescriptor type) {
		switch (type) {
		case UnitStatDescriptor.Weak:
			return Random.Range(1,4);
		case UnitStatDescriptor.Intermediate:
			return Random.Range(4,7);
		case UnitStatDescriptor.Strong:
			return Random.Range(7,10);
		default:
			return Random.Range(4,7);
		}
	}
	
	public int getStartingHP (UnitStatDescriptor type) {
		switch (type) {
		case UnitStatDescriptor.Weak:
			return Random.Range(100,400);
		case UnitStatDescriptor.Intermediate:
			return Random.Range(400,700);
		case UnitStatDescriptor.Strong:
			return Random.Range(700,1000);
		default:
			return Random.Range(400,700);
		}
	}

	//path finding

	public void OnPathFound (Vector3[] newPath, bool pathSuccess) {
		if (pathSuccess) {
			path = newPath;
			if (newPath.Length>0) {
				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
		} else {
			print("EMPTY PATH");
		}
	}
	
	public virtual void goTo (Vector3 targetPosition) {
		PathRequestManager.RequestPath (transform.position,targetPosition, pathFound);
	}
	
	public void pathFound(Vector3[] newPath, bool pathSuccess) {
		if (pathSuccess) {
			print ("FOUND PATH");
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		} else {
			Debug.Log("FAILED TO FIND PATH");
		}
	}

	IEnumerator FollowPath () {
		if (path.Length==0) {
			StopCoroutine("FollowPath");
			yield break;
		}
		Vector3 currentWayPoint = path[0];
		while (true) {
			if (transform.position==currentWayPoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					targetIndex = 0;
					path = new Vector3[0];
					if (arrivedAtDestinationCallback!=null)
						arrivedAtDestinationCallback();
					yield break;
				}
				currentWayPoint = path[targetIndex];
			}
			//rotate
			transform.position = Vector3.MoveTowards(transform.position,currentWayPoint,unitSpeed * Time.deltaTime);
			//if (transform.position.y<1)
				//transform.position = new Vector3(transform.position.x,1,transform.position.z);
			yield return null;
		}
	}

	public void OnDrawGizmos () {
		if (path!=null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i==targetIndex) {
					Gizmos.DrawLine(transform.position,path[i]);
				} else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}

	protected override void OnDestroy () {
		base.OnDestroy();
		onDeath();
	}

	//combat stuff

	public abstract bool attack();
	public abstract void onSpawn ();
	public virtual void onDeath () {
		StopCoroutine("FollowPath");
	}

	public virtual bool takeHit(int damage) {
		hp -=damage;
		StartCoroutine("lookHurt");
		if (hp<1) {
			StartCoroutine("die");
			return true;//did die
		}
		return false;
	}

	IEnumerator lookHurt () {
		Material temp =  GetComponent<MeshRenderer>().material;
		GetComponent<MeshRenderer>().material = GraphicalAssets.graphicalAssets.damagedMaterial;
		yield return new WaitForSeconds(0.5f);
		GetComponent<MeshRenderer>().material = temp;
	}

	IEnumerator die () {
		yield return new WaitForEndOfFrame();
		Destroy(gameObject);
	}

	public void enemyPathFound(Vector3[] newPath, bool pathSuccess) {
		if (pathSuccess) {
			try {
				path = newPath;
				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			} catch (MissingReferenceException e) {
				print (e.Message);
			}
		} else {
			Debug.Log("Failed to find path to enemy");
		}
	}

	IEnumerator moveToEnemy () {
		while (true) {
			if (engagedEnemy!=null) {
				float speed = 15.0f;
				float step = speed * Time.deltaTime;
				Vector3 newDir = Vector3.RotateTowards(transform.forward, engagedEnemy.gameObject.transform.position, step, 0.0F);
				Debug.DrawRay(transform.position, newDir, Color.red);
				transform.rotation = Quaternion.LookRotation(newDir);
			} else {
				endCombat();
				yield break;
			}
			if (Vector3.Distance(transform.position, engagedEnemy.gameObject.transform.position)<defaultAttack.Range) {
				if (attack()) {
					endCombat();
					yield break;
				}
			} else if (lastLoc == transform.position) {
				if (attack()) {
					endCombat();
					yield break;
				}
			}
			lastLoc = transform.position;
			PathRequestManager.RequestPath (transform.position,engagedEnemy.gameObject.transform.position, enemyPathFound);
			yield return new WaitForSeconds(1);
		}
	}

	public virtual void engageEnemy(Unit enemy) {
		engagedEnemy = enemy;
		beginCombat();
		StopCoroutine("moveToEnemy");
		StartCoroutine("moveToEnemy");
	}
	public virtual void beginCombat() {
		InCombat = true;
	}
	public virtual void endCombat() {
		engagedEnemy = null;
		InCombat = false;
	}

}
