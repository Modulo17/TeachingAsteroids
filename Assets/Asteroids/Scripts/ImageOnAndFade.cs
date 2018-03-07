using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOnAndFade : MonoBehaviour {


    Image mImage;
	// Use this for initialization
	void Start () {
        mImage = GetComponent<Image>();
        mImage.enabled = false;
	}

    Coroutine mFadeRoutine=null;

	public void ShowAndFade(float vTime) {
        if(mFadeRoutine!=null) {
            StopCoroutine(mFadeRoutine);
        }
        mFadeRoutine = StartCoroutine(DoFade(vTime));
	}

    IEnumerator DoFade(float vTime) {
        mImage.enabled = true;
        Color tColour = mImage.color;
        tColour.a = 1.0f;       //Full Alpha
        yield return new WaitForSeconds(vTime);
        while(tColour.a > 0.0) {
            tColour.a = Mathf.Max(0.0f,tColour.a-Time.deltaTime);
            mImage.color = tColour;
            yield return null;
        }
        mImage.enabled = false;
        mFadeRoutine = null;
    }
}
