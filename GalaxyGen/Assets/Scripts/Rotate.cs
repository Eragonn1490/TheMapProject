using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	//Public
	public float speed = -10;

	private void Update()
	{
		transform.Rotate(Vector3.forward, speed * Time.deltaTime);
	}
}
