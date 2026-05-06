using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagerPlate : MonoBehaviour
{
	void OnTriggerStay(Collider collider)
	{
		if(collider.tag == "Player")
		{
			SinglePlayerMovement p = collider.GetComponent<SinglePlayerMovement>();
			p.health = Mathf.Max(p.healthMin, p.health-40f*Time.deltaTime);
		}
		if(collider.tag == "Dummy")
		{
			SingleDummy d = collider.GetComponent<SingleDummy>();
			d.health -= 40f*Time.deltaTime;
		}
	}
}
