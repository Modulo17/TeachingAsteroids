using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateFuel : MonoBehaviour {

	[Header("Links to other GO's")]
	public	PhysicsShipWithDebug	Player;		//Link in IDE


	Slider	mHealthSlider;


	// Use this for initialization
	void Start () {
		mHealthSlider = GetComponent<Slider> ();	
	}
	
	// Update is called once per frame
	void Update () {
		mHealthSlider.value = Player.Fuel;
	}
}
