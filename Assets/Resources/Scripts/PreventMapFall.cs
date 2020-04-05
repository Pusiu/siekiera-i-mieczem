using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventMapFall : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInChildren<Rigidbody>() != null)
			other.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;

		other.gameObject.transform.position = new Vector3(other.transform.position.x, 10, other.transform.position.z);
	}
}
