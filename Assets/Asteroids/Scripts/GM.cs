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

	#region States

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
		,None
		,ShowIntro
		,WaitIntro
		,SpawnAsteroids
		,WaitPlayerSafe
		,PlayLevel
		,PlayerDead
		,GameOver
		,WaitGameOver
		,AsteroidsGone
	}

	State	mCurrentState=State.Invalid;

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

	void	EnterState(State vState) {
		switch (vState) {

		case	State.None:
			StartCoroutine(TimedStateChange(1f,State.ShowIntro));
			return;

		case	State.ShowIntro:
			PS.Intro = true;
			CurrentState = State.WaitIntro;
			return;

		case	State.WaitIntro:
			StartCoroutine(InputStateChange(KeyCode.Space,State.SpawnAsteroids));
			return;

		case State.SpawnAsteroids:
			NewAsteroids ();
			StartCoroutine(TimedStateChange(1f,State.WaitPlayerSafe));
			return;

		case State.PlayLevel:
			PlayerShip.Show (true);
			return;

		case	State.PlayerDead:
			PS.GameOver = true;
			StartCoroutine(TimedStateChange(5f,State.WaitGameOver));
			return;

		default:
			return;
		}
	}

	void	Update() {
		ProcessState (CurrentState);
	}

	void	ProcessState(State vState) {
		switch (vState) {

		case	State.WaitPlayerSafe:
			if (IsPlayerSafe (PlayerShip.transform.position)) {
				CurrentState = State.PlayLevel;
			}
			return;

		case	State.PlayLevel:
			if (PlayerShip.Lives == 0) {
				CurrentState=State.PlayerDead;
			}
			return;

		default:
			return;
		}
	}
	#endregion

	//Trigger state change after timeout
	IEnumerator	TimedStateChange(float vTime, State vNewState) {
		yield	return	new	WaitForSeconds (vTime);
		CurrentState = vNewState;
	}

	//Trigger state change if key pressed
	IEnumerator	InputStateChange(KeyCode vKey, State vNewState) {
		while(!Input.GetKey(vKey)) {
			yield	return null;
		}
		CurrentState = vNewState;
	}


    #region Player

    PlayerShip  mPlayerShip;    //Keep variable hidden

    public static PlayerShip PlayerShip {       //Allow access to Player ship globally
        get {
            return sGM.mPlayerShip;
        }
    }

	public	static	void	NewAsteroids() {
		for (int tI = 0; tI < 3; tI++) {
			Vector3	tPosition=Quaternion.Euler(0,0,Random.Range(0,360))* Vector3.up;		//Random position 1 unit away
			CreateAsteroid (tPosition+sGM.mPlayerShip.transform.position, Asteroid.AsteroidSize.Big);
		}
	}
	
    #endregion

    #region Asteroids


	//Is player ship safe, ie no asteroids near the player
	public bool	IsPlayerSafe (Vector3 vPlayerPosition) {
		Asteroid[] tAsteroids = FindObjectsOfType<Asteroid> ();
		foreach (var tAsteroid in tAsteroids) {
			Vector2	tPosition = (Vector2)vPlayerPosition - (Vector2)tAsteroid.transform.position;
			if (tPosition.magnitude < 2f) {
				return false;	//Too Close
			}
		}
		return	true;
	}


	public  static  void    CreateAsteroid(Vector3 tPosition,Asteroid.AsteroidSize vSize) {
		int	tIndex = (int)vSize;		//Converts enum to int (safe)
		if (tIndex < GM.sGM.AsteroidPrefabs.Length) {		//Make sure we have sufficent prefabs
			GameObject	tGO = Instantiate (GM.sGM.AsteroidPrefabs [tIndex]);		//Makes a GameObject from prefab
			Asteroid	tAsteroid = tGO.GetComponent<Asteroid> ();
			tAsteroid.mSize = vSize;		//So Asteroid knows what type it is, so it knows the next type to go to
			tGO.transform.position = tPosition;		//Move it to location
		}
    }

	public  static  void    CreateBullet(Vector3 tPosition,Vector3 vVelocity, float vTimeToLive=1f) {
		GameObject	tGO = Instantiate (GM.sGM.BulletPrefab);		//Makes a GameObject from prefab
		Bullet	tmBullet = tGO.GetComponent<Bullet> ();
		tmBullet.transform.position = tPosition;
		tmBullet.Fire (vVelocity,vTimeToLive);
	}
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

	public	bool	CheckPlayerWin() {
		if (AsteroidCount > 0) {
			return	false;
		} else {
			GM.NewAsteroids ();
			return	true;
		}
	}
		
    #endregion
	
}
