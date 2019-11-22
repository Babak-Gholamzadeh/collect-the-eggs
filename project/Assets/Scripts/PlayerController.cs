using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speedMovement = 10f;
	public bool characterIsRight = true;
	public float jumpForce = 7000f;
	public bool doubleJump = false;
	public bool isGrounded = false;
	public Transform groundCheck;
	public LayerMask whatIsGround;
	public Transform enemyCheck;
	public LayerMask whatIsEnemy;
	public GameObject Carrot;
	public float timeSpaceShooting = 0.5f;
	public bool enemyCollide = false;
	public bool deadLineCollide = false;
	public bool willDie = false;
	public LayerMask whatIsDeadLine;
	public bool endLevel = false;
	public LayerMask whatIsEndLevel;
	public Animator rabbitAnimator;
	public bool dontMove = false;
	public bool boxPath = false;
	public Vector3 boxPathPos;
	public float moveBtn;

	GameObject audioController;
	Rigidbody2D rigid2D;
	AudioSource jumpAudio;
	AudioSource shootAudio;
	GameController GMC;
	float fixJumpTime = 0.05f;
	float countFixJumpTime = 0.05f;

	void Start ()
	{
		GMC = GetComponentInParent <GameController> ();
		audioController = GMC.audioController;
		rigid2D = GetComponent <Rigidbody2D> ();
		jumpAudio = audioController.GetComponent <AudioManagement> ().jump1.GetComponent <AudioSource> ();
		shootAudio = audioController.GetComponent <AudioManagement> ().rabbitShoot.GetComponent <AudioSource> ();
	}

	void Update ()
	{
		if (!willDie)
		{
			isGrounded = Physics2D.OverlapBox (groundCheck.position, groundCheck.localScale, 0f, whatIsGround);
			ShootCarrot (false);
			Jump (false);
		}

		DetectEnemy ();

		if ((enemyCollide || GMC.currentTime <= 0) && !willDie)
		{
			willDie = true;
			rigid2D.AddForce (new Vector2 (0, 1000f));
			foreach (CircleCollider2D collider2d in GetComponents <CircleCollider2D> ())
				Destroy (collider2d);
			GetComponent <SpriteRenderer> ().sortingLayerName = "Topest";
			audioController.GetComponent <AudioManagement> ().enemyCollide.GetComponent <AudioSource> ().Play ();
			if (Random.Range (0.1f, 2.1f) < 1f)
				transform.Rotate (new Vector3 (0, 0, -25));
			else
				transform.Rotate (new Vector3 (0, 0, 25));

		}
		if (deadLineCollide)
		{
			GMC.isDead = true;
			Destroy (gameObject);
		}
		if (endLevel)
		{
			GMC.isEndLevel = 1;
			gameObject.SetActive (false);
		}
		
	}

	void FixedUpdate ()
	{
		if (!willDie)
			Movement ();
	}

	public void ShootCarrot (bool trigun)
	{
		if (timeSpaceShooting > 0)
			timeSpaceShooting -= Time.deltaTime;
		
		if ((Input.GetKeyDown (KeyCode.Space) || trigun) && timeSpaceShooting < 0.1f)
		{
			shootAudio.Play ();
			GameObject tempCarrot = (GameObject) Instantiate (Carrot, new Vector3 (transform.position.x, transform.position.y - 0.5f, transform.position.z), new Quaternion (0, 0, 0, 0));
			if (characterIsRight)
				tempCarrot.GetComponent <CarrotController> ().direction = 1;
			else
				tempCarrot.GetComponent <CarrotController> ().direction = -1;
			timeSpaceShooting = 0.4f;
		}
	}

	void DetectEnemy ()
	{
		enemyCollide = Physics2D.OverlapBox (enemyCheck.position, enemyCheck.localScale, 0f, whatIsEnemy);
		deadLineCollide = Physics2D.OverlapBox (enemyCheck.position, enemyCheck.localScale, 0f, whatIsDeadLine);
		endLevel = Physics2D.OverlapBox (enemyCheck.position, enemyCheck.localScale, 0f, whatIsEndLevel);
	}

	public void Jump (bool jumpBtn)
	{
		if (countFixJumpTime > 0 && isGrounded)
		{
			countFixJumpTime -= Time.deltaTime;
			return;
		}
		rabbitAnimator.SetBool ("Jump", !isGrounded);
		if (isGrounded)
			doubleJump = false;
		if ((Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow) || jumpBtn) && (isGrounded || !doubleJump) )
		{
			countFixJumpTime = fixJumpTime;
			rigid2D.AddForce (new Vector2 (0, jumpForce), ForceMode2D.Force);
			if (!isGrounded && !doubleJump)
				doubleJump = true;
			jumpAudio.Play ();
		}
	}

	void Movement ()
	{
		float move = Input.GetAxis ("Horizontal") + moveBtn;
		if (move > 0 && !characterIsRight)
			Flip ();
		else if (move < 0 && characterIsRight)
			Flip ();

		if (dontMove || dontMove2 (characterIsRight))
		{
			if (characterIsRight && move > 0 && !isGrounded)
				return;
			if (!characterIsRight && move < 0 && !isGrounded)
				return;
		}
		rabbitAnimator.SetFloat ("Speed", Mathf.Abs (move));

		rigid2D.velocity = new Vector2 (move * speedMovement, rigid2D.velocity.y);
	}

	void Flip ()
	{
		characterIsRight = !characterIsRight;
		transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
	}

	void OnCollisionEnter2D (Collision2D other)
	{
		if (!isGrounded)
			dontMove = true;
	}


	void OnCollisionStay2D (Collision2D other)
	{
		if (!isGrounded)
			dontMove = true;
	}

	void OnCollisionExit2D (Collision2D other)
	{
		dontMove = false;
	}

	bool dontMove2 (bool isRight)
	{
		Vector2 direct;

		if (isRight)
			direct = Vector2.right;
		else
			direct = -Vector2.right;

		RaycastHit2D hitUp = Physics2D.Raycast (new Vector2 (transform.position.x + 1.5f, transform.position.y + 1f), direct, 0.5f);
		RaycastHit2D hitDown = Physics2D.Raycast (new Vector2 (transform.position.x + 1.5f, transform.position.y - 1f), direct, 0.5f);

		if (hitUp.collider)
			if (hitUp.transform.tag == "BoxPath")
				return true;

		if (hitDown.collider)
		if (hitDown.transform.tag == "BoxPath")
			return true;

		return false;
	}
}