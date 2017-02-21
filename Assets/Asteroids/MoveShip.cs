using UnityEngine;
using System.Collections;

public class MoveShip : MonoBehaviour
{
    float mSpeed=0.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(0, 0, 360f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))  {
            transform.Rotate(0, 0, -360f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))  {
            mSpeed += 0.1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            mSpeed -= 0.1f;
        }
        transform.position += transform.rotation*Vector3.up*Time.deltaTime*mSpeed;
    }

	public	void	Hyperspace() {
		float	tHeight = Camera.main.orthographicSize;
		float	tWidth = tHeight*Camera.main.aspect;
		float	tNewX = Random.Range (-tWidth, tWidth);
		float	tNewY = Random.Range (-tHeight, tHeight);
		transform.position = new Vector2 (tNewX, tNewY);
	}
}
