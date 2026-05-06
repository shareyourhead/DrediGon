using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballRigidbody : MonoBehaviour
{
	public float speed;
	private Rigidbody body;
	public Vector3 hitNormal;
	public float hitAngle = 0f;
	public float angleLimit = 45f;
	
	// Start is called before the first frame update
	void Start()
	{
		body = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Vector3 movement = transform.position+transform.forward*speed*Time.deltaTime;
		movement = movement - Vector3.up*0.01f;
		body.MovePosition((movement));
	}
	
	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawRay(contact.point*10f, contact.normal, Color.red);
			
			hitNormal = contact.normal;
		
			hitAngle = -Mathf.Asin(Vector3.Dot(Vector3.up,hitNormal))*Mathf.Rad2Deg+90;
			
			if(hitAngle>angleLimit && collision.gameObject.layer == 3)
				Destroy(gameObject);
		}
		
	}
}
