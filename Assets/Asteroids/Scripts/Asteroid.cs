using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	[HideInInspector]		//Don't show as its set in code
	public	GM.AsteroidSize mSize;

	Rigidbody2D mRB;  //Keep a reference to the RB

	//Set up start position
	void Start () {
		mRB = GetComponent<Rigidbody2D>(); //Get RB component from GameObject
		RandomMove ();
	}

	public	void	RandomMove() {
		mRB.angularVelocity = Random.Range(-360f,360f);	//Random rotation
		mRB.velocity = Quaternion.Euler(0,0,Random.Range(0,360f))* Vector2.up;	//Random move
	}

    #region Collision
    void OnTriggerEnter2D(Collider2D vOther) {   //Must use 2D version
		PlayerShip tSP=vOther.gameObject.GetComponent<PlayerShip>();
		if (tSP) {
			Split ();
			GM.CreateExplosion (GM.PlayerShip.transform.position);
			GM.PlayerShip.Die ();
		}
    }


	public	void	Split() {		//Simple state machine
		switch (mSize) {
		case	GM.AsteroidSize.Big:
			GM.CreateAsteroid (transform.position, GM.AsteroidSize.Medium);		//Make 2 medium ones and kill big one
			GM.CreateAsteroid (transform.position, GM.AsteroidSize.Medium);
			Destroy (gameObject);
			GM.CreateExplosion (transform.position);
			return;
		case	GM.AsteroidSize.Medium:
			GM.CreateAsteroid (transform.position, GM.AsteroidSize.Small);
			GM.CreateAsteroid (transform.position, GM.AsteroidSize.Small);
			GM.CreateAsteroid (transform.position, GM.AsteroidSize.Small);
			GM.CreateExplosion (transform.position);
			Destroy (gameObject);
			return;
		case	GM.AsteroidSize.Small:
			GM.CreateExplosion (transform.position);
			Destroy (gameObject);
			return;
		}
	}

	public	int	Score {
		get {
			switch (mSize) {
			case	GM.AsteroidSize.Big:
				return	10;
			case	GM.AsteroidSize.Medium:
				return	50;
			case	GM.AsteroidSize.Small:
				return	100;
			default:
				return	0;
			}
		}
	}

    void OnCollisionEnter2D(Collision2D vOther) {
    }
    #endregion

}
