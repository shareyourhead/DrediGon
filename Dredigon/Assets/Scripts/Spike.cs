using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spike : MonoBehaviour
{
	public Transform sourcePlayer;
	
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward*9f,ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
		if(Physics.CheckSphere(transform.position, 0.375f, 448))
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, 0.375f, 448);
			foreach(Collider c in colliders)
			{
				if(c.tag == "Player" && c.transform != sourcePlayer)
				{
					PlayerMovement player = c.transform.GetComponent<PlayerMovement>();
					Vector3 vec = transform.rotation*Vector3.back;
					player.TakeKnockback(-4f, new Vector3(vec.x,Mathf.Min(-0.5f, vec.y),vec.z));
					player.TakeDamage(20f);
					// player.gravityCoef = new Vector3(vec.x,Mathf.Min(-0.5f, vec.y),vec.z);
					// player.yVel = -4f;
					// player.health = Mathf.Max(player.healthMin,player.health-20f);
					sourcePlayer.transform.GetComponent<PlayerMovement>().Points(20f);
				}
				if(c.tag == "Dummy")
				{
					Dummy dummy = c.transform.GetComponent<Dummy>();
					Vector3 vec = transform.rotation*Vector3.back;
					dummy.gravityCoef = new Vector3(vec.x,Mathf.Min(-0.5f, vec.y),vec.z);
					dummy.yVel = -4f;
					dummy.health = Mathf.Max(0f,dummy.health-20f);
					sourcePlayer.transform.GetComponent<PlayerMovement>().Points(20f);
				}
			}
			PhotonNetwork.Destroy(gameObject);
		}
		if(Physics.CheckSphere(transform.position, 0.375f, 8))
		{
			PhotonNetwork.Destroy(gameObject);
		}
    }
}
