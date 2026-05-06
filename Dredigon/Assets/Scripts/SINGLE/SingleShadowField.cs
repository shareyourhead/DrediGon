using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShadowField : MonoBehaviour
{
	public Transform sourcePlayer;
	
	public float lifetime = 20f;

	// Update is called once per frame
	void Update()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position,6.5f);
		foreach(Collider c in colliders)
		{
			if(c.tag == "Player" && c.transform != sourcePlayer)
			{
				SinglePlayerMovement player = c.transform.GetComponent<SinglePlayerMovement>();
				player.gravityCoef = -Vector3.Normalize(Vector3.MoveTowards(transform.position, c.transform.position, 1f)-transform.position+Vector3.up*0.5f);
				player.yVel = Mathf.Min(-1f,player.yVel);
				player.health = Mathf.Max(player.healthMin,player.health-80f*Time.deltaTime);
				sourcePlayer.transform.GetComponent<SinglePlayerMovement>().Points(80f*Time.deltaTime);
			}
			//Dev testing dummy
			if(c.tag == "Dummy")
			{
				SingleDummy dummy = c.transform.GetComponent<SingleDummy>();
				dummy.gravityCoef = -Vector3.Normalize(Vector3.MoveTowards(transform.position, c.transform.position, 1f)-transform.position+Vector3.up*0.5f);
				dummy.yVel = Mathf.Min(-1f,dummy.yVel);
				dummy.health = Mathf.Max(0f,dummy.health-80f*Time.deltaTime);
				sourcePlayer.transform.GetComponent<SinglePlayerMovement>().Points(80f*Time.deltaTime);
			}
		}
		lifetime -= Time.deltaTime;
		if(lifetime <= 0f)
		{
			Destroy(gameObject);
		}
	}
}
