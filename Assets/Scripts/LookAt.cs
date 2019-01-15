using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
	//Public
	public Transform target;

	private void Update()
	{
		transform.right = -(target.position - transform.position).normalized;
	}
}
