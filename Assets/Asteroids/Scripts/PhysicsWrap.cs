using UnityEngine;
using System.Collections;

public class PhysicsWrap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame, after all other processing is done
	void FixedUpdate () {
        float tHeight = Camera.main.orthographicSize;       //Height
        float tWidth = tHeight * Camera.main.aspect;
        if (transform.position.y > tHeight) {
            transform.position += Vector3.down * tHeight * 2f;
        }
        if (transform.position.y < -tHeight) {
            transform.position += Vector3.up * tHeight * 2f;
        }

        if (transform.position.x > tWidth) {
            transform.position += Vector3.left * tWidth * 2f;
        }

        if (transform.position.x < -tWidth) {
            transform.position += Vector3.right * tWidth * 2f;
        }
    }
}
