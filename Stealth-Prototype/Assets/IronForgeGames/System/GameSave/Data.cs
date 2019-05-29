using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data 
{
	///<Summary>
	///the slot that this data is linked to
	///</Summary>
	public int _slot;

	///<Summary>
	///the actual path that this data is linked to 
	///</Summary>
	public string savePath;

	///<Summary>
	///A list of character data. This will likely keep a count of one but I want to account for character. 
 	///</Summary>
	public List <PlayerCharacterData> characterDataList;
}

public class PlayerCharacterData
{
	public string characterName;
	public Vector3 position;
	public Quaternion rotation;

	///<Summary>
	/// The game level
	///</Summary>
	public string currentlevel;
}


