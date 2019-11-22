using UnityEngine;
using System.Collections;

public class BoxMovingController : MonoBehaviour
{
	public Transform start, end, box;
	public float speed = 5f;
	public LayerMask whatIsPlayer;
	public bool isPlayer = false;

	GameController GMC;
	bool goToEnd = true;
	Vector3 posPlayer;
	Vector3 posBox;

	void Start ()
	{
		GMC = GetComponentInParent <GameController> ();
	}

	void Update ()
	{
		isPlayer = Physics2D.OverlapBox (new Vector2 (box.position.x, box.position.y + 2f), new Vector2 (1f, 1f), 0f, whatIsPlayer);
		if (isPlayer)
		{
			posPlayer = GMC.currentPlayer.transform.position;
			posBox = box.position;
		}

		if (goToEnd)
		{
			box.position = Vector3.MoveTowards (box.position, end.position, Time.deltaTime * speed);
			if ((Mathf.Abs (box.position.x - end.position.x) + Mathf.Abs (box.position.y - end.position.y)) < 0.1f)
				goToEnd = false;
			if (isPlayer)
				GMC.currentPlayer.transform.position = box.position - (posBox - posPlayer);
		}
		else
		{
			box.position = Vector3.MoveTowards (box.position, start.position, Time.deltaTime * speed);
			if ((Mathf.Abs (box.position.x - start.position.x) + Mathf.Abs (box.position.y - start.position.y)) < 0.1f)
				goToEnd = true;
			if (isPlayer)
				GMC.currentPlayer.transform.position = box.position - (posBox - posPlayer);
		}
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine (start.position, end.position);
	}

}
