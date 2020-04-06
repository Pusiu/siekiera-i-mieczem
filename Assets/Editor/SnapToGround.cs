using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SnapToGround
{
	[MenuItem("Tools/Snap all to ground")]
    public static void SnapAllToGround()
	{
		Collider[] allCollider = GameObject.FindObjectsOfType<Collider>();

		foreach (Collider c in allCollider)
		{
			Ray r = new Ray(c.transform.position, -Vector3.up);
			RaycastHit h;
			if (Physics.Raycast(r, out h, 1000, 10))
			{
				c.gameObject.transform.position = h.point;
			}
		}
	}
}
