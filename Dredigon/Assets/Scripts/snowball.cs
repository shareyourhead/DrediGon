using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class snowball : MonoBehaviour
{
	public Transform sourcePlayer;
	
	public float speed = 18f;
	public float angleLimit = 45f;
	public float travel = 1f;
	
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if(transform.childCount == 0)
		{
			Transform closest = null;
			float minDist = Mathf.Infinity;
			Collider[] colliders = Physics.OverlapSphere(transform.position, 0.75f);
			foreach(Collider c in colliders)
			{
				if(c.tag == "Player" || c.tag == "Dummy")
				{
					float dist = Vector3.Distance(transform.position, c.transform.position);
					if(dist < minDist)
					{
						closest = c.transform;
						minDist = dist;
					}
				}
			}
			if(closest != null)
			{
				closest.parent = gameObject.transform;
				closest.localPosition = new Vector3(0f,1.5f,0f);
			}
		}
	}

	// FixedUpdate is called once per tick
	void FixedUpdate()
	{
		transform.Translate(0f, -0.01f, speed * Time.fixedDeltaTime);
		travel += speed * Time.fixedDeltaTime;
		if (travel >= 40f)
			DestroyBall();
	}
	
	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawRay(contact.point, contact.normal*10f, Color.red);
			float hitAngle = -Mathf.Asin(Vector3.Dot(Vector3.up,contact.normal))*Mathf.Rad2Deg+90;
			if(hitAngle>angleLimit && collision.gameObject.layer == 3)
				DestroyBall();
		}
	}

	public void DestroyBall()
	{
		if(transform.childCount > 0)
		{
			Transform child = transform.GetChild(0);
			child.parent = null;
		}
		PhotonNetwork.Destroy(gameObject);
	}
}
