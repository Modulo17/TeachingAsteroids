using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {


	public enum AsteroidSize {
		Big=0
		,Medium
		,Small
		,Spaceship
		,None
	}

	[HideInInspector]		//Don't show as its set in code
	public	AsteroidSize mSize;

	Rigidbody2D mRB;  //Keep a reference to the RB

	//Set up start position
	void Start () {
		mRB = GetComponent<Rigidbody2D>(); //Get RB component from GameObject
		RandomMove ();		//Give Asteroid a random direction
	}


	//Random rotation & velocity
	public	void	RandomMove() {
		mRB.angularVelocity = Random.Range(-360f,360f);	//Random rotation
		mRB.velocity = Quaternion.Euler(0,0,Random.Range(0,360f))* Vector2.up;	//Random move
	}

    #region Collision	//Deal with player collisions
    void OnTriggerEnter2D(Collider2D vOther) {   //Must use 2D version
		PlayerShip tSP=vOther.gameObject.GetComponent<PlayerShip>();
		if (tSP) {		//If asteroid hits player, tell Asteroid to split
			Split ();
			GM.CreateExplosion (GM.PlayerShip.transform.position);		//Create explosion effect
			GM.PlayerShip.Die ();										//Destroy player ship
		}
    }


	public	void	Split() {		//Simple state machine, allows Asteroids to split to smaller ones & eventually die
		switch (mSize) {
		case	AsteroidSize.Big:
			GM.CreateAsteroid (transform.position, AsteroidSize.Medium);		//Make 2 medium ones and kill original one
			GM.CreateAsteroid (transform.position, AsteroidSize.Medium);
			GM.AsteroidHitCount++;
			Destroy (gameObject);
			GM.CreateExplosion (transform.position);
			break;
		case	AsteroidSize.Medium:			//Make 3 small ones and kill original one
			GM.CreateAsteroid (transform.position, AsteroidSize.Small);
			GM.CreateAsteroid (transform.position, AsteroidSize.Small);
			GM.CreateAsteroid (transform.position, AsteroidSize.Small);
			GM.AsteroidHitCount++;
			Destroy (gameObject);
			GM.CreateExplosion (transform.position);
			break;
		case	AsteroidSize.Small:		//Small Asteroid just dies
			GM.CreateExplosion (transform.position);
			GM.Counter++;
			GM.AsteroidHitCount++;
			Destroy (gameObject);
			break;
		case	AsteroidSize.Spaceship:		//Spaceship
			GM.CreateExplosion (transform.position);
			GM.UFOHitCount++;
			Destroy (gameObject);
			break;
		}
	}

	public	int	Score {			//Return the score for this kind of asteroid
		get {
			switch (mSize) {
			case	AsteroidSize.Big:
				return	10;
			case	AsteroidSize.Medium:
				return	50;
			case	AsteroidSize.Small:
				return	100;
			default:
				return	0;
			}
		}
	}

    #endregion
}
