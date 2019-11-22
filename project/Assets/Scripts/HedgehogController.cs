using UnityEngine;
using System.Collections;

public class HedgehogController : MonoBehaviour {

	public Transform start, end, hedgehog;
	public float speed = 5f;
	public bool isShooter;
	public GameObject arw;
	public float timeShoot = 2f;
	public float lifeTimeShoot = 0.8f;

	bool goToEnd = true;
	bool isRight = true;
	int countDeath = 3;
	int directionIndex = 1;
	float timeSpaceShooting;
	GameObject audioController;
	GameObject camPos;
	AudioSource shootAudio;
	AudioSource damageAudio;
	CircleCollider2D moveCollide;

	void Start ()
	{
		timeSpaceShooting = timeShoot;
		audioController = GetComponentInParent <GameController> ().audioController;
		camPos = GetComponentInParent <GameController> ().mainCamera;
		shootAudio = audioController.GetComponent <AudioManagement> ().hedgehogShoot.GetComponent <AudioSource> ();
		damageAudio = audioController.GetComponent <AudioManagement> ().hedgehogDamage.GetComponent <AudioSource> ();
		moveCollide = GetComponent <CircleCollider2D> ();
	}

	void Update ()
	{
		if (countDeath <= 0)
			return;
		
		if (goToEnd)
		{
			if (hedgehog.position.x > end.position.x && isRight)
				Flip ();
			else if (hedgehog.position.x < end.position.x && !isRight)
				Flip ();
			
			hedgehog.position = Vector3.MoveTowards (hedgehog.position, end.position, Time.deltaTime * speed);
			if (Mathf.Abs (hedgehog.position.x - end.position.x) < 0.1f)
				goToEnd = false;
		}
		else
		{
			if (hedgehog.position.x > start.position.x && isRight)
				Flip ();
			else if (hedgehog.position.x < start.position.x && !isRight)
				Flip ();
			
			hedgehog.position = Vector3.MoveTowards (hedgehog.position, start.position, Time.deltaTime * speed);
			if (Mathf.Abs (hedgehog.position.x - start.position.x) < 0.1f)
				goToEnd = true;
		}
		MoveCollider ();

		if (isShooter)
			ShootArw ();
	}

	void ShootArw ()
	{
		if (camPos.transform.position.x < (hedgehog.position.x - 25f) ||
			camPos.transform.position.x > (hedgehog.position.x + 25f))
			return;
		if (timeSpaceShooting > 0)
			timeSpaceShooting -= Time.deltaTime;
		
		if (timeSpaceShooting < 0.1f)
		{
			GameObject tempArrow;
			if (directionIndex == 1)
				tempArrow = (GameObject) Instantiate (arw , new Vector3 (hedgehog.position.x, hedgehog.position.y + 1f, hedgehog.position.z), Quaternion.Euler (new Vector3 (0, 0, 90f)));
			else if (directionIndex == 2)
				tempArrow = (GameObject) Instantiate (arw , new Vector3 (hedgehog.position.x, hedgehog.position.y + 1f, hedgehog.position.z), Quaternion.Euler (new Vector3 (0, 0, 45f)));
			else if (directionIndex == 3)
				tempArrow = (GameObject) Instantiate (arw , new Vector3 (hedgehog.position.x, hedgehog.position.y + 1f, hedgehog.position.z), Quaternion.Euler (new Vector3 (0, 0, 0)));
			else if (directionIndex == 4)
				tempArrow = (GameObject) Instantiate (arw , new Vector3 (hedgehog.position.x, hedgehog.position.y + 1f, hedgehog.position.z), Quaternion.Euler (new Vector3 (0, 0, -45f)));
			else
				tempArrow = (GameObject) Instantiate (arw , new Vector3 (hedgehog.position.x, hedgehog.position.y + 1f, hedgehog.position.z), Quaternion.Euler (new Vector3 (0, 0, -90f)));
			tempArrow.transform.parent = gameObject.transform;
			tempArrow.GetComponent <ArrowController> ().setDirec (directionIndex++);
			if (!shootAudio.isPlaying)
				shootAudio.Play ();
		}
		if (directionIndex > 5)
		{
			directionIndex = 1;
			timeSpaceShooting = timeShoot;
		}
	}

	void Flip ()
	{
		isRight = !isRight;
		hedgehog.localScale = new Vector3 (hedgehog.localScale.x * -1, hedgehog.localScale.y, hedgehog.localScale.z);
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine (start.position, end.position);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.CompareTag ("CarrotArrow") && countDeath > 0)
		{
			damageAudio.Play ();
			countDeath--;
		}
		if (countDeath == 2)
			hedgehog.GetComponent <SpriteRenderer> ().color = Color.red;
		else if (countDeath == 1)
			hedgehog.GetComponent <SpriteRenderer> ().color = Color.black;
		else if (countDeath == 0)
		{
			countDeath--;
			hedgehog.GetComponent <Rigidbody2D> ().AddForce (new Vector2 (0, 1000f));
			Destroy (hedgehog.GetComponent <EdgeCollider2D> ());
			Destroy (hedgehog.GetComponent <CircleCollider2D> ());
			Destroy (GetComponent <CircleCollider2D> ());
			hedgehog.GetComponent <SpriteRenderer> ().sortingLayerName = "Topest";
		
			Destroy (gameObject, 3f);
		}
	}

	void MoveCollider ()
	{
		moveCollide.offset = new Vector2 (hedgehog.localPosition.x, hedgehog.localPosition.y + 0.68f);
	}
}
