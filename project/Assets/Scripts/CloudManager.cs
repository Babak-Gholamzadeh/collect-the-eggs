using UnityEngine;
using System.Collections;

public class CloudManager : MonoBehaviour
{
	public GameObject[] Clouds = new GameObject[3];
	float timeSpaceCreate = 5f;

	void Update ()
	{
		if (timeSpaceCreate > 0)
			timeSpaceCreate -= Time.deltaTime;
		if (timeSpaceCreate < 0.1f)
		{
			GameObject tempCloud = (GameObject)	Instantiate (Clouds [Random.Range (0, 3)], new Vector3 (transform.position.x, transform.position.y + (Random.Range (-1f, 1f)), transform.position.z), new Quaternion (0, 0, 0, 0));
			tempCloud.transform.parent = gameObject.transform;
			timeSpaceCreate = Random.Range (6f, 9f);
		}
	}
}
