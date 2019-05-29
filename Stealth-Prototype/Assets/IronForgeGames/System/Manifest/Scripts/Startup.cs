using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour 
{
	private void Start () 
	{
		ManifestManager.Instance.LoadGame();
	}
}
