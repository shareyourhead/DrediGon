using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
	
	public string input = "Public";

	public void CreateRoom()
	{
		// Server
		
		PhotonNetwork.CreateRoom(input);
	}

	public void JoinRoom()
	{
		// Client
		// PhotonNetwork.JoinRoom("Testing")
		// For When These Actually Get Set Up
		
		PhotonNetwork.JoinRoom(input);
	}

	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel("MainGame");
	}
	
	public void ReadStringInput(string s){
		input =s;
		Debug.Log(input);
		
	}
	
}
