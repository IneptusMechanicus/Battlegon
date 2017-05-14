using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFuncScript : MonoBehaviour
{
    //Basic UI functions for the overall game

    //Reference to the Main Menu Canvas object
    [SerializeField]
    GameObject MainMenu;

    //Reference to the Armor Menu Canvas object
    [SerializeField]
    GameObject ArmorMenu;

    //Input field for the player's in-game name
    [SerializeField]
    InputField playerName;

    void Start()
    {
        if(PlayerPrefs.GetString("PlayerName") != "")
        {
            playerName.placeholder.GetComponent<Text>().text = PlayerPrefs.GetString("PlayerName");
        }

        playerName.onEndEdit.AddListener(delegate {SavePlayerName(playerName.text);});
    }


    //Loads the InGame scene
    public void LoadPlayScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }

    //Shuts the game down
    public void QuitGame()
    {
        Application.Quit();
    }

    //Disables the Main Menu and enables the Armor Menu
    public void LoadArmorMenu()
    {
        MainMenu.GetComponent<Canvas>().enabled = false;
        ArmorMenu.GetComponent<Canvas>().enabled = true;
    }

    //Disables the Armor Menu and enables the Main Menu
    public void LoadMainMenu()
    {
        MainMenu.GetComponent<Canvas>().enabled = true;
        ArmorMenu.GetComponent<Canvas>().enabled = false;
    }

    //Event method for the playerName input field, which saves his name in PlayerPrefs
    public void SavePlayerName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }
}
