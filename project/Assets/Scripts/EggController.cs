using UnityEngine;
using System.Collections;

public class EggController : MonoBehaviour {

	public LayerMask whatIsPlayer;
	public bool found = false;
	public Animator eggAnimator;
	public bool isBigEgg = false;
	public GameObject effect;

	GameObject effectObject;
	GameObject audioController;
	GameObject cam;
	Animator anim;
	GameController GMC;
	void Start ()
	{
		GMC = GetComponentInParent <GameController> ();
		audioController = GMC.audioController;
		cam = GMC.mainCamera;
		anim = GetComponent <Animator> ();
	}

	void Update ()
	{
		if (cam.transform.position.x < (transform.position.x - 25f) ||
			cam.transform.position.x > (transform.position.x + 25f))
			anim.enabled = false;
		else
			anim.enabled = true;
		
		Vector3 destPos = cam.transform.position;
		destPos = new Vector3 (destPos.x + 10f, destPos.y + 8.5f, 0);
		if (IsFound () && !found)
		{
			effectObject = (GameObject) Instantiate (effect, transform.position, transform.rotation);
			effectObject.transform.parent = gameObject.transform;

			GMC.countEggsAnimator.SetBool ("AddEgg", true);
			audioController.GetComponent <AudioManagement> ().egg.GetComponent <AudioSource> ().Play ();

			if (isBigEgg)
				GMC.eggs += 10;
			else
				GMC.eggs++;

			PlayerPrefs.SetInt ("Eggs", GMC.eggs);

			eggAnimator.SetBool ("IsFound", true);

			found = true;
		}

		if (found)
		{
			transform.position = Vector3.Lerp (transform.position, destPos, 5f * Time.deltaTime);
		}
	}

	bool IsFound ()
	{
		return Physics2D.OverlapCircle (transform.position, transform.localScale.x, whatIsPlayer);
	}

	public void DestroyEgg ()
	{
		Destroy (gameObject);
	}
}
