using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballProjectile : MonoBehaviour
{
	
	public float angleLimit = 45f;
	public float speed = 2f;
	public float radius = 0.5f;
	private float rayLength;
	
	public float gravity = 9.8f;
	public float velocity;
	public bool isGrounded;
	public LayerMask groundMask;
	
	public Vector3 rollingRotation = Vector3.zero;
	
	// Start is called before the first frame update
	void Start()
	{
		rayLength = radius/Mathf.Sin((angleLimit+90f)*Mathf.Deg2Rad);
	}

	// Update is called once per frame
	void Update()
	{
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		rollingRotation = new Vector3(Mathf.Repeat(rollingRotation.x + speed*Time.deltaTime/radius*Mathf.Rad2Deg, 360f), 0, 0);
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward)*radius, Color.blue);
		Debug.DrawRay(transform.position, Vector3.down*rayLength, Color.yellow);
		
		RaycastHit hit;
		isGrounded = Physics.Raycast(transform.position, Vector3.down*rayLength, out hit, rayLength, groundMask);
		
		if (isGrounded==true)
		{
			transform.Translate (Vector3.up * (rayLength-hit.distance), Space.World);
			velocity = 0f;
		}
		{
			velocity = velocity - gravity * Time.deltaTime;
		}
		
		transform.Translate(Vector3.up * velocity * Time.deltaTime, Space.World);
	}
	
	private void OnCollisionEnter(Collision collision)
	{
		Destroy(gameObject);
	}
}
