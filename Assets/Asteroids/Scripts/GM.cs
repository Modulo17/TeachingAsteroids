using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RL_Helpers;

public class GM : Singleton {

    #region Prefabs


	public Text 		GameOverText;
	public GameObject 	ExplosionPrefab;
	public GameObject 	BulletPrefab;
    public GameObject[] AsteroidPrefabs;
	public GameObject 	PlayerPrefab;

	public	Toggle		CheatToggle;

    #endregion


    #region Singleton       //Deal with once only creation
    //We keep the actual variable hidden
    static GM sGM;

    void Awake() {
        if (CreateSingleton<GM>(ref sGM)) {    //Do one time setup inside
			CurrentState=State.None;
			GameObject	tGO=Instantiate (PlayerPrefab);
			mPlayerShip = tGO.GetComponent<PlayerShip> ();
        }
    }
    #endregion

	#region StateMachine

	PlayerScore	mPS;

	public	static	PlayerScore	PS {
		set {
			sGM.mPS = value;
		}
		get {
			return	sGM.mPS;
		}
	}

	public	enum State {
		Invalid
		,None			//Initial state
		,ShowIntro		//Will Show Intro on state entry
		,WaitIntro		//Waiting on Player
		,SpawnAsteroids		//Start game by spawning initial Asteroids
		,WaitPlayerSafe		//Wait until player is safe
		,PlayLevel			//Actually playing a level
		,ReSpawnAsteroids	//More Asteroids needed
		,PlayerLifeLost		//Player Lost a life
		,PlayerNewLife		//Player gets new life
		,PlayerDead			//Player has run out of lives and died
		,GameOver			//Show Game Over
		,WaitGameOver		//Wait for player
	}

	State	mCurrentState=State.Invalid;		//Set Invalid

	//Updates current state calling Exit & Enter 
	public	static	State	CurrentState {
		get {
			return	sGM.mCurrentState;
		}
		set {
			if (sGM.mCurrentState != value) {
				Debug.Log ("Exit State:" + sGM.mCurrentState + " Enter:" + value);
				sGM.ExitState (sGM.mCurrentState);
				sGM.mCurrentState = value;
				sGM.EnterState (sGM.mCurrentState);
			}
		}
	}


	//Called when a state in exited
	void	ExitState(State vState) {
		switch (vState) {

		case	State.WaitIntro:
			PS.Intro = false;
			return;

		case	State.WaitGameOver:
			PS.GameOver = false;
			return;

		case State.PlayLevel:
			PlayerShip.Show (false);
			return;

		default:
			return;
		}
	}


	//Called when a state in entered
	void	EnterState(State vState) {
		switch (vState) {

		case	State.None:
			TriggerChange (State.ShowIntro, 1f);
			return;

		case	State.ShowIntro:
			PS.Intro = true;
			CurrentState = State.WaitIntro;
			return;

		case	State.WaitIntro:
			TriggerChange (State.SpawnAsteroids, KeyCode.Space);
			return;

		case State.ReSpawnAsteroids:		//Used to make more during game
			DeleteBullets ();
			NewAsteroids (PlayerShip.transform.position);
			TriggerChange (State.WaitPlayerSafe, 1f);
			return;

		case State.SpawnAsteroids:		//Initial asteroids
			DeleteBullets ();
			NewAsteroids ();
			TriggerChange (State.WaitPlayerSafe, 1f);
			return;

		case State.PlayLevel:		//Changing from this state is controlled using DuringState()
			PlayerShip.Show (true);
			return;

		case State.PlayerLifeLost:
			TriggerChange (State.PlayerNewLife, 5f);
			return;

		case State.PlayerNewLife:
			if (PlayerShip.Lives == 0) {
				CurrentState = State.PlayerDead;
			} else {
				PlayerShip.ReSpawn ();
				CurrentState = State.PlayLevel;
			}
			return;

		case	State.PlayerDead:
			PS.GameOver = true;
			TriggerChange (State.WaitGameOver, 5f);
			return;

		default:
			return;
		}
	}


	//Handle update, here we process the Druing states
	void	Update() {
		DuringState (CurrentState);
	}


	//This is called in Update to process State
	void	DuringState(State vState) {
		switch (vState) {

		case	State.WaitPlayerSafe:
			if (IsPlayerSafe (PlayerShip.transform.position)) {
				CurrentState = State.PlayLevel;
			}
			return;

		case	State.PlayLevel:
			 if (AsteroidCount == 0) {
				CurrentState = State.ReSpawnAsteroids;
			}
			return;

		default:
			return;
		}
	}

	//Some helpers to call CoRoutine with correct parameters

	//Trigger state change after time has ellapsed
	public	void	TriggerChange(State vNewState,float vTime) {
		StartCoroutine(StateChangeCoRoutine(vNewState,vTime,0));		//Use zero key, so its ignored
	}

	//Trigger state change when key is pressed 
	public	void	TriggerChange(State vNewState,KeyCode vKey) {
		StartCoroutine(StateChangeCoRoutine(vNewState,-1f,vKey));		//Use negative time , so its ignored
	}

	//Trigger state change when key is pressed or timeout
	public	void	TriggerChange(State vNewState,float vTime, KeyCode vKey) {
		StartCoroutine(StateChangeCoRoutine(vNewState,vTime,vKey));		//Use Key and time
	}
		
	//Trigger state change after timeout
	IEnumerator	StateChangeCoRoutine(State vNewState,float vTime, KeyCode vKey) {
		bool	tTrigger = false;
		do {
			if(vKey!=0) {		//If Key is zero ignore it
				if(Input.GetKey(vKey)) {
					tTrigger=true;
				}
			}
			if(vTime>0f) {		//If Time is negative zero coming in ignore it
				vTime-=Time.deltaTime;	//New TimeSlice
				if(vTime<=0f) {		//Have we triggered
					vTime=0f;
					tTrigger=true;
				}
			}
			yield	return null;
		} while(!tTrigger);
		CurrentState = vNewState;	//Go to New State
	}

	#endregion


    #region Player

    PlayerShip  mPlayerShip;    //Keep variable hidden

    public static PlayerShip PlayerShip {       //Allow access to Player ship globally
        get {
            return sGM.mPlayerShip;
        }
    }

	public	static	void	NewAsteroids(Vector3 vOrigin,int vCount=3) {
		for (int tI = 0; tI < vCount; tI++) {
			Vector3	tPositionOnRadius=Quaternion.Euler(0,0,Random.Range(0,360))* Vector3.up*2f;		//Random position 2 units radius from center
			CreateAsteroid (tPositionOnRadius+vOrigin, Asteroid.AsteroidSize.Big);
		}

	}
	public	static	void	NewAsteroids(int vCount=3) {
		Vector3	tOrigin = Vector3.zero;
		NewAsteroids (tOrigin, vCount);
	}
	
    #endregion

    #region Asteroids


	//Is player ship safe, ie no asteroids near the player
	public bool	IsPlayerSafe (Vector3 vPlayerPosition,float vRadius=1.5f) {
		Asteroid[] tAsteroids = FindObjectsOfType<Asteroid> ();	//Look for Asteroid which are too close
		foreach (var tAsteroid in tAsteroids) {
			Vector2	tPosition = (Vector2)vPlayerPosition - (Vector2)tAsteroid.transform.position;
			if (tPosition.magnitude < vRadius) {
				return false;	//Too Close
			}
		}
		return	true;	//We are good to go
	}

	//Create an Asteroid from a prefab
	public  static  void    CreateAsteroid(Vector3 tPosition,Asteroid.AsteroidSize vSize) {
		int	tIndex = (int)vSize;		//Converts enum to int (safe)
		if (tIndex < GM.sGM.AsteroidPrefabs.Length) {		//Make sure we have sufficent prefabs
			GameObject	tGO = Instantiate (GM.sGM.AsteroidPrefabs [tIndex]);		//Makes a GameObject from prefab
			Asteroid	tAsteroid = tGO.GetComponent<Asteroid> ();
			tAsteroid.mSize = vSize;		//So Asteroid knows what type it is, so it knows the next type to go to
			tGO.transform.position = tPosition;		//Move it to location
		}
    }

	//Simple Particle explosion
	public  static  void    CreateExplosion(Vector3 tPosition) {
		GameObject	tGO = Instantiate (GM.sGM.ExplosionPrefab);		//Makes a GameObject from prefab
		tGO.transform.position=tPosition;			//Place it where asteroid splits
		Destroy(tGO,tGO.GetComponent<AudioSource>().clip.length);		//Only last for as long as the audio
	}


	public	static	int	AsteroidCount {
		get {
			Asteroid[] tAsteroids = FindObjectsOfType<Asteroid> ();
			return	tAsteroids.Length;
		}
	}

	public	static	bool	Cheat {		//Are we in cheat mode?
		get {
			if (sGM.CheatToggle != null) {
				return	sGM.CheatToggle.isOn;
			}
			return	false;
		}
	}


	#endregion

	#region Bullets

	public	static	void	DeleteBullets() {
		Bullet[] tBullets = FindObjectsOfType<Bullet> ();
		foreach (var tBullet in tBullets) {
			Destroy (tBullet.gameObject);
		}
	}

	public  static  void    CreateBullet(Vector3 tPosition,Vector3 vVelocity, float vTimeToLive=1f) {
		GameObject	tGO = Instantiate (GM.sGM.BulletPrefab);		//Makes a GameObject from prefab
		Bullet	tmBullet = tGO.GetComponent<Bullet> ();
		tmBullet.transform.position = tPosition;
		tmBullet.Fire (vVelocity,vTimeToLive);
	}

		
    #endregion
	
}
