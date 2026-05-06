using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
	public Rigidbody body;
	public Transform Fireball;
	
	
	public float speed = 4.5f;
	public float jumpHeight = 1.0f;
	
	public Vector3 movementVector;
	public bool isGrounded = true;
	
	public Vector3 hitNormal;
	public Vector3 hitNormalPerpendicular;
	public float hitHorizontalMagnitude;
	public float hitAngle;
	
	public List<List<Vector3>> hitArray;
	
	public float health = 100f;
	public float castingDistance = 18f;
	public float slope;
	
	// Start is called before the first frame update
	void Start()
	{
		body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update()
	{
		movementVector = Vector3.ClampMagnitude(transform.right*Input.GetAxis("Horizontal") + transform.forward*Input.GetAxis("Vertical"),1f);
			
		if(Input.GetButtonDown("Jump"))
		{
			body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight*-2f*Physics.gravity.y), ForceMode.VelocityChange);
			Debug.Log("Jump Pressed");
		}
		
		body.MovePosition(body.position + (movementVector*speed)*Time.deltaTime);
		
		
		castingDistance = Mathf.Clamp(castingDistance + Input.mouseScrollDelta.y, 1f, 35f);
		if(Input.GetMouseButtonDown(1))
		{
			Transform fireball = GameObject.Instantiate(Fireball, transform.position, transform.rotation);
			//FireballAnchor fireballAnchor = FireballGO.getComponent<FireballAnchor>();
			//fireball.eulerAngles = new Vector3(0f, transform.rotation.y, 0f);
			Debug.Log(transform.rotation.y);
		}
	}
	
	void onCollisionEnter(Collision collision)
	{
	}
}
