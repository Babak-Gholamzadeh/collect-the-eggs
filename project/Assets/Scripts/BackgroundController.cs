using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour
{
	public Transform cameraPos;

	Vector3 startPos;

	void Start ()
	{
		startPos = cameraPos.position;
	}

	void Update ()
	{
		transform.position = new Vector3 ((cameraPos.position.x - startPos.x - 5f) / 2f, transform.position.y, transform.position.z);
	}


}
