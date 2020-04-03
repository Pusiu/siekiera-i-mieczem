using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static void LookAtYOnly(this Transform t, Vector3 pos)
	{
		t.LookAt(pos);
		//t.rotation = Quaternion.Euler(0, t.rotation.y, 0);
	}
}
