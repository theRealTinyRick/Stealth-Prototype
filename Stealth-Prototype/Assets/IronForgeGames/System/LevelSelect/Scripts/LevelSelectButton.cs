using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectButton : MonoBehaviour 
{
	public AH.Max.LevelData level;
	public TextMeshProUGUI buttonText;

	public void LoadLevel()
	{
		ManifestManager.Instance.LoadLevel(level);
	}
}
