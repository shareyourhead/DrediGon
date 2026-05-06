using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovOLD : MonoBehaviour
{
	public CharacterController controller;
	public Transform Fireball;
	
	public float speed = 3.5f;
	public float gravity = 9.8f;
	public float jumpHeight = 1.0f;
	
	public Vector3 velocity;
	
	public float castingDistance = 18f;
	
	public Vector3 movementVector;
	
	public bool isGrounded;
	
	public Vector3 hitNormal;
	public Vector3 hitNormalPerpendicular;
	public float hitHorizontalMagnitude;
	public float hitAngle;
	
	// Update is called once per frame
	void Update()
	{
		movementVector = Vector3.ClampMagnitude(transform.right*Input.GetAxis("Horizontal") + transform.forward*Input.GetAxis("Vertical"),1f) + Vector3.up*movementVector.y;
		
		isGrounded = (controller.isGrounded && hitAngle<=controller.slopeLimit);
			
		if(isGrounded)
		{
			if(Input.GetButtonDown("Jump"))
			{
				movementVector.y = 0f;
				velocity += Vector3.up*Mathf.Sqrt(jumpHeight * 2f * gravity);
				Debug.Log("BRUH");
			}
			{
				movementVector.y = -Mathf.Tan(controller.slopeLimit*Mathf.Deg2Rad)*speed;
				velocity=Vector3.zero;
			}
		}
		{
			movementVector.y = 0f;
			if(!controller.isGrounded)
				velocity.y -= gravity*Time.deltaTime;
		}
		
		controller.Move((movementVector*speed+velocity) * Time.deltaTime);
		
		
		castingDistance = Mathf.Clamp(castingDistance + Input.mouseScrollDelta.y, 1f, 35f);
		if(Input.GetMouseButtonDown(1))
		{
			Transform fireball = GameObject.Instantiate(Fireball, transform.position, transform.rotation);
			//FireballAnchor fireballAnchor = FireballGO.getComponent<FireballAnchor>();
			//fireball.eulerAngles = new Vector3(0f, transform.rotation.y, 0f);
			Debug.Log(transform.rotation.y);
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		hitNormal = hit.normal;
		hitNormalPerpendicular = hitNormal+Vector3.down;
		hitHorizontalMagnitude = new Vector3(hitNormal.x, 0f, hitNormal.z).magnitude;
		hitAngle = Mathf.Atan(hitHorizontalMagnitude/hitNormal.y)*Mathf.Rad2Deg;
		if(!isGrounded)
			velocity += (hitNormal.x*Vector3.right + hitNormal.z*Vector3.forward + (hitNormal.y-1f)*Vector3.up)*gravity*Time.deltaTime;
	}
}
