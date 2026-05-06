using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerPlate : MonoBehaviour
{
	void OnTriggerStay(Collider collider)
	{
		if(collider.tag == "Player")
		{
			SinglePlayerMovement p = collider.GetComponent<SinglePlayerMovement>();
			p.health = Mathf.Min(100f, p.health+40f*Time.deltaTime);
		}
		if(collider.tag == "Dummy")
		{
			SingleDummy d = collider.GetComponent<SingleDummy>();
			d.health = Mathf.Min(100f, d.health+40f*Time.deltaTime);
		}
	}
}
