using UnityEngine;
using System.Collections;

public class CarrotController : MonoBehaviour {

	public int direction = 1;
	public float speed = 1f;

	Rigidbody2D rigid2D;

	void Start ()
	{
		rigid2D = GetComponent <Rigidbody2D> ();
		transform.localScale = new Vector3 (transform.localScale.x * direction, transform.localScale.y, transform.localScale.z);
		Destroy (gameObject, 1f);
	}

	void Update ()
	{
		rigid2D.velocity = new Vector2 (speed * direction, 0f);

	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (!other.CompareTag ("Player"))
			Destroy (gameObject);
	}
}
