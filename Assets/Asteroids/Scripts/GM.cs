using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RL_Helpers;

public class GM : Singleton {

    #region Prefabs

    public enum AsteroidSize {
        Big=0
        ,Medium
        ,Small
        ,None
    }

	public Text 		GameOverText;
	public GameObject 	ExplosionPrefab;
	public GameObject 	BulletPrefab;
    public GameObject[] AsteroidPrefabs;

	public	Toggle		CheatToggle;

    #endregion


    #region Singleton       //Deal with once only creation
    //We keep the actual variable hidden
    static GM sGM;

    void Awake() {
        if (CreateSingleton<GM>(ref sGM)) {    //Do one time setup inside
			GameOver=true;
        }
    }
    #endregion


    #region Player

    PlayerShip  mPlayerShip;    //Keep variable hidden

	public	static	bool	GameOver {
		get {
			return	sGM.GameOverText.enabled;
		}
		set {
			sGM.GameOverText.enabled = value;
		}
	}

    public static PlayerShip PlayerShip {       //Allow access to Player ship globally
        get {
            return sGM.mPlayerShip;
        }
    }

    public static void    RegisterPlayerShip(PlayerShip vPS) {
		GameOver = false;
        sGM.mPlayerShip = vPS;
    }

	public	static	void	NewAsteroids() {
		for (int tI = 0; tI < 3; tI++) {
			Vector3	tPosition=Quaternion.Euler(0,0,Random.Range(0,360))* Vector3.up;		//Random position 1 unit away
			CreateAsteroid (tPosition+sGM.mPlayerShip.transform.position, AsteroidSize.Big);
		}
	}
	
    #endregion

    #region Asteroids

    public  static  void    CreateAsteroid(Vector3 tPosition,AsteroidSize vSize) {
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
