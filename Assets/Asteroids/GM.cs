using UnityEngine;
using System.Collections;
using RL_Helpers;

public class GM : Singleton {

    #region
    public enum AsteroidSize {
        Big=0
        ,Medium
        ,Small
        ,None
    }

	public GameObject 	ExplosionPrefab;
	public GameObject 	BulletPrefab;
    public GameObject[] AsteroidPrefabs;

    #endregion


    #region Singleton       //Deal with once only creation
    //We keep the actual variable hidden
    static GM sGM;

    void Awake() {
        if (CreateSingleton<GM>(ref sGM)) {    //Do one time setup inside
        }
    }
    #endregion


    #region Player

    PlayerShip  mPlayerShip;    //Keep variable hidden

    public static PlayerShip PlayerShip {       //Allow access to Player ship globally
        get {
            return sGM.mPlayerShip;
        }
    }

    public static void    RegisterPlayerShip(PlayerShip vPS) {
        sGM.mPlayerShip = vPS;
		for (int tI = 0; tI < 3; tI++) {
			Vector3	tPosition=Quaternion.Euler(0,0,Random.Range(0,360))* Vector3.up;		//Random position 1 unit away
			CreateAsteroid (tPosition+vPS.transform.position, AsteroidSize.Big);
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

	public  static  void    CreateBullet(Vector3 tPosition,Vector3 vVelocity) {
		GameObject	tGO = Instantiate (GM.sGM.BulletPrefab);		//Makes a GameObject from prefab
		Bullet	tmBullet = tGO.GetComponent<Bullet> ();
		tmBullet.transform.position = tPosition;
		tmBullet.Fire (vVelocity);
	}
	public  static  void    CreateExplosion(Vector3 tPosition) {
		GameObject	tGO = Instantiate (GM.sGM.ExplosionPrefab);		//Makes a GameObject from prefab
		tGO.transform.position=tPosition;			//Place it where asteroid splits
		Destroy(tGO,tGO.GetComponent<AudioSource>().clip.length);		//Only last for as long as the audio
	}
    #endregion
	
}
