using UnityEngine;
using System.Collections;

public class KillOffscreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame, after all other processing is done
	void FixedUpdate () {
        float tHeight = Camera.main.orthographicSize;       //Height
        float tWidth = tHeight * Camera.main.aspect;
        if (transform.position.y > tHeight) {
			Destroy (gameObject);
			return;
        }
        if (transform.position.y < -tHeight) {
			Destroy (gameObject);
			return;
        }

        if (transform.position.x > tWidth) {
			Destroy (gameObject);
			return;
        }

        if (transform.position.x < -tWidth) {
			Destroy (gameObject);
			return;
        }
    }
}
