using UnityEngine;
using System.Collections;

public class CommonButtonAction : MonoBehaviour {

	public void ResetCurrentScene()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
//#elif UNITY_WEBPLAYER
//         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }

    public void LoadMenu()
    {
        Application.LoadLevel("Menu");
    }
}
