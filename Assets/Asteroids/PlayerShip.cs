using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    [Header("User settings")]           //These will show up in inspector
    [Range(180f, 720f)]
    public float RotationSpeed = 180f;
    [Range(0, 50f)]
    public 	float Speed = 20.0f;
	[Range(0, 3f)]
	public 	float BulletSpeed = 1.0f;



	public	GameObject	BulletSpawn;

    #region Score

	Vector3	mStartPosition;

    int mScore = 0;     //Keep score private

	int	mLives = 4;

    public int Score {
           get {
            return mScore;
        }
		set {
			mScore = value;
		}
    }

	public	int Lives {
		get {
			return	mLives;
		}
	}

	public	void	Die() {
		Show (false);
		if (mLives > 0) {
			mLives--;
			Invoke("ReSpawn",2f);	//Show ship again
		}
	}

    #endregion

	void	Show(bool vShow) {		//Show or hide ship
		GetComponent<SpriteRenderer> ().enabled = vShow;
		GetComponent<Collider2D> ().enabled = vShow;
	}

    void Start() {
        GM.RegisterPlayerShip(this);        //Register ship with Game Manager
        mRB = GetComponent<Rigidbody2D>(); //Get RB component from GameObject
        mRB.gravityScale = 0f;      //Turn gravity "off"
		mStartPosition=transform.position;
    }

	void	ReSpawn() {
		mRB.velocity = Vector2.zero;
		transform.position=mStartPosition;
		transform.rotation = Quaternion.identity;
		Show (true);
	}

    #region PhysicsMove
    Rigidbody2D mRB;  //Keep a reference to the RB
    //For Physics we use Fixed Update	
    void FixedUpdate() {
        if (Input.GetKey(KeyCode.LeftArrow)) {      //Rotate ship, coudl use torque, but this looks better
            mRB.MoveRotation(mRB.rotation+(RotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            mRB.MoveRotation(mRB.rotation-(RotationSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.UpArrow)) {    //Apply force in direction of rotation
            Vector2 tForce = Quaternion.Euler(0, 0, mRB.rotation) * Vector2.up * Time.deltaTime * Speed;
            mRB.AddForce(tForce);
        }
		if (CoolDown() && Input.GetKey(KeyCode.Space)) {
			Vector3	tFire = transform.rotation*Vector3.up;
			GM.CreateBullet (BulletSpawn.transform.position, (BulletSpawn.transform.position-transform.position).normalized*BulletSpeed);
		}
    }
    #endregion


	#region Cooldown
	public	float	Fire=0.25f;

	float	mFireTimer=0f;

	bool	CoolDown() {
		if(mFireTimer>=Fire) {
			mFireTimer = 0;
			return	true;
		}
		mFireTimer += Time.deltaTime;
		return	false;
	}
	#endregion

}
