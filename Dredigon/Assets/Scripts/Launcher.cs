using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
	public static Launcher Instance;
	
	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] TMP_InputField nickNameInputField;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject playerListItemPrefab;
	[SerializeField] GameObject startGameButton; 
	[SerializeField] TMP_InputField RoomNameInput;
	[SerializeField] Toggle PrivateToggle;
	
	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
		PhotonNetwork.ConnectUsingSettings();	
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}
	
	public override void OnJoinedLobby() 
	{
		MenuManager.Instance.OpenMenu("Title");
		Debug.Log("Connecting to Title Menu");
	}

	public void SetNickName()
	{
		string nickname = nickNameInputField.text;
		if (nickname == "Enter your Nickname...")
		{
			PhotonNetwork.LeaveRoom();
			errorText.text = "Please Choose a Nickname";
			MenuManager.Instance.OpenMenu("Error");
		}
		else
		{
			PhotonNetwork.NickName = nickname;
		}
	}

	public void CreateRoom()
	{
		if (string.IsNullOrEmpty(roomNameInputField.text))
			return;
		if (PrivateToggle.isOn)
		{
			PhotonNetwork.CreateRoom("PRIVATE_"+roomNameInputField.text);
		}
		else 
		{
			PhotonNetwork.CreateRoom(roomNameInputField.text);
		}

		MenuManager.Instance.OpenMenu("Loading");
	}

	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("Loading");
		SetNickName();
	}
	
	public void JoinRoomByName()
	{
		PhotonNetwork.JoinRoom(RoomNameInput.text);
	}

	public override void OnJoinedRoom()
	{
		MenuManager.Instance.OpenMenu("Room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;
	
		SetNickName();

		foreach (Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}
		
		for(int i=0; i<PhotonNetwork.PlayerList.Count(); i++)
		{
			Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(PhotonNetwork.PlayerList[i]);
		}

		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
		MenuManager.Instance.OpenMenu("Error");
		
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("Loading");
	}

	public override void OnLeftRoom()
	{
		MenuManager.Instance.OpenMenu("Title");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach(Transform trans in roomListContent)
		{
			Destroy(trans.gameObject);
		}
		for(int i=0; i<roomList.Count; i++)
		{
			if(roomList[i].RemovedFromList)
				continue;
			if(roomList[i].Name.StartsWith("PRIVATE_"))
				continue;
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
		}	
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}
	public void StartGame()
	{
		//DiscordController.Instance.StatusGame();
		PhotonNetwork.LoadLevel("MainGame");
	}

	public void QuitGame()
	{
		DiscordController.Instance.StatusExit();
		Application.Quit();
	}
}
