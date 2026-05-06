using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Trap : MonoBehaviour, IPunInstantiateMagicCallback
{
	public PhotonView source;
	public int sourceID;
	public float health = 200f;
	public Transform Cylinder;

	PhotonView view;
	//Every time a new trap is spawned, deal 50 damage to existing traps

    // Start is called before the first frame update
	void Start()
	{
		//Cylinder.localEulerAngles = transform.eulerAngles;
		//transform.eulerAngles = Vector3.zero;
		view = GetComponent<PhotonView>();
	}

	void FixedUpdate()
	{
		Cylinder.GetComponent<Renderer>().material.color = new Color(0f,0f,Mathf.Clamp01(0.0025f*health));
		if(transform.childCount == 1)
		{
			Transform closest = null;
			float minDist = Mathf.Infinity;
			Collider[] colliders = Physics.OverlapSphere(transform.position, 0.75f);
			foreach(Collider c in colliders)
			{
				if((c.tag == "Player" || c.tag == "Dummy") && c.gameObject.GetComponent<PhotonView>().ViewID != sourceID)
				{
					// Debug.Log(c.gameObject.GetComponent<PhotonView>().ViewID);
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
				ChildBind(closest.GetComponent<PhotonView>().ViewID);
			}
		}
		else
		{
			//Damage self and child
			if(transform.childCount > 1)
			{
				float d = 12.5f*Time.fixedDeltaTime;
				if(transform.GetChild(1).tag == "Player")
				{
					transform.GetChild(1).GetComponent<PlayerMovement>().TakeDamage(d); 
					source.GetComponent<PlayerMovement>().Points(d);
					Damage(4*d, sourceID);
				}
				if(transform.GetChild(1).tag == "Dummy")
				{
					transform.GetChild(1).GetComponent<Dummy>().health = Mathf.Max(0f, transform.GetChild(1).GetComponent<Dummy>().health - d);
					source.GetComponent<PlayerMovement>().Points(d);
					Damage(4*d, sourceID);
				}
			}
			//health = Mathf.Max(0f,health-50f*Time.deltaTime);
		}
		if(health <= 0f)
		{
			if(transform.childCount > 1)
			{
				Debug.Log(transform.GetChild(1));
				Transform child = transform.GetChild(1);
				child.parent = null;
				Destroy();
			}
			//sourcePlayer.transform.GetComponent<PlayerMovement>().myTraps.Remove(transform);
		}
	}
	
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		object[] instantiationData = info.photonView.InstantiationData;
		sourceID = (int)instantiationData[0];
		source = PhotonView.Find(sourceID);
	}

	public void Damage(float d, int id)
	{
		if(id == sourceID)
			view.RPC("RPC_Damage", RpcTarget.All, d, id);
	}
	
	public void Destroy()
	{
		view.RPC("RPC_Destroy", RpcTarget.All);
	}
	
	public void ChildBind(int id)
	{
		view.RPC("RPC_ChildBind", RpcTarget.All, id);
	}

	[PunRPC]
	void RPC_Damage(float d, int id) //damage, and source
	{
		health -= d;
	}

	[PunRPC]
	void RPC_Destroy()
	{
		PhotonNetwork.Destroy(gameObject);
	}
	
	[PunRPC]
	void RPC_ChildBind(int id)
	{
		Transform t = PhotonView.Find(id).GetComponent<Transform>();
		t.parent = gameObject.transform;
		t.localPosition = new Vector3(0f,1.2f,0f);
	}
}
