using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grapple : MonoBehaviour
{
    public Transform sourcePlayer;
	public Transform sourceCam;
	public Transform claw;
	
	public float speed = 8.5f;
	private float acceleration = 0f;
	
	PhotonView view;
	
	// Start is called before the first frame update
    void Start()
    {
		view = GetComponent<PhotonView>();
		sourceCam = sourcePlayer.transform.GetComponent<PlayerMovement>().cam.transform;
		Vector3 looking = sourceCam.transform.GetChild(0).position-sourceCam.transform.position;
		Transform closest = null;
		float minDist = Mathf.Infinity;
		Collider[] colliders = Physics.OverlapCapsule(sourceCam.transform.position+11f*looking, sourceCam.transform.position+200.5f*looking, 10.5f, 448);
		foreach(Collider c in colliders)
		{
			RaycastHit hit;
			Physics.Raycast(sourceCam.transform.position, c.transform.position - sourceCam.transform.position, out hit, 200.5f, 448);
			if(hit.transform == c.transform && (c.tag == "Player" || c.tag == "Dummy") && !c.gameObject.GetComponent<PhotonView>().IsMine && Vector3.Angle(looking,c.transform.position-sourceCam.transform.position) <= 15f && Vector3.Distance(sourceCam.transform.position,c.transform.position) <= 200f)
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
			ChildBind(closest.GetComponent<PhotonView>().ViewID);
		}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(claw.childCount == 0)
			PhotonNetwork.Destroy(gameObject);
		if(claw.position == transform.position && claw.childCount > 0)
		{
			Transform child = claw.GetChild(0);
			child.parent = null;
		}
		acceleration += 2f*Time.fixedDeltaTime;
		claw.position = Vector3.MoveTowards(claw.position, transform.position, (speed+acceleration)*Time.fixedDeltaTime);
	}
	
	public void ChildBind(int id)
	{
		view.RPC("RPC_ChildBind", RpcTarget.All, id);
	}
	
	[PunRPC]
	void RPC_ChildBind(int id)
	{
		Transform t = PhotonView.Find(id).GetComponent<Transform>();
		t.parent = gameObject.transform;
		t.localPosition = Vector3.zero;
	}
}
