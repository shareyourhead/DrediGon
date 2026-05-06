using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
	PhotonView PV;	

	public float minX;
	public float maxX;
	public float minZ;
	public float maxZ;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}
	
	void Start()
	{
		if (PV.IsMine)
		{
			CreateController();
		}
	}

	void CreateController()
	{
		Vector3 randomPosition = new Vector3 (Random.Range(minX, maxX), 25f, Random.Range(minZ, maxZ)); 
		
		object[] customInstantiationData = new object[]
		{
		PV.Controller.NickName,
		Random.value,
		Random.value,
		Random.value
		};

		PhotonNetwork.Instantiate("Player", randomPosition, Quaternion.identity, 0, customInstantiationData);
	}
}
