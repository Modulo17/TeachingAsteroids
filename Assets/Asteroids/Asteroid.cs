using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {
	Rigidbody2D mRB;  //Keep a reference to the RB

	// Use this for initialization
	void Start () {
		mRB = GetComponent<Rigidbody2D>(); //Get RB componet from GameObject
		mStartPosition=transform.position;
		mStartRotation = transform.rotation;
		Reset ();
	}

	Vector3	mStartPosition;
	Quaternion	mStartRotation;

	public	void	Reset() {
		transform.position = mStartPosition;
		transform.rotation = mStartRotation;
		mRB.angularVelocity = 0;
		mRB.velocity = Quaternion.Euler(0,0,Random.Range(0,360f))* Vector2.up;	//Random move
	}

	// Update is called once per frame
	void Update () {
	
	}
}
