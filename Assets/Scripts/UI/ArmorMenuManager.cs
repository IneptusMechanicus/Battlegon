using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorMenuManager : MonoBehaviour {

	// The first five variables are the transforms for each armor piece
	[SerializeField]
	Transform Head;

	[SerializeField]
	Transform ShoulderLeft;

	[SerializeField]
	Transform ShoulderRight;

	[SerializeField]
	Transform ShinguardLeft;

	[SerializeField]
	Transform ShinguardRight;

	//The prefab for the armor piece selection buttons
	[SerializeField]
	Button ListButton;

	//References to the armor piece group buttons for onClick setup
	[SerializeField]
	Button ShoulderLeftBtn;

	[SerializeField]
	Button ShoulderRightBtn;

	[SerializeField]
	Button ShinguardLeftBtn;

	[SerializeField]
	Button ShinguardRightBtn;

	[SerializeField]
	Button HelmetBtn;

	//The content panel, where the selection buttons will be listed
	[SerializeField]
	RectTransform List;

	//These arrays contain the "In-Game" names for each armor piece
	//These names show up on the Customization Menu buttons.
	[SerializeField]
	string[] ShoulderNames;

	[SerializeField]
	string[] ShinguardNames;

	[SerializeField]
	string[] HelmetNames;

	//The dictionary for every armor piece type, used in the menu generation
	//The dictionaries are generated this way due to them being unserializable in the unity inspector by default
	Dictionary<string, string> ShoulderDict = new Dictionary<string, string>();
	Dictionary<string, string> ShinguardDict = new Dictionary<string, string>();
	Dictionary<string, string> HelmetDict = new Dictionary<string, string>();

	public void SetUpDicts()
	{
		for(int i = 0; i < ShoulderNames.Length; i++)
		{
			ShoulderDict.Add(ShoulderNames[i], "Shoulder" + (i + 1).ToString());
		}
		for(int i = 0; i < ShinguardNames.Length; i++)
		{
			ShinguardDict.Add(ShinguardNames[i], "Shin" + (i + 1).ToString());
		}
		for(int i = 0; i < HelmetNames.Length; i++)
		{
			HelmetDict.Add(HelmetNames[i], "Helmet" + (i + 1).ToString());
		}
		ShoulderLeftBtn.onClick.AddListener(delegate {ListArmor(ShoulderLeft, "LeftShoulder", ShoulderDict);});
		ShoulderRightBtn.onClick.AddListener(delegate {ListArmor(ShoulderRight, "RightShoulder", ShoulderDict);});
		ShinguardLeftBtn.onClick.AddListener(delegate {ListArmor(ShinguardLeft, "LeftShinguard", ShinguardDict);});
		ShinguardRightBtn.onClick.AddListener(delegate {ListArmor(ShinguardRight, "RigthShinguard", ShinguardDict);});
		HelmetBtn.onClick.AddListener(delegate {ListArmor(Head, "Helmet", HelmetDict);});
	}

	//Sets up the saved armor configuration on the player prefab
	//Used both in armor menu and in-game to load armor
	void SetEquipedArmor()
	{
		if(PlayerPrefs.GetString("LeftShoulder") != "")
		{
			EquipArmor(PlayerPrefs.GetString("LeftShoulder"), ShoulderLeft, "LeftShoulder");
		}

		if(PlayerPrefs.GetString("RightShoulder") != "")
		{
			EquipArmor(PlayerPrefs.GetString("RightShoulder"), ShoulderRight, "RightShoulder");
		}

		if(PlayerPrefs.GetString("LeftShinguard") != "")
		{
			EquipArmor(PlayerPrefs.GetString("LeftShinguard"), ShinguardLeft, "LeftShinguard");
		}

		if(PlayerPrefs.GetString("RightShinguard") != "")
		{
			EquipArmor(PlayerPrefs.GetString("RightShinguard"), ShinguardRight, "RightShinguard");
		}

		if(PlayerPrefs.GetString("Helmet") != "")
		{
			EquipArmor(PlayerPrefs.GetString("Helmet"), Head, "Helmet");
		}
	}

	//Loads armor prefab with name armorName into the scene
	// and calls SetArmor to apply it on the player at armorPos.
	void EquipArmor(string armorName, Transform armorPos, string prefsName)
	{
		GameObject armorPiece = Resources.Load(armorName) as GameObject;
		SetArmor(armorPiece, armorPos, prefsName);
	}

	//Sets the armorPiece object at armorPos
	//When applied, the selection is saved in PlayerPrefs with key prefName
	void SetArmor(GameObject armorPiece, Transform armorPos, string prefsName)
	{
		foreach(Transform child in armorPos)
		{
			if(child.gameObject.tag =="Armor")
    		Destroy(child.gameObject);
		}

		Instantiate(armorPiece, armorPos.transform);
		PlayerPrefs.SetString(prefsName, armorPiece.name);
	}

	//CLears the selection buttons list
	void ClearList()
	{
		foreach(Transform child in List.transform)
		{
    		Destroy(child.gameObject);
		}
	}

	//Generates selection buttons for each armor piece in the armor type
	public void ListArmor(Transform pos, string keyToSet, Dictionary<string, string> dict)
	{
		ClearList();
		int ListPos = 20;
		foreach(KeyValuePair<string, string> entry in dict)
		{
			ListPos -= 40;
			Button b = Instantiate(ListButton, List.transform) as Button;
			b.transform.GetChild(0).GetComponent<Text>().text = entry.Key;
			b.onClick.AddListener(delegate {EquipArmor(entry.Value, pos, keyToSet);});

			b.GetComponent<RectTransform>().transform.SetParent(List.transform);
			b.GetComponent<RectTransform>().transform.localPosition = new Vector3(
				b.GetComponent<RectTransform>().transform.localPosition.x,
				ListPos,
				b.GetComponent<RectTransform>().transform.localPosition.z);
			Debug.Log(b.GetComponent<RectTransform>().transform.localPosition.y);
		}
	}

	void Start ()
	{
		SetEquipedArmor();
	}
}
