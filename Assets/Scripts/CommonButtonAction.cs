using UnityEngine;
using System.Collections;

public class CommonButtonAction : MonoBehaviour {

	public void ResetCurrentScene()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}
}
