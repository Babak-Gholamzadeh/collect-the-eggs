using UnityEngine;
using System.Collections;

public class BoxFallDownController : MonoBehaviour
{
	public Transform start, end, box;
	public float speed = 5f;

	bool collide = false;
	bool goToEnd = false;
	EdgeCollider2D edgeCollider2D;

	void Start ()
	{
		edgeCollider2D = GetComponent <EdgeCollider2D> ();
	}

	void Update ()
	{
		if (collide || goToEnd)
			box.position = Vector3.Lerp (box.position, end.position, Time.deltaTime * speed / 4f);

		if (Mathf.Abs (box.position.y - end.position.y) < 0.1f)
			goToEnd = false;

		if (!goToEnd && !collide)
			box.position = Vector3.MoveTowards (box.position, start.position, Time.deltaTime * speed);
	
	}

	void FixedUpdate ()
	{
		edgeCollider2D.offset = new Vector2 (0f, box.localPosition.y + 2.3f);
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine (start.position, end.position);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.CompareTag ("Player"))
		{
			collide = true;
			goToEnd = true;
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		collide = false;
	}
}
