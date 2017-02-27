using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    [Header("User settings")]           //These will show up in inspector
    [Range(180f, 720f)]
    public float RotationSpeed = 180f;
    [Range(0, 100f)]
    public float Speed = 10.0f;

    #region Score
    int mScore = 0;     //Keep score private

    public int Score {
           get {
            return mScore;
        }
    }

    #endregion

    void Start() {
        GM.RegisterPlayerShip(this);        //Register ship with Game Manager
        mRB = GetComponent<Rigidbody2D>(); //Get RB componet from GameObject
        mRB.gravityScale = 0f;      //Turn gravity "off"
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
    }
    #endregion

    #region Collision
    void OnTriggerEnter2D(Collider2D vOther) {   //Must use 2D version
        Debug.Log("OnTriggerEnter2D" + name + " with " + vOther.gameObject.name);
    }

    void OnCollisionEnter2D(Collision2D vOther) {
        Debug.Log("OnCollisionEnter2D" + name + " with " + vOther.gameObject.name);
    }
    #endregion

}
