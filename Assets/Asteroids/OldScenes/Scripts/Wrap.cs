using UnityEngine;
using System.Collections;

public class Wrap : MonoBehaviour {

    Rigidbody2D mRB;  //Keep a reference to the RB
    void Start() {
        mRB = GetComponent<Rigidbody2D>(); //Get RB componet from GameObject
    }
	
	// Update is called once per frame
	void Update () {
        float tHeight = Camera.main.orthographicSize;       //Height
        float tWidth = tHeight * Camera.main.aspect;
        Vector2 tPosition = mRB.position;

        if (tPosition.y > tHeight)  {
            tPosition.y -= tHeight * 2f;
        } else if (tPosition.y < -tHeight) {
            tPosition.y += tHeight * 2f;
        }

        if (tPosition.x > tWidth) {
            tPosition.x -= tWidth * 2f;
        }

        if (tPosition.x <- tWidth) {
            tPosition.x += tWidth * 2f;
        }
        mRB.MovePosition(tPosition);
    }
}
