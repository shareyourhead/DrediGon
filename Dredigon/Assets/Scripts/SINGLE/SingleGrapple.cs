using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGrapple : MonoBehaviour
{
    public Transform sourcePlayer;
	public Transform sourceCam;
	public Transform claw;
	
	public float speed = 8.5f;
	private float acceleration = 0f;
	
	// Start is called before the first frame update
    void Start()
    {
		sourceCam = sourcePlayer.transform.GetComponent<SinglePlayerMovement>().cam.transform;
		Vector3 looking = sourceCam.transform.GetChild(0).position-sourceCam.transform.position;
		Transform closest = null;
		float minDist = Mathf.Infinity;
		Collider[] colliders = Physics.OverlapCapsule(sourceCam.transform.position+11f*looking, sourceCam.transform.position+200.5f*looking, 10.5f, 448);
		foreach(Collider c in colliders)
		{
			RaycastHit hit;
			Physics.Raycast(sourceCam.transform.position, c.transform.position - sourceCam.transform.position, out hit, 200.5f, 448);
			if(hit.transform == c.transform && (c.tag == "Player" || c.tag == "Dummy") && c.transform != sourcePlayer.transform && Vector3.Angle(looking,c.transform.position-sourceCam.transform.position) <= 15f && Vector3.Distance(sourceCam.transform.position,c.transform.position) <= 200f)
			{
				float dist = Vector3.Angle(looking,c.transform.position-sourceCam.transform.position);
				if(dist < minDist)
				{
					closest = c.transform;
					minDist = dist;
				}
			}
		}
		if(closest != null)
		{
			claw.position = closest.position;
			closest.parent = claw;
			closest.localPosition = Vector3.zero;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if(claw.childCount == 0)
			Destroy(gameObject);
		if(claw.position == transform.position && claw.childCount > 0)
		{
			Transform child = claw.GetChild(0);
			child.parent = null;
		}
		acceleration += 2f*Time.deltaTime;
		claw.position = Vector3.MoveTowards(claw.position, transform.position, (speed+acceleration)*Time.deltaTime);
    }
}
