using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RL_Helpers;
using System.Text;

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
		,None				//Initial state
		,ShowIntro			//Will Show Intro on state entry
		,NewGame			//Start New Game
		,SpawnAsteroids		//Start game by spawning initial Asteroids
		,WaitPlayerSafe		//Wait until player is safe
		,NewLevel			//Start new Level
		,PlayLevel			//Actually playing a level
		,PlayerLifeLost		//Player Lost a life
		,PlayerNewLife		//Player gets new life
		,PlayerDead			//Player has run out of lives and died
		,GameOver			//Show Game Over
		,WarpPlayer			//Player Warp
	}

	State	mCurrentState=State.Invalid;		//Set Invalid
	State	mPreviousState=State.Invalid;

	//Updates current state calling Exit & Enter 
	public	static	State	CurrentState {
		get {
			return	sGM.mCurrentState;
		}
		private	set {
			if (sGM.mCurrentState != value) {
				Debug.Log ("Exit State:" + sGM.mCurrentState + " Enter:" + value);
				sGM.ExitState (sGM.mCurrentState);
				sGM.mPreviousState = sGM.mCurrentState;
				sGM.mCurrentState = value;
				sGM.EnterState (sGM.mCurrentState);
			}
		}
	}


	//Called when a state in exited
	void	ExitState(State vState) {
		switch (vState) {

		case	State.ShowIntro:
			PS.Intro = false;
			return;

		case	State.PlayerDead:
			PS.GameOver = false;
			PlayerShip.Lives = 3;
			break;

		case State.PlayLevel:
			PlayerShip.Show (false);
			return;

		default:
			return;
		}
	}

	void	NewGameResetVariables() {
		PlayerShip.Lives = 3;		//reset counters
		PlayerShip.Score = 0;
		mPlayTime = 0f;
		mLevel = 0;
		mAsteroidHitCount = 0;
		mBulletCount = 0;
		mUFOCount = 0;
		mUFOHitCount = 0;
		mCheatTime = 0f;
		mCheatCount = 0;
	}

	//Called when a state in entered
	void	EnterState(State vState) {
		switch (vState) {

		case	State.None:
			TriggerChange (State.NewGame, 3f);
			return;

		case	State.ShowIntro:
			PS.Intro = true;
			return;

		case State.NewGame:
			NewGameResetVariables ();
			NewAsteroids ();
			LogGameStateEvent();
			TriggerChange (State.ShowIntro, 3f);
			break;

		case State.SpawnAsteroids:		//Initial asteroids
			NewAsteroids ();
			TriggerChange (State.NewLevel, 1f);
			return;
		
		case State.NewLevel: 
			TriggerChange (State.WaitPlayerSafe);
			return;

		case State.PlayLevel:		//Changing from this state is controlled using DuringState()
			LogGameStateEvent();
			PlayerShip.Show (true);
			return;

		case State.PlayerLifeLost:
			PlayerShip.Show (false);
			TriggerChange (State.PlayerNewLife, 5f);
			return;

		case State.PlayerNewLife:
			LogGameStateEvent();
			if (PlayerShip.Lives == 0) {
				CurrentState = State.PlayerDead;
			} else {
				PlayerShip.ReSpawn ();
				CurrentState = State.NewLevel;
			}
			return;

		case	State.PlayerDead:
			LogGameStateEvent();
			PS.GameOver = true;
			TriggerChange (State.NewGame, 5f);
			return;

		case	State.WarpPlayer:
			LogGameStateEvent ();
			PlayerShip.Warp ();
			TriggerChange (State.PlayLevel, 1f);
			return;

		default:
			return;
		}
	}

	void	OnDestroy () {
		LogGameEvent ("Quit game");
	}

	//Handle update, here we process the Druing states
	void	Update() {
		DuringState (CurrentState);
		mPlayTime += Time.deltaTime;
	}


	public	int	mCounter;
	public	static	int	Counter {
		get {
			return	sGM.mCounter;
		}
		set {
			sGM.mCounter = value;
		}
	}

	//This is called in Update to process State
	void	DuringState(State vState) {
		switch (vState) {

		case	State.ShowIntro:
			if (Input.GetKey (KeyCode.Space)) {
				TriggerChange (State.NewLevel);
			}
			return;

		case	State.WaitPlayerSafe:
			if (IsPlayerSafe (PlayerShip.transform.position)) {
				CurrentState = State.PlayLevel;
			}
			return;

		case	State.PlayLevel:
			if (AsteroidCount == 0) {
				CurrentState = State.SpawnAsteroids;
				mLevel++;
			}
			if (Counter > 10) {
				if (Random.Range (0, 100f) < 10f) {
					Counter = 0;
					mUFOCount++;
					GM.CreateAsteroid (RandomScreenPosition, Asteroid.AsteroidSize.Spaceship);
				}
			}
			if (Cheat) {
				mCheatTime += Time.deltaTime;
			}
			return;
		default:
			return;
		}
	}

	//Some helpers to call CoRoutine with correct parameters
	static	public	void	TriggerChange(State vNewState) {
		CurrentState = vNewState;
	}
	//Trigger state change after time has ellapsed
	static	public	void	TriggerChange(State vNewState,float vTime) {
		sGM.StartCoroutine(sGM.StateChangeCoRoutine(vNewState,vTime,0));		//Use zero key, so its ignored
	}

	//Trigger state change when key is pressed 
	static	public	void	TriggerChange(State vNewState,KeyCode vKey) {
		sGM.StartCoroutine(sGM.StateChangeCoRoutine(vNewState,-1f,vKey));		//Use negative time , so its ignored
	}

	//Trigger state change when key is pressed or timeout
	static	public	void	TriggerChange(State vNewState,float vTime, KeyCode vKey) {
		sGM.StartCoroutine(sGM.StateChangeCoRoutine(vNewState,vTime,vKey));		//Use Key and time
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
	public	static	void	NewAsteroids() {
		int	tCount;
		if (sGM.mLevel < 4) {
			tCount = sGM.mLevel + 3;
		} else {
			tCount = 7;
		}
		DeleteObjects<Bullet> ();
		DeleteObjects<Asteroid> ();
		Vector3	tOrigin = Vector3.zero;
		NewAsteroids (tOrigin, tCount);
	}
	
    #endregion

    #region Asteroids


	//Is player ship safe, ie no asteroids near the player
	public bool	IsPlayerSafe (Vector3 vPlayerPosition) {
		bool	tIsSafe = true;
		Asteroid[] tAsteroids = FindObjectsOfType<Asteroid> ();	//Look for Asteroid which are too close
		foreach (var tAsteroid in tAsteroids) {
			Vector2	tPosition = (Vector2)vPlayerPosition - (Vector2)tAsteroid.transform.position;
			CircleCollider2D	tCol = tAsteroid.GetComponent<CircleCollider2D> ();
			SpriteRenderer		tSR = tAsteroid.GetComponent<SpriteRenderer> ();
			float	tRadius = tCol.radius*2f;
			if (tPosition.magnitude < tRadius) {
				tIsSafe = false;	//Too Close
				tSR.color = Color.red;
			} else {
				tSR.color = Color.white;
			}
		}
		return	tIsSafe;	//We are good to go
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

	public	static	void	DeleteObjects<T>() where T:MonoBehaviour{
		T[] tObjects = FindObjectsOfType<T> ();
		foreach (var tO in tObjects) {
			Destroy (tO.gameObject);
		}
	}

	public  static  void    CreateBullet(Vector3 tPosition,Vector3 vVelocity, float vTimeToLive=1f) {
		GameObject	tGO = Instantiate (GM.sGM.BulletPrefab);		//Makes a GameObject from prefab
		Bullet	tmBullet = tGO.GetComponent<Bullet> ();
		tmBullet.transform.position = tPosition;
		tmBullet.Fire (vVelocity,vTimeToLive);
		sGM.mBulletCount++;
	}

	public	static	Vector2	ScreenSize {
		get {
			return	new Vector2 (Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
		}
	}

	public	static	Vector2	RandomScreenPosition {
		get {
			Vector2	tSize=ScreenSize;
			return	new Vector2 (Random.Range(-tSize.x,tSize.x),Random.Range(-tSize.y,tSize.y));
		}
	}

    #endregion


	#region Analytics

	public	float	mPlayTime=0f;
	public	int		mLevel=0;
	static	public	int		Level {
		get {
			return	sGM.mLevel;
		}
	}
	public	int		mBulletCount=0;
	public	float	mCheatTime = 0;

	public	int 	mCheatCount = 0;
	static	public	int CheatCount {
		get {
			return	sGM.mCheatCount;
		}
		set {
			sGM.mCheatCount = value;
		}
	}

	public	int		mAsteroidHitCount=0;
	static	public	int		AsteroidHitCount {
		get {
			return	sGM.mAsteroidHitCount;
		}
		set {
			sGM.mAsteroidHitCount = value;
		}
	}
	public	int		mUFOHitCount=0;
	static	public	int		UFOHitCount {
		get {
			return	sGM.mUFOHitCount;
		}
		set {
			sGM.mUFOHitCount = value;
		}
	}

	public	int		mUFOCount=0;
	static	public	int		UFOCount {
		get {
			return	sGM.mUFOCount;
		}
		set {
			sGM.mUFOCount = value;
		}
	}

	static	public	void	LogGameStateEvent() {
		LogGameEvent (CurrentState.ToString ());
	}
	static	public	void	LogGameEvent(string vEvent) {
		StringBuilder tSB = new StringBuilder ();
		Dictionary<string,object>	tDetail = new Dictionary<string,object> ();
		tDetail.Add("Level",string.Format("Level{0}",sGM.mLevel));
		tDetail.Add("Playtime",sGM.mPlayTime);
		tDetail.Add("BulletCount",sGM.mBulletCount);
		tDetail.Add("AsteroidHitCount",sGM.mAsteroidHitCount);
		tDetail.Add("CheatTime",sGM.mCheatTime);
		tDetail.Add("Asteroids",AsteroidCount);
		//Analytics.CustomEvent (vEvent, tDetail);
		tSB.AppendFormat ("Event:{0}",vEvent);
		foreach (var tItem in tDetail) {
			tSB.AppendFormat (" {0}",tItem.ToString());
		}
		DebugMsg (tSB.ToString ());
	}

	#endregion
}
