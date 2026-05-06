using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using Photon.Pun;

public class DiscordController : MonoBehaviourPunCallbacks
{
	
	public Discord.Discord discord;
	public static DiscordController Instance;	
	
	void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
        	discord = new Discord.Discord(967842848747688036, (System.UInt64)Discord.CreateFlags.Default);
	}

    // Update is called once per frame
	void Update()
	{
        	discord.RunCallbacks();
	}

	public override void OnJoinedLobby()
	{
		StatusLobby();
	}	
	
	public override void OnJoinedRoom()
	{
		StatusRoom();
	}	
	
	public override void OnLeftRoom()
	{
		StatusLobby();
	}

	

	public void StatusExit()
	{
        	discord = new Discord.Discord(967842848747688036, (System.UInt64)Discord.CreateFlags.Default);
		var activityManager = discord.GetActivityManager();
		
		activityManager.ClearActivity((result) => {
			Debug.Log(result == Discord.Result.Ok ? "Cleared Activity" : "Failed to Clear Activity");
		});
	}

	public void StatusLobby()
	{
		discord = new Discord.Discord(967842848747688036, (System.UInt64)Discord.CreateFlags.Default);
		var activityManager = discord.GetActivityManager();
		var activity = new Discord.Activity
		{
			State = "In a lobby...",
			Details = "Free-for-all",
		};
		activityManager.UpdateActivity(activity, (res) => 
		{
			if (res == Discord.Result.Ok)
			{
				Debug.Log("Updated to Lobby Status");
			}
			else 
			{
				Debug.Log("Failed to update to Lobby Status");
			}
		});
	}

	public void StatusRoom()
	{
		discord = new Discord.Discord(967842848747688036, (System.UInt64)Discord.CreateFlags.Default);
		var activityManager = discord.GetActivityManager();
		var activity = new Discord.Activity
		{
			State = "In a Room...",
			Details = "Free-for-all",
			Party = 
			{
				Id = "Public",
				Size = {
					CurrentSize = 1,
					MaxSize = 4,
				}
			}
		};
		activityManager.UpdateActivity(activity, (res) => 
		{
			if (res == Discord.Result.Ok)
			{
				Debug.Log("Updated to Room Status");
			}
			else 
			{
				Debug.Log("Failed to update to Room Status");
			}
		});
	}

	public void StatusGame()
	{
		discord = new Discord.Discord(967842848747688036, (System.UInt64)Discord.CreateFlags.Default);
		var activityManager = discord.GetActivityManager();
		var activity = new Discord.Activity
		{
			State = "Playing Game",
			Details = "Free-for-all",
		};
		activityManager.UpdateActivity(activity, (res) => 
		{
			if (res == Discord.Result.Ok)
			{
				Debug.Log("Updated to Game Status");
			}
			else 
			{
				Debug.Log("Failed to update to Game Status");
			}
		});
	}
}
