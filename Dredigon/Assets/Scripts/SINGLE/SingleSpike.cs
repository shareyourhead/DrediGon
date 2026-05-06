using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSpike : MonoBehaviour
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
					SinglePlayerMovement player = c.transform.GetComponent<SinglePlayerMovement>();
					Vector3 vec = transform.rotation*Vector3.back;
					player.gravityCoef = new Vector3(vec.x,Mathf.Min(-0.5f, vec.y),vec.z);
					player.yVel = -4f;
					player.health = Mathf.Max(player.healthMin,player.health-20f);
					sourcePlayer.transform.GetComponent<SinglePlayerMovement>().Points(20f);
				}
				if(c.tag == "Dummy")
				{
					SingleDummy dummy = c.transform.GetComponent<SingleDummy>();
					Vector3 vec = transform.rotation*Vector3.back;
					dummy.gravityCoef = new Vector3(vec.x,Mathf.Min(-0.5f, vec.y),vec.z);
					dummy.yVel = -4f;
					dummy.health = Mathf.Max(0f,dummy.health-20f);
					sourcePlayer.transform.GetComponent<SinglePlayerMovement>().Points(20f);
				}
			}
			Destroy(gameObject);
		}
		if(Physics.CheckSphere(transform.position, 0.375f, 8))
		{
			Destroy(gameObject);
		}
    }
}
