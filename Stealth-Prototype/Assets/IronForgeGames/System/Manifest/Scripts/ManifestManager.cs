using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AH.Max;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ManifestManager : Singleton_MonoBehavior<ManifestManager>
{
	[SerializeField]
	private ManifestType currentManifestType;
	public ManifestType CurrentManifestType
	{
		get { return currentManifestType; }
	}

	[SerializeField]
	private Manifest currentManifest;
	public Manifest CurrentManifest
	{
		get { return currentManifest; }
		set{ currentManifest = value; }
	}

	private Manifest testManifest;
	private Manifest FullManifest;
	private Manifest demoManifest;

	private Coroutine loadGameRoutine;
	private Coroutine loadLevelRoutine;

	private static InitializationStarted initializationStarted = new InitializationStarted();
	public static InitializationStarted InitializationStarted
	{
		get { return initializationStarted; }
	}

	private static InitializationEnded initializationEnded = new InitializationEnded();
	public static InitializationEnded InitializationEnded
	{
		get { return initializationEnded; }
	}

	///<Summary>
	///
	///</Summary>
	public static ContentLoadedEvent contentLoadedEvent = new ContentLoadedEvent();

	///<Summary>
	///
	///</Summary>
	public static ContentsLoadedEvent contentsLoadedEvent = new ContentsLoadedEvent();

	public static void SetManifestType( Manifest _manifest )
	{ 
		if( _manifest == null ) return;

		Instance.currentManifestType = _manifest.ManifestType;		
		Instance.currentManifest = _manifest;
	} 

	public void LoadGame()
	{
		loadGameRoutine = StartCoroutine(LoadGameRoutine(currentManifest));
	}

	public void LoadLevel(AH.Max.LevelData _level)
	{
		loadLevelRoutine = StartCoroutine(LoadLevelRoutine(_level));
	}	

	private void LoadResources(Manifest _manifest)
	{
		SceneManager.LoadScene(_manifest.ResourcesScene.name, LoadSceneMode.Additive);
	}

	private IEnumerator LoadGameRoutine(Manifest _manifest)
	{
		float _screenTime = 3f;

		AsyncOperation _LoadResourcesAsync = SceneManager.LoadSceneAsync(_manifest.ResourcesScene.name, LoadSceneMode.Additive);
		while(!_LoadResourcesAsync.isDone)
		{
			yield return null;
		}

		foreach(AH.Max.LevelData _scene in _manifest.StartUpScenes)
		{
			AsyncOperation _asyncLoad = SceneManager.LoadSceneAsync(_scene.name);

			while(!_asyncLoad.isDone)
			{
				yield return null;
			}

			if(contentLoadedEvent != null)
			{
				contentLoadedEvent.Invoke(_scene);
			}

			yield return new WaitForSecondsRealtime(_screenTime);
		}

		AsyncOperation _asyncLoadMainMenuLoad = SceneManager.LoadSceneAsync(_manifest.MainMenu.name);
		
		while(!_asyncLoadMainMenuLoad.isDone)
		{
			yield return null;
		}

		if(contentsLoadedEvent != null)
		{
			contentsLoadedEvent.Invoke();
		}

		yield break;
	}

	private IEnumerator LoadLevelRoutine(AH.Max.LevelData _level)
	{
		//load all the dependencies in the level
		foreach(AH.Max.LevelData _dependency in _level.dependencies)
		{
			AsyncOperation _asyncLoad = SceneManager.LoadSceneAsync(_dependency.name);

			while(!_asyncLoad.isDone)
			{
				yield return null;
			}

			if(contentLoadedEvent != null)
			{
				contentLoadedEvent.Invoke(_dependency);
			}
		}

		AsyncOperation _asyncLoadMainMenuLoad = SceneManager.LoadSceneAsync(_level.name);
		
		while(!_asyncLoadMainMenuLoad.isDone)
		{
			yield return null;
		}

		if(contentsLoadedEvent != null)
		{
			contentsLoadedEvent.Invoke();
		}
		yield break;
	}

	private void OnInitializationStarted()
	{	
		if(initializationStarted != null)
		{
			initializationStarted.Invoke(GameStates.Initializing);
		}
	}

	private void OnInitializationEnded()
	{
		if(initializationStarted != null)
		{
			initializationStarted.Invoke(GameStates.MainMenu);
		}
	}
}



