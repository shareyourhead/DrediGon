using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
	public Transform sourcePlayer;
	public CharacterController controller;
	public Transform Capsule;
	
	public float health = 100f;
	
	public Vector3 movementVector = Vector3.zero;
	public float gravity = 9.81f;
	public float yVel = 0f;
	public Vector3 gravityCoef = Vector3.up;
	
	public bool isGrounded;
	private bool controllerIsGrounded;
	private bool hitAngleIsGrounded;
	private Vector3 hitNormal = Vector3.zero;
	private Vector3 hitHorizontal = Vector3.zero;
	private float hitHorizontalMagnitude = 0f;
	private float hitAngle = 0f;
	private float angleLimit = 40f;
	
	void Start()
	{
		//Set name to instance ID
		gravity = Physics.gravity.y;
		Capsule.GetComponent<Renderer>().material.color = new Color(1f,0f,1f);
	}
	
	// Update is called once per frame
	void Update()
	{
	  //Dead?
		if(health == 0f)
			Destroy(gameObject);
		
	  //APPLY MOVEMENT
		if(transform.parent == null)
			controller.Move((movementVector + gravityCoef*yVel + Vector3.down*0.01f) * Time.deltaTime);
		
	  //Health color
		Capsule.GetComponent<Renderer>().material.color = new Color(Mathf.Clamp01(2f-0.02f*health),Mathf.Clamp01(0.02f*health),0f,1f);
		
	  //Grounded
		controllerIsGrounded = controller.isGrounded;
		hitAngleIsGrounded = (hitAngle <= angleLimit);
		isGrounded = (controller.isGrounded && hitAngle <= angleLimit);
		if(transform.parent != null)
			isGrounded = true;
		
	  //Gravitational Acceleration
		yVel += gravity*Time.deltaTime;
		//was just vector3.up
		gravityCoef = gravityCoef*(1f-2f*Time.deltaTime)+Vector3.up*2f*Time.deltaTime;
		if(isGrounded)
		{
			yVel = Mathf.Max(yVel, 0f);
			gravityCoef = Vector3.up;
		}
		else
		{
			if(controller.isGrounded)
			{
				//There's GOTTA be a way to get the perpendicular normal more simply
				gravityCoef = new Vector3(hitHorizontal.x*Mathf.Cos(Mathf.Acos(hitHorizontalMagnitude)+Mathf.PI/2f),Mathf.Sin(Mathf.Asin(hitNormal.y)+Mathf.PI/2f),hitHorizontal.z*Mathf.Cos(Mathf.Acos(hitHorizontalMagnitude)+Mathf.PI/2f));
			}
		}
		
	  //Water?
		if(transform.position.y <= 0.8f && health > 0f)
			health = Mathf.Max(0f, health - 15f * Time.deltaTime);
		
	  //Debug DUMMY path
		Debug.DrawRay(transform.position, Vector3.Normalize(gravityCoef), Color.blue, 0.0f, false);
	  //Independent Error Logging
		if((transform.localRotation.x != 0f) || (transform.localRotation.z != 0f))
			Debug.Log("(Dummy) ERROR: X and/or Z Rot is not 0");
		if( health<0f  )
			Debug.Log("(Dummy) ERROR: Health is below 0");
		if( float.IsNaN(health) )
			Debug.Log("(Dummy) ERROR: Health is NaN");
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		hitNormal = hit.normal;
		hitHorizontal = Vector3.Normalize(new Vector3(hitNormal.x,0f,hitNormal.z));
		hitHorizontalMagnitude = new Vector3(hitNormal.x, 0f, hitNormal.z).magnitude;
		hitAngle = Mathf.Atan(hitHorizontalMagnitude/hitNormal.y)*Mathf.Rad2Deg;
	}
}
