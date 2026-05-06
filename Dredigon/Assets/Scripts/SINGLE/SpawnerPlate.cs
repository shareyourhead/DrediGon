using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPlate : MonoBehaviour
{
	public Transform spawnPoint;
	[SerializeField] private Transform dummy;
	
    void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Player")
			Instantiate(dummy, spawnPoint.position, spawnPoint.rotation);
	}
}
