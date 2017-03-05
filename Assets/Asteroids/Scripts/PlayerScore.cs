﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {

	public	Text	AsteroidCountText;
	public	Text	ShipCountText;
	public	Text	StateText;
	public	Text	ScoreText;

	public	GameObject	GameOverScreen;
	public	GameObject	HighScoreScreen;
	public	GameObject	IntroScreen;


	// Use this for initialization
	void	Awake() {
		GM.PS = this;		//Link us to Game Manager
	}

	void Start () {
        StartCoroutine(UpdateScore());  //Run CoRoutine in background
	}

	public	bool	GameOver {
		set {
			GameOverScreen.SetActive (value);
		}
	}

	public	bool	HighScore {
		set {
			HighScoreScreen.SetActive (value);
		}
	}
	public	bool	Intro {
		set {
			IntroScreen.SetActive (value);
		}
	}

    IEnumerator UpdateScore() {     //This CoRoutine will run in the background updating the score from the player every 1/2 second
        do {
			AsteroidCountText.text = string.Format("Asteroids {0}",GM.AsteroidCount);
			if(GM.PlayerShip!=null) {
				ShipCountText.text = string.Format("Lives {0}", GM.PlayerShip.Lives);
			} else {
				ShipCountText.text = "No ship";
			}
			StateText.text = string.Format("State {0}", GM.CurrentState);
			if(GM.PlayerShip!=null) {
				ScoreText.text = string.Format("Score {0}", GM.PlayerShip.Score);
			} else {
				ScoreText.text="";
			}
            yield return new    WaitForSeconds(0.5f);       //Show score update every 0.5 seconds
        } while (true);     //Loop forwever
    }
}
