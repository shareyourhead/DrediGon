using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenuController : MonoBehaviour
{
    public string MainGame;
    public string MainMenu;
    public string SettingsMainMenu;
    public string MenuScene;
    public string Tutorial;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadGame()
    {
        SceneManager.LoadScene(MenuScene);
    }

    public void SettingsOpen()
    {
        SceneManager.LoadScene(SettingsMainMenu);
    }
	
	public void TutorialOpen()
    {
        SceneManager.LoadScene(Tutorial);
    }
	
	public void TutorialClose()
    {
        SceneManager.LoadScene(MainMenu);
    }

    public void SettingsMainClose()
    {
            SceneManager.LoadScene(MainMenu); 
    }

    public void SettingsPauseClose()
    {
        SceneManager.LoadScene(MainGame);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenuLoad()
    {
         PhotonNetwork.LeaveRoom();
	 SceneManager.LoadScene(MainMenu);
    }



}
