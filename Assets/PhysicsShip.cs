using UnityEngine;
using System.Collections;

public class PhysicsShip : MonoBehaviour {

    [Header("User settings")]           //These will show up in inspector

	[Range(0,100f)]
    public float RotationSpeed = 2;
	[Range(0,100f)]
    public float Speed = 10.0f;

    Rigidbody2D mRB;  //Keep a reference to the RB
	void Start () {
        mRB = GetComponent<Rigidbody2D>(); //Get RB componet from GameObject
        mRB.gravityScale = 0f;      //Turn gravity "off"
	}

    //For Physics we use Fixed Update	
    void FixedUpdate()
    {
            if (Input.GetKey(KeyCode.LeftArrow)) {      //Rotate ship, coudl use torque, but this looks better
				mRB.AddTorque(RotationSpeed*Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
			mRB.AddTorque(-RotationSpeed*Time.deltaTime);
    	    }
        if (Input.GetKey(KeyCode.UpArrow)) {    //Apply force in direction of rotation
			Vector2 tForce = Quaternion.Euler(0,0, mRB.rotation)* Vector2.up * Time.deltaTime*  Speed;
            mRB.AddForce(tForce);
        }
    }
}
