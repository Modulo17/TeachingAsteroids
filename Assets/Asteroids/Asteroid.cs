using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {
	Rigidbody2D mRB;  //Keep a reference to the RB

	// Use this for initialization
	void Start () {

		mRB = GetComponent<Rigidbody2D>(); //Get RB component from GameObject
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

    #region Collision
    void OnTriggerEnter2D(Collider2D vOther) {   //Must use 2D version
        Debug.Log("OnTriggerEnter2D" + name + " with " + vOther.gameObject.name);
    }

    void OnCollisionEnter2D(Collision2D vOther) {
        Debug.Log("OnCollisionEnter2D" + name + " with " + vOther.gameObject.name);
    }
    #endregion

}
