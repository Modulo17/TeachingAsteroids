using UnityEngine;
using System.Collections;
using UnityEngine.UI;		//Must be included to access UI

public class PhysicsShipWithDebug : MonoBehaviour {

    [Header("User settings")]           //These will show up in inspector

	[Range(0,100f)]
    public float RotationSpeed = 2;
	[Range(0,100f)]
    public float Speed = 10.0f;

	[Header("Links to other GO's")]
	public	Text	DebugText;		//Link in IDE


	[Range(0,1000)]
	public	int		Score=0;
	[Range(0,100)]
	public	int	Fuel=1000;


	Vector3	mStartPosition;
	Quaternion	mStartRotation;

    Rigidbody2D mRB;  //Keep a reference to the RB
	void Start () {
        mRB = GetComponent<Rigidbody2D>(); //Get RB componet from GameObject
        mRB.gravityScale = 0f;      //Turn gravity "off"
		mStartPosition=transform.position;
		mStartRotation = transform.rotation;
	}

	public	void	Reset() {
		transform.position = mStartPosition;
		transform.rotation = mStartRotation;
		mRB.angularVelocity = 0;
		mRB.velocity = Vector2.zero;
	}

    //For Physics we use Fixed Update	
    void FixedUpdate()
    {
		DebugText.text = "";		//Clear old Text		
            if (Input.GetKey(KeyCode.LeftArrow)) {      //Rotate ship, coudl use torque, but this looks better
				mRB.MoveRotation(mRB.rotation+RotationSpeed*Time.deltaTime);
				DebugText.text += "Rotate Left";
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
			mRB.MoveRotation(mRB.rotation-RotationSpeed*Time.deltaTime);
				DebugText.text += "Rotate Right";
    	    }
        if (Input.GetKey(KeyCode.UpArrow)) {    //Apply force in direction of rotation
			Vector2 tForce = Quaternion.Euler(0,0, mRB.rotation)* Vector2.up * Time.deltaTime*  Speed;
            mRB.AddForce(tForce);
			DebugText.text += " Thrust";
			Fuel--;
        }
    }

	void OnTriggerEnter2D(Collider2D vOther) {
		Asteroid	tAsteroid = vOther.GetComponent<Asteroid> ();
		if (tAsteroid != null) {
			Destroy (gameObject);
		}
	}
}
