using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddHealth : MonoBehaviour {

	Text	mText;

	// Use this for initialization
	void Start () {
		mText = GetComponent<Text> ();		//Get Text Component
		mText.text="Hello";
	}

	public void	NewHealth(int vHealth) {
		mText.text = string.Format ("Health {0}", vHealth);
	}

	public void	NewHealth(float vHealth) {
		NewHealth ((int)vHealth);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
