using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {

    Text[] mText;     //Keep copy of text ref

	// Use this for initialization
	void Start () {
        mText = GetComponentsInChildren<Text>();
        StartCoroutine(UpdateScore());  //Run CoRoutine in background
	}
	
    IEnumerator UpdateScore() {     //This CoRoutine will run in the background updating the score from the player every 1/2 second
        do {
            if(GM.PlayerShip!=null && mText.Length>=3) {
				mText[0].text = string.Format("Asteroids {0}",GM.AsteroidCount);
				mText[1].text = string.Format("Lives {0}", GM.PlayerShip.Lives);
				mText[2].text = string.Format("Score {0}", GM.PlayerShip.Score);
            } else {
				mText[0].text="";
                mText[1].text = string.Format("Player Missing");
				mText[2].text="";
            }
            yield return new    WaitForSeconds(0.5f);       //Show score update every 0.5 seconds
        } while (true);     //Loop forwever
    }
}
