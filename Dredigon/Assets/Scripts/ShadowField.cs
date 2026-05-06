using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShadowField : MonoBehaviour, IPunInstantiateMagicCallback
{
	public int viewId;
	public GameObject sourcePlayer;

	public float lifetime = 20f;
	
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		object[] instantiationData = info.photonView.InstantiationData;
		viewId = (int)instantiationData[0];
		sourcePlayer = PhotonView.Find(viewId).gameObject;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position,6.5f);
		foreach(Collider c in colliders)
		{
			if(c.tag == "Player" && c.gameObject.GetComponent<PhotonView>().ViewID != viewId)
			{
				PlayerMovement player = c.transform.GetComponent<PlayerMovement>();
				if(c.transform.GetComponent<PhotonView>().IsMine)
					player.TakeKnockback(Mathf.Min(-1f,player.yVel), -Vector3.Normalize(Vector3.MoveTowards(transform.position, c.transform.position, 1f)-transform.position+Vector3.up*0.5f));
				// player.gravityCoef = -Vector3.Normalize(Vector3.MoveTowards(transform.position, c.transform.position, 1f)-transform.position+Vector3.up*0.5f);
				// player.yVel = Mathf.Min(-1f,player.yVel);
				player.TakeDamage(60f*Time.fixedDeltaTime);
				sourcePlayer.transform.GetComponent<PlayerMovement>().Points(60f*Time.fixedDeltaTime);
			}
			//Dev testing dummy
			if(c.tag == "Dummy")
			{
				Dummy dummy = c.transform.GetComponent<Dummy>();
				dummy.gravityCoef = -Vector3.Normalize(Vector3.MoveTowards(transform.position, c.transform.position, 1f)-transform.position+Vector3.up*0.5f);
				dummy.yVel = Mathf.Min(-1f,dummy.yVel);
				dummy.health = Mathf.Max(0f,dummy.health-60f*Time.fixedDeltaTime);
				sourcePlayer.transform.GetComponent<PlayerMovement>().Points(60f*Time.fixedDeltaTime);
			}
		}
		lifetime -= Time.deltaTime;
		if(lifetime <= 0f)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}
}
