using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {

    Text mText;     //Keep copy of text ref

	// Use this for initialization
	void Start () {
        mText = GetComponent<Text>();
        StartCoroutine(UpdateScore());  //Run CoRoutine in background
	}
	
    IEnumerator UpdateScore() {     //This CoRoutine will run in the background updating the score from the player every 1/2 second
        do {
            if(GM.PlayerShip!=null) {
				mText.text = string.Format("Lives {1} Score {0}", GM.PlayerShip.Score,GM.PlayerShip.Lives);
            } else {
                mText.text = string.Format("Player Dead");
            }
            yield return new    WaitForSeconds(0.5f);       //Show score update every 0.5 seconds
        } while (true);     //Loop forwever
    }
}
