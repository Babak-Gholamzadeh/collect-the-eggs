using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public Transform player;

	GameController GMC;
	Vector3 resetPos;

	void Start ()
	{
		GMC = GetComponentInParent <GameController> ();
		if (GMC.currentLevel == 9)
			Destroy (this);
		resetPos = transform.position;
	}

	void Update ()
	{
		GMC.audioController.transform.position = transform.position;
		if (player != null)
		{
			if (player.position.x >= 16 && player.position.x <= 165)
				transform.position = new Vector3 (player.position.x, transform.position.y, transform.position.z);
			else if (player.position.x < 16)
				transform.position = resetPos;
		}
	}
}
