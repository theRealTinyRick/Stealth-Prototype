using System;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void OnEnable() 
    {
        InputDriver.anyButtonWasPressedEvent.AddListener(LoadLevelSelect);
    }
    private void OnDisable() 
    {
        InputDriver.anyButtonWasPressedEvent.AddListener(LoadLevelSelect);
    }

    private void LoadLevelSelect()
    {
        Debug.Log("Load the scene");

        ManifestManager.Instance.LoadLevel(ManifestManager.Instance.CurrentManifest.LevelSelect);
    }
}