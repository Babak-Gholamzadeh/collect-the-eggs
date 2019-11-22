using UnityEngine;
using System.Collections;

public class AddHeartController : MonoBehaviour
{
	public void falseAnim ()
	{
		GetComponent <Animator> ().SetBool ("AddHeart", false);
	}
}
