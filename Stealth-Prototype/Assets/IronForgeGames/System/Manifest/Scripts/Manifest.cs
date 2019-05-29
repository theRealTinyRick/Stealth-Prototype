using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Manifest", menuName = "CompanyName/Manifest", order = 1)]
public class Manifest : ScriptableObject
{
	[SerializeField]
	private ManifestType manifestType;
	public ManifestType ManifestType
	{
		get { return manifestType; }
	}

	///<Summary>
	/// Theses are scenes that are actual playable levels in the game.
	///</Summary>
	[Tooltip("Theses are scenes that are actual playable levels in the game.")]
	[SerializeField]
	private List <AH.Max.LevelData> levels = new List<AH.Max.LevelData>();
	public List <AH.Max.LevelData> Levels
	{
		get { return levels; }
	}

	///<Summary>
	/// These are the scenes that the game uses on startup. 
	///</Summary>
	[Tooltip("These are the scenes that the game uses on startup. ")]
	[SerializeField]
	private List <AH.Max.LevelData> startUpScenes = new List<AH.Max.LevelData>();
	public List <AH.Max.LevelData> StartUpScenes
	{
		get { return startUpScenes; }
	}

	///<Summary>
	/// Theses are scenes that the game needs to run the game logic. UI, Entities, ect...
	///</Summary>
	[Tooltip("Theses are scenes that the game needs to run the game logic. UI, Entities, ect...")]
	[SerializeField]
	private List <UnityEngine.SceneManagement.Scene> systemScenes = new List <UnityEngine.SceneManagement.Scene>();
	public List <UnityEngine.SceneManagement.Scene> SystemScenes
	{
		get { return systemScenes; }
	}

	///<Summary>
	/// The Main Menu Scene
	///</Summary>
	[Tooltip("The Main Menu Scene")]
	[SerializeField]
	private AH.Max.LevelData mainMenu;
	public AH.Max.LevelData MainMenu
	{
		get { return mainMenu; }
	}

	///<Summary>
	///The level select scenes
	///</Summary>
	[SerializeField]
	private AH.Max.LevelData levelSelect;
	public AH.Max.LevelData LevelSelect
	{
		get { return levelSelect; }
	}

	///<Summary>
	/// Theses are scenes that the game needs to run the game logic. UI, Entities, ect...
	///</Summary>
	[Tooltip("These are thing like input drivers and what not. They are all singletons")]
	[SerializeField]
	private AH.Max.LevelData resourcesScene;
	public AH.Max.LevelData ResourcesScene
	{
		get { return resourcesScene; }
	}

	[Button]
	public void Activate()
	{
		ManifestManager.SetManifestType(this);		
	}
}

