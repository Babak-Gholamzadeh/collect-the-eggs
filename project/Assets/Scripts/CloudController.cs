using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour {

	float speed = 1f;

	void Update ()
	{
		transform.position = Vector3.MoveTowards (transform.position, new Vector3 (-20f, transform.position.y, transform.position.z), Time.deltaTime * speed);
		if (transform.position.x < -5)
			Destroy (gameObject);
	}
}
