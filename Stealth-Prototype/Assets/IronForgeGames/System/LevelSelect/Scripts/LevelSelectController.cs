using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour 
{
	[SerializeField]
	private Transform levelSelectButtonParent;

	[SerializeField]
	private GameObject levelSelectButtonPrefab;

	private void Start() 
	{
		PopulateList();
	}

	private void PopulateList()
	{
		foreach(AH.Max.LevelData _level in ManifestManager.Instance.CurrentManifest.Levels)
		{
			var _newLevelButton = Instantiate(levelSelectButtonPrefab, Vector3.zero, Quaternion.identity);
			_newLevelButton.transform.SetParent(levelSelectButtonParent);

			LevelSelectButton _levelSelectButton = _newLevelButton.GetComponent<LevelSelectButton>();
			if(_levelSelectButton != null)
			{
				_levelSelectButton.level = _level;
				_levelSelectButton.buttonText.text = _level.name;
			}
		}
	}
}
