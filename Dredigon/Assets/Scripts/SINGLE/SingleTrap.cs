using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTrap : MonoBehaviour
{
	public Transform sourcePlayer;
	public float health = 200f;
	public Transform Cylinder;
	//Every time a new trap is spawned, deal 50 damage to existing traps

    // Start is called before the first frame update
	void Start()
	{
		//Cylinder.localEulerAngles = transform.eulerAngles;
		//transform.eulerAngles = Vector3.zero;
	}

	void Update()
    {
		Cylinder.GetComponent<Renderer>().material.color = new Color(0f,0f,Mathf.Clamp01(0.0025f*health));
        if(transform.childCount == 1)
		{
			Transform closest = null;
			float minDist = Mathf.Infinity;
			Collider[] colliders = Physics.OverlapSphere(transform.position, 0.75f);
			foreach(Collider c in colliders)
			{
				if((c.tag == "Player" || c.tag == "Dummy") && c.transform != sourcePlayer.transform)
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
				closest.localPosition = new Vector3(0f,1.2f,0f);
			}
		}
		else
		{
			//Damage self and child
			if(transform.childCount > 1)
			{
				if(transform.GetChild(1).tag == "Player")
					transform.GetChild(1).GetComponent<SinglePlayerMovement>().health = Mathf.Max(transform.GetChild(1).GetComponent<SinglePlayerMovement>().healthMin, transform.GetChild(1).GetComponent<SinglePlayerMovement>().health - 12.5f*Time.deltaTime); 
					sourcePlayer.transform.GetComponent<SinglePlayerMovement>().Points(6.25f*Time.deltaTime);
				if(transform.GetChild(1).tag == "Dummy")
					transform.GetChild(1).GetComponent<SingleDummy>().health = Mathf.Max(0f, transform.GetChild(1).GetComponent<SingleDummy>().health - 12.5f*Time.deltaTime);
					sourcePlayer.transform.GetComponent<SinglePlayerMovement>().Points(6.25f*Time.deltaTime);
			}
			health = Mathf.Max(0f,health-50f*Time.deltaTime);
		}
		if(health == 0f)
		{
			if(transform.childCount > 1)
			{
				Debug.Log(transform.GetChild(1));
				Transform child = transform.GetChild(1);
				child.parent = null;
			}
			sourcePlayer.transform.GetComponent<SinglePlayerMovement>().myTraps.Remove(transform);
			Destroy(gameObject);
		}
    }
}
