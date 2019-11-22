using UnityEngine;
using System.Collections;

public class AddEggController : MonoBehaviour
{
	public void falseAnim ()
	{
		GetComponent <Animator> ().SetBool ("AddEgg", false);
	}
}
