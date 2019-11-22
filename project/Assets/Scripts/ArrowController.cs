using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {

	float speed = 5f;

	void Start ()
	{
		Destroy (gameObject, GetComponentInParent <HedgehogController> ().lifeTimeShoot);
		Invoke ("changeLayer", 0.2f);
	}

	public void setDirec (int dir)
	{
		if (dir == 1)
		{
			GetComponent <Rigidbody2D> ().velocity = new Vector2 (-speed, 0);
		}
		else if (dir == 2)
		{
			GetComponent <Rigidbody2D> ().velocity = new Vector2 (-speed, speed);
		}
		else if (dir == 3)
		{
			GetComponent <Rigidbody2D> ().velocity = new Vector2 (0, speed);
		}
		else if (dir == 4)
		{
			GetComponent <Rigidbody2D> ().velocity = new Vector2 (speed, speed);
		}
		else
		{
			GetComponent <Rigidbody2D> ().velocity = new Vector2 (speed, 0);
		}
	}

	void changeLayer ()
	{
		GetComponent <SpriteRenderer> ().sortingLayerName = "Foreground";
		GetComponent <SpriteRenderer> ().sortingOrder = 2;
	}
}
