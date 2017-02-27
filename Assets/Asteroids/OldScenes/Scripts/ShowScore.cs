using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ShowScore : MonoBehaviour {
	
	[Header("Links to other GO's")]
	public	PhysicsShipWithDebug	Player;		//Link in IDE

	Text	ScoreText;

	void	Start() {
		ScoreText = GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () {
		ScoreText.text = string.Format ("Score {0}", Player.Score);		
	}
}
