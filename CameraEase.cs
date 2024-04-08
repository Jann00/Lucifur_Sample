using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEase : MonoBehaviour
{

	[SerializeField] Transform target;

	[Header("Movement"),SerializeField,Tooltip("How fast the Camera moves")]
	float smoothTime = .5f;
	Vector3 velocity;

	Vector3 offset;
	// Start is called before the first frame update
	void Start()
    {
		offset = new Vector3(0, 0, -10);
		if (target == null)
			target = GameObject.FindGameObjectWithTag("Player").transform;
		transform.position = target.position + offset;

	}
	
    // Update is called once per frame
    void FixedUpdate()
    {
		if (target != null)
			transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
	}
}
