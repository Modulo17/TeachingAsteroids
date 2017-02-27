using UnityEngine;
using System.Collections;

public class Unrotate : MonoBehaviour {

	Quaternion	mOriginal;

	// Use this for initialization
	void Start () {
		mOriginal = transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.rotation = mOriginal;
	}
}
